using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerProfileService : IPlayerProfileService
    {
        private const string FILE_NAME = "players.json";
        #if UNITY_WEBGL && !UNITY_EDITOR
        private const string PLAYER_PREFS_KEY = "PLAYER_PROFILES_JSON_v1";
        #endif
        
        public IReadOnlyList<PlayerProfile> Players => _players;

        public PlayerProfile CurrentPlayer { get; set; }

        private List<PlayerProfile> _players = new List<PlayerProfile>();

        private string JsonPath => Path.Combine(Application.persistentDataPath, FILE_NAME);

        public PlayerProfileService()
        {
            Load();
        }

        public bool TrySetCurrentPlayerById(string existingPlayerId, out PlayerProfile currentPlayer)
        {
            if (!TryGetPlayerById(existingPlayerId, out currentPlayer)) return false;
            
            CurrentPlayer = currentPlayer;
            return true;
        }

        public bool TryGetPlayerById(string id, out PlayerProfile player)
        {
            return _players.TryGetFirst(p => string.Equals(p.Id, id, StringComparison.Ordinal), out player);
        }

        public bool TryGetPlayerByName(string playerName, out PlayerProfile player)
        {
            return _players.TryGetFirst(p => string.Equals(p.Name, playerName, StringComparison.Ordinal), out player);
        }

        /// <summary>
        ///     Creates a new player with the given id+name. Returns false if id already exists.
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
            _players = new List<PlayerProfile>();
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
            {
                var json = PlayerPrefs.GetString(PLAYER_PREFS_KEY, "{}");
                var data = JsonUtility.FromJson<PlayerProfilesData>(json) ?? new PlayerProfilesData();
                _players = data.Players;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PlayerProfiles] WebGL load failed: {ex}");
        }
#else
            try
            {
                if (File.Exists(JsonPath))
                {
                    var json = File.ReadAllText(JsonPath);
                    var data = JsonUtility.FromJson<PlayerProfilesData>(json) ?? new PlayerProfilesData();
                    _players = data.players;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerProfiles] Load failed: {ex}");
            }
#endif
            RemoveDuplicateIds();
        }

        private void Save(bool clear = false)
        {
            var saveData = new PlayerProfilesData(_players);
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
    }
}