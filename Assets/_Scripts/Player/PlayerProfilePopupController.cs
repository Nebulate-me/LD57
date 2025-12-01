using System.Collections;
using System.Linq;
using _Scripts.Screens;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Player
{
    public class PlayerProfilePopupController : MonoBehaviour
    {
        [Header("UI Refs")] 
        [SerializeField] private CanvasGroup group;
        [SerializeField] private TextMeshProUGUI currentPlayerLabel;
        [SerializeField] private TMP_Dropdown existingPlayersDropdown;
        [SerializeField] private TMP_InputField newPlayerNameInput;
        [SerializeField] private TextMeshProUGUI errorLabel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI confirmButtonText;

        [Header("Behavior")] 
        [SerializeField] private float fadeDuration = 0.15f;
        [SerializeField] private bool closeOnConfirm = true;

        [Inject] private IPlayerProfileService _playerProfileService;

        private string _selectedPlayerId; 

        private void Awake()
        {
            if (errorLabel) errorLabel.gameObject.SetActive(false);
            if (group)
            {
                group.alpha = 0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

        private void OnEnable()
        {
            if (confirmButton) confirmButton.onClick.AddListener(OnConfirm);
            if (existingPlayersDropdown) existingPlayersDropdown.onValueChanged.AddListener(OnDropdownChanged);
            if (newPlayerNameInput) newPlayerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
        }

        private void OnDisable()
        {
            if (confirmButton) confirmButton.onClick.RemoveListener(OnConfirm);
            if (existingPlayersDropdown) existingPlayersDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
            if (newPlayerNameInput) newPlayerNameInput.onValueChanged.RemoveListener(OnPlayerNameChanged);
        }
        
        public void Open()
        {
            if (_playerProfileService == null)
            {
                Debug.LogError("PlayerProfilesService not present.");
                return;
            }

            RefreshUI();
            
            if (group) StartCoroutine(Fade(0f, 1f, fadeDuration, true));
        }

        public void Close()
        {
            SignalsHub.DispatchAsync(new PlayerProfilePopupClosedSignal());
            if (group)
            {
                StartCoroutine(Fade(1f, 0f, fadeDuration, false));
            }
        }
        
        private void RefreshUI()
        {
            var currentPlayer = _playerProfileService.CurrentPlayer;

            // Dropdown
            if (existingPlayersDropdown)
            {
                existingPlayersDropdown.ClearOptions();
                var opts = _playerProfileService.Players
                    .OrderBy(p => p.Name)
                    .Select(p => new TMP_Dropdown.OptionData(p.Name))
                    .ToList();
                existingPlayersDropdown.AddOptions(opts);

                _selectedPlayerId = currentPlayer?.Id;
                var index = Mathf.Max(0, _playerProfileService.Players.ToList()
                    .FindIndex(p => p.Id == _selectedPlayerId));
                if (_playerProfileService.Players.Count > 0)
                    existingPlayersDropdown.SetValueWithoutNotify(index);
            }

            // Clear input & error
            if (newPlayerNameInput) newPlayerNameInput.text = "";
            ShowError(string.Empty);
            UpdateConfirmButton();
        }

        private void UpdateConfirmButton()
        {
            if (errorLabel.text != string.Empty)
            {
                confirmButton.interactable = false;
                confirmButtonText.text = "Исправьте ошибку";
                return;
            }
            
            confirmButton.interactable = true;
            confirmButtonText.text = newPlayerNameInput.text != string.Empty ? "Создать игрока" : "Подтвердить";
        }

        private void OnDropdownChanged(int idx)
        {
            var players = _playerProfileService.Players
                .OrderBy(player => player.Name)
                .ToList();
            if (idx >= 0 && idx < players.Count)
            {
                _selectedPlayerId = players[idx].Id;
                newPlayerNameInput.text = string.Empty;
            }
        }
        
        private void OnPlayerNameChanged(string playerName)
        {
            ShowError(string.Empty);
            UpdateConfirmButton();
        }

        private void OnConfirm()
        {
            if (_playerProfileService == null) return;

            // If user typed a new name, handle creation first.
            var typedName = newPlayerNameInput ? newPlayerNameInput.text.Trim() : "";
            newPlayerNameInput.SetTextWithoutNotify(typedName);
            if (!string.IsNullOrEmpty(typedName))
            {
                // Check for name collision (case-insensitive)
                if (_playerProfileService.TryGetPlayerByName(typedName, out var existing))
                {
                    ShowError($"Игрок уже существует.");
                    UpdateConfirmButton();
                    return;
                }

                // Create & set current
                if (_playerProfileService.TryCreatePlayer(typedName, out var created))
                {
                    _playerProfileService.SetCurrentPlayer(created);
                    RefreshUI();
                    if (closeOnConfirm) Close();
                    return;
                }
                
                ShowError($"Не получилось создать игрока.");
                UpdateConfirmButton();
                return;
            }

            // Otherwise use dropdown selection
            if (!string.IsNullOrEmpty(_selectedPlayerId))
            {
                if (_playerProfileService.TrySetCurrentPlayerById(_selectedPlayerId, out var player))
                {
                    RefreshUI();
                    if (closeOnConfirm) Close();
                    return;
                }

                ShowError("Не получилось установить текущего игрока.");
                UpdateConfirmButton();
                return;
            }

            // No input and no selection? Keep current, just close.
            if (closeOnConfirm) Close();
        }

        private void ShowError(string msg)
        {
            if (!errorLabel) return;
            if (string.IsNullOrEmpty(msg))
            {
                errorLabel.gameObject.SetActive(false);
                errorLabel.text = string.Empty;
            }
            else
            {
                errorLabel.gameObject.SetActive(true);
                errorLabel.text = msg;
            }
        }

        // ---- Optional fade ----
        private IEnumerator Fade(float from, float to, float dur, bool enable)
        {
            if (enable)
            {
                group.interactable = true;
                group.blocksRaycasts = true;
            }

            var t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / dur));
                yield return null;
            }

            group.alpha = to;
            if (!enable)
            {
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }
    }
}