using System.Collections;
using System.Collections.Generic;
using _Scripts.Missions;
using _Scripts.Popups.StartGame;
using Signals;
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

        [Header("Messages")]
        [Tooltip("Phrases will be shown one after another on each click.")]
        [SerializeField, TextArea(2, 5)]
        private string[] startGamePhrases = {
            "Добро пожаловать в игру СибТехПроект.",
            "СибТехПроект — это Сибирские Технологии Проектирования.",
            "Мы уже 17 лет быстро и качественно проектируем многоквартирные дома и гордимся каждым своим объектом.",
            "Предлагаем тебе почувствовать себя частью команды и попробовать спроектировать свой дом."
        };

        [Inject] private IScoreManager _scoreManager;

        private Coroutine showRoutine;
        private Coroutine typeRoutine;

        // sequence state
        private readonly List<string> _activePhrases = new List<string>();
        private int _phraseIndex = 0;

        void Awake()
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            mascotPopup.SetActive(false);
        }

        void OnEnable()
        {
            SignalsHub.AddListener<StartGamePopupClosedSignal>(OnGameStarted);
            if (clickCatcher) clickCatcher.onClick.AddListener(OnClicked);
        }

        void OnDisable()
        {
            SignalsHub.RemoveListener<StartGamePopupClosedSignal>(OnGameStarted);
            if (clickCatcher) clickCatcher.onClick.RemoveListener(OnClicked);
        }

        private void OnGameStarted(StartGamePopupClosedSignal signal)
        {
#if SKIP_TUTORIAL
            _scoreManager.StartGame();
#else
            // Use configured phrases
            ShowSequence(startGamePhrases);
#endif
        }

        /// <summary>
        /// Starts showing a sequence of phrases (fades in once, then click-through).
        /// </summary>
        public void ShowSequence(IEnumerable<string> phrases)
        {
            _activePhrases.Clear();
            if (phrases != null) _activePhrases.AddRange(phrases);
            if (_activePhrases.Count == 0)
            {
                // nothing to show, just continue
                _scoreManager.StartGame();
                return;
            }

            _phraseIndex = 0;

            if (showRoutine != null) StopCoroutine(showRoutine);
            if (typeRoutine != null) StopCoroutine(typeRoutine);

            mascotPopup.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // prime first phrase
            SetPhrase(_activePhrases[_phraseIndex], resetVisible: true);

            showRoutine = StartCoroutine(FadeInThenType());
        }

        /// <summary>
        /// Keeps the old API around — shows a single message as a 1-phrase sequence.
        /// </summary>
        public void Show(string message)
        {
            ShowSequence(new[] { message });
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
            if (_phraseIndex < _activePhrases.Count - 1)
            {
                _phraseIndex++;
                // stop any previous typing coroutine
                if (typeRoutine != null) StopCoroutine(typeRoutine);
                SetPhrase(_activePhrases[_phraseIndex], resetVisible: true);

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

            _activePhrases.Clear();
            _phraseIndex = 0;

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
