using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Player;
using _Scripts.Popups.HighScore;
using Signals;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Screens
{
    public class HighScoreScreenController : MonoBehaviour
    {
        [SerializeField] private List<FakePlayerProfileDto> fakePlayerProfiles = new();
        [SerializeField] private float scrollDuration = 1f;
        
        [Header("UI")] 
        [SerializeField] private Transform contentParent;
        [SerializeField] private ScrollRect contentScrollRect;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private Button returnButton;

        [Inject] private IPrefabPool _prefabPool;
        [Inject] private IPlayerProfileService _playerProfileService;
        [Inject] private IScreenManager _screenManager;
        
        private Coroutine scrollRoutine;

        private void Awake()
        {
            if (!contentParent) Debug.LogWarning("HighScoreScreenController: contentParent not set.");
            if (!rowPrefab) Debug.LogWarning("HighScoreScreenController: rowPrefab not set.");
        }

        private void OnEnable()
        {
            if (returnButton) returnButton.onClick.AddListener(OnReturnToMainMenuButtonClicked);
            
            SignalsHub.AddListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }

        private void OnDisable()
        {
            if (returnButton) returnButton.onClick.RemoveListener(OnReturnToMainMenuButtonClicked);
            
            SignalsHub.RemoveListener<PlayerProfilePopupClosedSignal>(OnPlayerProfilePopupClosed);
        }
        

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            for (var i = contentParent.childCount - 1; i >= 0; i--)
                Destroy(contentParent.GetChild(i).gameObject);

            var currentPlayer = _playerProfileService.CurrentPlayer;
            var players = _playerProfileService.Players
                // .Concat(fakePlayerProfiles.TakeLast(10).Select(p => p.ToPlayerProfile()))
                .OrderByDescending(player => player.Score)
                .ThenBy(player => player.CreatedUtc)
                .ToList();
            HighScoreEntryRow currentRow = null;
            
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var row = _prefabPool.Spawn(rowPrefab, contentParent).GetComponent<HighScoreEntryRow>();

                var isCurrent = currentPlayer != null && player.Id == currentPlayer.Id;
                if (isCurrent) currentRow = row;

                row.SetUp(
                    index: i + 1,
                    entry: player,
                    isHighlighted: isCurrent
                );
            }
            
            if (currentRow != null)
                ScrollToChild(currentRow.GetComponent<RectTransform>());
        }
        
        private void OnPlayerProfilePopupClosed(PlayerProfilePopupClosedSignal signal)
        {
            Refresh();
        }

        private void OnReturnToMainMenuButtonClicked()
        {
            _screenManager.GoToMainMenuScreen();
        }
        
        /// <summary>
        /// Smoothly scrolls so the target child is centered in the viewport.
        /// </summary>
        public void ScrollToChild(RectTransform target)
        {
            if (!contentScrollRect || !target) return;

            if (scrollRoutine != null) StopCoroutine(scrollRoutine);
            scrollRoutine = StartCoroutine(AnimateScroll(target));
        }

        private IEnumerator AnimateScroll(RectTransform target)
        {
            yield return new WaitForSeconds(0.1f);
            Canvas.ForceUpdateCanvases(); // make sure layout is updated

            var content = contentScrollRect.content;
            var viewport = contentScrollRect.viewport != null ? contentScrollRect.viewport : contentScrollRect.GetComponent<RectTransform>();

            // Calculate world positions
            Vector3 childWorldPos = target.position;
            Vector3 vpWorldPos = viewport.position;

            // Convert to content-local
            Vector2 childLocal = content.InverseTransformPoint(childWorldPos);
            Vector2 vpLocal = content.InverseTransformPoint(vpWorldPos);

            // Offset needed to align vertically
            float offset = vpLocal.y - childLocal.y - target.sizeDelta.y;

            // Target anchoredPosition.y
            float targetY = content.anchoredPosition.y + offset;

            // Clamp within scrollable range
            float minY = 0f;
            float maxY = Mathf.Max(0f, content.rect.height - viewport.rect.height);
            targetY = Mathf.Clamp(targetY, minY, maxY);

            float startY = content.anchoredPosition.y;
            float elapsed = 0f;

            while (elapsed < scrollDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / scrollDuration);
                float newY = Mathf.Lerp(startY, targetY, t);

                content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);

                yield return null;
            }

            content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);
            scrollRoutine = null;
        }
    }
}