using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts.Utils;
using Sirenix.Utilities;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerProfileService : IPlayerProfileService
    {
        public IReadOnlyList<PlayerProfile> Players => _players;
        public PlayerProfile CurrentPlayer { get; set; }

        private List<PlayerProfile> _players = new();
        private string JsonPath => Path.Combine(Application.persistentDataPath, FILE_NAME);
        
        private const string DEFAULT_PLAYER_NAME = "Аноним";
        private const string FILE_NAME = "players.json";
        private const string PRESET_RESOURCE_NAME = "preset_players"; // Resources/preset_players.json, do not move around
#if UNITY_WEBGL && !UNITY_EDITOR
        private const string PLAYER_PREFS_KEY = "PLAYER_PROFILES_JSON_v1";
#endif
        public PlayerProfileService()
        {
            Load();
        }
        
        private void SetCurrentPlayerDefault()
        {
            if (TryGetPlayerById(DEFAULT_PLAYER_NAME, out var existingDefaultPlayer))
            {
                CurrentPlayer = existingDefaultPlayer;
                return;
            }

            if (TryCreatePlayer(DEFAULT_PLAYER_NAME, out var newDefaultPlayer))
            {
                CurrentPlayer = newDefaultPlayer;
            }
        }

        public void SetCurrentPlayer(PlayerProfile player)
        {
            CurrentPlayer = player;
            Save();
            
        }

        public bool TrySetCurrentPlayerById(string existingPlayerId, out PlayerProfile currentPlayer)
        {
            if (!TryGetPlayerById(existingPlayerId, out currentPlayer)) return false;
            
            CurrentPlayer = currentPlayer;
            Save();
            return true;
        }

        public bool TryGetPlayerById(string id, out PlayerProfile player)
        {
            if (id.IsNullOrWhitespace())
            {
                player = null;
                return false;
            }
            return _players.TryGetFirst(p => string.Equals(p.Id, id, StringComparison.Ordinal), out player);
        }

        public bool TryGetPlayerByName(string playerName, out PlayerProfile player)
        {
            return _players.TryGetFirst(p => string.Equals(p.Name, playerName, StringComparison.Ordinal), out player);
        }

        /// <summary>
        /// Creates a new player with the given id+name. Returns false if id already exists.
        /// </summary>
        public bool TryCreatePlayer(string name, out PlayerProfile player)
        {
            var id = Guid.NewGuid().ToString();
            name = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();

            if (string.IsNullOrEmpty(id))
                id = Guid.NewGuid().ToString("N"); // fallback
            
            if (TryGetPlayerById(id, out PlayerProfile existingPlayer))
            {
                player = existingPlayer;
                return false;
            }

            var profile = new PlayerProfile(id, name);
            _players.Add(profile);
            CurrentPlayer ??= profile;
            Save();
            player = profile;
            return true;
        }

        public bool TrySetPlayerScore(string playerId, int score)
        {
            if (!TryGetPlayerById(playerId, out var player))
            {
                return false;
            }

            if (!player.SetScore(score))
            {
                return false;
            }
            
            Save();
            return true;
        }

        public bool TrySetCurrentPlayerScore(int score)
        {
            return CurrentPlayer != null && TrySetPlayerScore(CurrentPlayer.Id, score);
        }

        public List<PlayerProfile> GetPlayersSortedByScore()
        {
            return _players.OrderByDescending(p => p.Score)
                .ThenBy(p => p.CreatedUtc)
                .ToList();
        }
        
        public void ClearAll()
        {
            _players.Clear();
            Save(true);
        }

        // ---------- Persistence ----------
        private void Load()
        {
            Debug.Log($"[PlayerProfiles] > loading player profiles from {JsonPath}.");
            _players = new List<PlayerProfile>();

            var presetPlayers = LoadPresetPlayers();

            string json = string.Empty;
#if UNITY_WEBGL && !UNITY_EDITOR
    try
    {
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            json = PlayerPrefs.GetString(PLAYER_PREFS_KEY, "{}");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"[PlayerProfiles] WebGL load failed: {ex}");
        // fall back to presets only
        _players = presetPlayers;
        SetCurrentPlayerDefault();
        RemoveDuplicateIds();
        return;
    }
#else
            try
            {
                if (File.Exists(JsonPath))
                {
                    json = File.ReadAllText(JsonPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerProfiles] Load failed: {ex}");
                // fall back to presets only
                _players = presetPlayers;
                SetCurrentPlayerDefault();
                RemoveDuplicateIds();
                return;
            }
#endif

            var savedData = string.IsNullOrWhiteSpace(json)
                ? new PlayerProfilesData()
                : JsonUtility.FromJson<PlayerProfilesData>(json) ?? new PlayerProfilesData();

            var savedPlayers = savedData.players ?? new List<PlayerProfile>();
            
            _players = MergePlayers(presetPlayers, savedPlayers);
            
            if (!string.IsNullOrEmpty(savedData.currentPlayerId) &&
                TryGetPlayerById(savedData.currentPlayerId, out var currentPlayer))
            {
                CurrentPlayer = currentPlayer;
            }
            else
            {
                Debug.LogWarning($"[PlayerProfiles] Could not get the current player by id: [{savedData.currentPlayerId}]");
                SetCurrentPlayerDefault();
            }

            RemoveDuplicateIds();
        }
        
        private void Save(bool clear = false)
        {
            if (CurrentPlayer == null)
            {
                Debug.LogWarning("[PlayerProfileService] Current player is null, cannot save!");
                return;
            }
            var saveData = new PlayerProfilesData(CurrentPlayer.Id, _players);
            var json = JsonUtility.ToJson(saveData, prettyPrint: true);
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (clear)
            {
                PlayerPrefs.DeleteKey(PLAYER_PREFS_KEY);
            }
            else
            {
                PlayerPrefs.SetString(PLAYER_PREFS_KEY, json);
            }
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PlayerProfiles] WebGL save failed: {ex}");
        }
