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
        [Header("Persistence")] 
        [SerializeField] private readonly string fileName = "players.json";
        [SerializeField] private string playerPrefsKey = "PLAYER_PROFILES_JSON_v1";

        public IReadOnlyList<PlayerProfile> Players => _data.players;

        private PlayerProfilesData _data = new PlayerProfilesData();

        private string JsonPath => Path.Combine(Application.persistentDataPath, fileName);

        private void Awake()
        {
            Load();
        }
        
        public bool TryGetPlayer(string id, out PlayerProfile player)
        {
            return _data.players.TryGetFirst(p => string.Equals(p.Id, id, StringComparison.Ordinal), out player);
        }

        /// <summary>
        ///     Creates a new player with the given id+name. Returns false if id already exists.
        /// </summary>
        public bool TryCreatePlayer(string id, string name, out PlayerProfile player)
        {
            id = (id ?? "").Trim();
            name = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();

            if (string.IsNullOrEmpty(id))
                id = Guid.NewGuid().ToString("N"); // fallback
            
            if (TryGetPlayer(id, out PlayerProfile existingPlayer))
            {
                player = existingPlayer;
                return false;
            }

            var profile = new PlayerProfile(id, name);
            _data.players.Add(profile);
            Save();
            player = profile;
            return true;
        }

        public bool TrySetPlayerScore(string playerId, int score)
        {
            if (!TryGetPlayer(playerId, out var player))
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
        
        public List<PlayerProfile> GetPlayersSortedByScore()
        {
            return _data.players.OrderByDescending(p => p.Score)
                .ThenBy(p => p.CreatedUtc)
                .ToList();
        }
        
        public void ClearAll()
        {
            _data.players.Clear();
            Save(true);
        }

        // ---------- Persistence ----------

        private void Load()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (PlayerPrefs.HasKey(playerPrefsKey))
            {
                var json = PlayerPrefs.GetString(playerPrefsKey, "{}");
                _data = JsonUtility.FromJson<PlayerProfilesData>(json) ?? new PlayerProfilesData();
            }
            else _data = new PlayerProfilesData();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PlayerProfiles] WebGL load failed: {ex}");
            _data = new PlayerProfilesData();
        }
#else
            try
            {
                if (File.Exists(JsonPath))
                {
                    var json = File.ReadAllText(JsonPath);
                    _data = JsonUtility.FromJson<PlayerProfilesData>(json) ?? new PlayerProfilesData();
                }
                else
                {
                    _data = new PlayerProfilesData();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerProfiles] Load failed: {ex}");
                _data = new PlayerProfilesData();
            }
#endif
            DedupIds();
        }

        private void Save(bool clear = false)
        {
            var json = JsonUtility.ToJson(_data, prettyPrint: true);
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (clear)
            {
                PlayerPrefs.DeleteKey(playerPrefsKey);
            }
            else
            {
                PlayerPrefs.SetString(playerPrefsKey, json);
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
        private void DedupIds()
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var i = _data.players.Count - 1; i >= 0; i--)
            {
                var id = _data.players[i].Id;
                if (string.IsNullOrEmpty(id) || !seen.Add(id))
                    _data.players.RemoveAt(i);
            }
        }
    }
}