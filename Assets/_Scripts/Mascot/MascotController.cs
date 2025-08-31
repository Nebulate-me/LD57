using System.Collections;
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
        [SerializeField, TextArea(3, 5)] private string startGameMessage = @"Добро пожаловать в игру СибТехПроект. 
СибТехПроект - это Сибирские Технологии Проектирования. 
Мы уже 17 лет быстро и качественно 
проектируем многоквартирные дома
 и гордимся каждым своим объектом.
Предлагаем тебе почувствовать себя частью 
команды СибТехПроекта и 
попробовать спроектировать свой многоквартирный дом.
";

        [Inject] private IScoreManager _scoreManager;
        
        private Coroutine showRoutine;
        private Coroutine typeRoutine;

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
            string msg = startGameMessage;
            Show(msg);
            #endif
        }

        public void Show(string message)
        {
            if (showRoutine != null) StopCoroutine(showRoutine);
            if (typeRoutine != null) StopCoroutine(typeRoutine);

            mascotPopup.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // Prepare text
            contentBubbleText.text = message;
            shownBubbleText.maxVisibleCharacters = 0;
            shownBubbleText.text = message;

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
            // If typewriter hasn’t finished, finish instantly. Else, hide.
            if (useTypewriter && shownBubbleText.maxVisibleCharacters < shownBubbleText.text.Length)
            {
                shownBubbleText.maxVisibleCharacters = shownBubbleText.text.Length;
            }
            else
            {
                Hide();
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

            // Typewriter
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