#else
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(JsonPath));
                if (clear)
                {
                    if (File.Exists(JsonPath)) File.Delete(JsonPath);
                }
                else
                {
                    File.WriteAllText(JsonPath, json);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerProfiles] Save failed: {ex}");
            }
#endif
        }

        // Make sure no duplicate IDs sneak in (e.g., from manual edits).
        private void RemoveDuplicateIds()
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var i = _players.Count - 1; i >= 0; i--)
            {
                var id = _players[i].Id;
                if (string.IsNullOrEmpty(id) || !seen.Add(id))
                    _players.RemoveAt(i);
            }
        }
        
        private List<PlayerProfile> LoadPresetPlayers()
        {
            try
            {
                var textAsset = Resources.Load<TextAsset>(PRESET_RESOURCE_NAME);
                if (textAsset == null || string.IsNullOrWhiteSpace(textAsset.text))
                {
                    Debug.Log("[PlayerProfiles] No preset_players.json found in Resources or it's empty.");
                    return new List<PlayerProfile>();
                }

                var data = JsonUtility.FromJson<PlayerProfilesData>(textAsset.text);
                var presetPlayers =  data?.players ?? new List<PlayerProfile>();
                return presetPlayers.Where(player => player.Score > 0).ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerProfiles] Failed to load preset players: {ex}");
                return new List<PlayerProfile>();
            }
        }
        
        /// <summary>
        /// Merge preset and saved players by Id. Saved overrides preset on conflicts.
        /// </summary>
        private List<PlayerProfile> MergePlayers(
            List<PlayerProfile> presetPlayers,
            List<PlayerProfile> savedPlayers)
        {
            var dict = new Dictionary<string, PlayerProfile>(StringComparer.Ordinal);

            if (presetPlayers != null)
            {
                foreach (var p in presetPlayers)
                {
                    if (p == null || string.IsNullOrEmpty(p.Id)) continue;
                    dict[p.Id] = p;
                }
            }

            if (savedPlayers != null)
            {
                foreach (var p in savedPlayers)
                {
                    if (p == null || string.IsNullOrEmpty(p.Id)) continue;
                    
                    dict[p.Id] = p; 
                }
            }

            return dict.Values
                .OrderByDescending(p => p.Score)
                .ThenBy(p => p.CreatedUtc)
                .ToList();
        }
    }
}