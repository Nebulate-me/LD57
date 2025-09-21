using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions;
using ModestTree;
using Plugins.Sirenix.Odin_Inspector.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Mascot
{
    public class MascotPopupController : MonoBehaviour
    {
        [SerializeField] private GameObject mascotPopup;
        [FormerlySerializedAs("group")] [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI shownBubbleText;
        [SerializeField] private TextMeshProUGUI contentBubbleText;
        [SerializeField] private Button clickCatcher;

        [Header("Behavior")]
        [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private bool useTypewriter = true;
        [SerializeField] private float charsPerSecond = 55f;
        
        [Header("Tutorial Targets")]
        [SerializeField] private MascotTutorialTopPanelTargetToGameObjectDictionary topPanelTutorialTargets;

        [Header("Messages")] 
        [SerializeField] private MascotTutorialConfig tutorialConfig;

        [Inject] private IScoreManager _scoreManager;

        private Coroutine showRoutine;
        private Coroutine typeRoutine;
        
        private readonly List<MascotTutorialStepConfig> _stepConfigs = new();
        private int _activeStepIndex = 0;
        

        void Awake()
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            mascotPopup.SetActive(false);
        }

        void OnEnable()
        {
            if (clickCatcher) clickCatcher.onClick.AddListener(OnClicked);
        }

        void OnDisable()
        {
            if (clickCatcher) clickCatcher.onClick.RemoveListener(OnClicked);
        }

        private void Start()
        {
            ShowSequence(tutorialConfig.Steps);
        }

        /// <summary>
        /// Starts showing a sequence of phrases (fades in once, then click-through).
        /// </summary>
        public void ShowSequence(List<MascotTutorialStepConfig> stepConfigs)
        {
            _stepConfigs.Clear();
            if (stepConfigs == null) return;
            
            _stepConfigs.AddRange(stepConfigs);
            if (_stepConfigs.IsEmpty())
            {
                _scoreManager.StartGame();
                return;
            }

            _activeStepIndex = 0;

            if (showRoutine != null) StopCoroutine(showRoutine);
            if (typeRoutine != null) StopCoroutine(typeRoutine);

            mascotPopup.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            SetPhrase(_stepConfigs[_activeStepIndex].Phrase, resetVisible: true);
            SetTutorialTarget(stepConfigs[_activeStepIndex]);

            showRoutine = StartCoroutine(FadeInThenType());
        }

        public void Hide()
        {
            if (showRoutine != null) StopCoroutine(showRoutine);
            if (typeRoutine != null) StopCoroutine(typeRoutine);
            StartCoroutine(FadeOutAndDisable());
        }

        private void OnClicked()
        {
            // If typewriter hasn’t finished, complete immediately.
            if (useTypewriter && shownBubbleText.maxVisibleCharacters < shownBubbleText.text.Length)
            {
                shownBubbleText.maxVisibleCharacters = shownBubbleText.text.Length;
                return;
            }

            // Otherwise go to next phrase, or finish if this was the last.
            if (_activeStepIndex < _stepConfigs.Count - 1)
            {
                _activeStepIndex++;
                // stop any previous typing coroutine
                if (typeRoutine != null) StopCoroutine(typeRoutine);
                SetPhrase(_stepConfigs[_activeStepIndex].Phrase, resetVisible: true);
                SetTutorialTarget(_stepConfigs[_activeStepIndex]);

                if (useTypewriter)
                    typeRoutine = StartCoroutine(Typewriter(shownBubbleText, charsPerSecond));
            }
            else
            {
                // Last phrase → fade out and start the game
                Hide();
            }
        }

        private void SetPhrase(string phrase, bool resetVisible)
        {
            contentBubbleText.text = phrase;
            shownBubbleText.text = phrase;
            if (resetVisible) shownBubbleText.maxVisibleCharacters = 0;
        }

        private void ClearTutorialTarget()
        {
            foreach (var topPanelTutorialTarget in topPanelTutorialTargets.Values)
            {
                topPanelTutorialTarget.SetActive(false);
            }
        }
        
        private void SetTutorialTarget(MascotTutorialStepConfig stepConfig)
        {
            ClearTutorialTarget();
            
            if (stepConfig.TargetType == MascotTutorialTargetType.TopPanel)
            {
                if (topPanelTutorialTargets.TryGetValue(stepConfig.TopPanelTargetType, out var stepTarget))
                {
                    stepTarget.SetActive(true);
                }
            }
        }

        private IEnumerator FadeInThenType()
        {
            // Fade in
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // Typewriter for first phrase
            if (useTypewriter)
            {
                typeRoutine = StartCoroutine(Typewriter(shownBubbleText, charsPerSecond));
            }
        }

        private IEnumerator FadeOutAndDisable()
        {
            canvasGroup.interactable = false;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            mascotPopup.SetActive(false);

            _stepConfigs.Clear();
            _activeStepIndex = 0;

            _scoreManager.StartGame();
        }

        private static IEnumerator Typewriter(TMP_Text label, float cps)
        {
            label.maxVisibleCharacters = 0;
            int total = label.text.Length;
            float acc = 0f;
            while (label.maxVisibleCharacters < total)
            {
                acc += Time.unscaledDeltaTime * cps;
                int visible = Mathf.Min(total, Mathf.FloorToInt(acc));
                if (visible != label.maxVisibleCharacters)
                    label.maxVisibleCharacters = visible;
                yield return null;
            }
        }
    }
}
