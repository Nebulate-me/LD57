using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts.Popups.HighscoreTable;
using Signals;
using UnityEngine;

namespace _Scripts.Score
{
    public class ScoreSaver : MonoBehaviour, IScoreSaver
    {
        [Header("Config")] 
        [SerializeField] private int maxEntries = 10;

        [Tooltip("File name used inside Application.persistentDataPath")] 
        [SerializeField] private string jsonFileName = "highscores.json";
        [SerializeField] private string textFileName = "highscores.txt";

        private const string PlayerPrefsKey = "Highscores";
        
        public List<HighscoreEntry> Entries => _data.entries;

        private HighscoreData _data = new();
        private string JsonPath => Path.Combine(Application.persistentDataPath, jsonFileName);
        private string TextPath => Path.Combine(Application.persistentDataPath, textFileName);

        private void Awake()
        {
            Load();
        }

        public void SubmitScore(int score, string playerName = null)
        {
            _data.entries.Add(new HighscoreEntry(playerName, score));
            SortAndTrim();
            Save();
            SignalsHub.DispatchAsync(new GameFinishedSignal());
        }

        public void ClearAll()
        {
            _data.entries.Clear();
            Save();
        }

        public void SaveToTextFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TextPath));
                using (var sw = new StreamWriter(TextPath, false))
                {
                    sw.WriteLine("=== HIGHSCORES ===");
                    var i = 1;
                    foreach (var e in _data.entries)
                    {
                        var dt = new DateTime(e.timestampUtc, DateTimeKind.Utc).ToLocalTime();
                        sw.WriteLine($"{i,2}. {e.playerName,-16}  {e.score,6}  [{dt:yyyy-MM-dd HH:mm}]");
                        i++;
                    }
                }
#if UNITY_EDITOR
                Debug.Log($"Highscores written to: {TextPath}");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write highscores txt: {ex}");
            }
        }

        private void Load()
        {
            try
            {
                #if UNITY_STANDALONE_WIN
                try
                {
                    if (File.Exists(JsonPath))
                    {
                        var json = File.ReadAllText(JsonPath);
                        _data = JsonUtility.FromJson<HighscoreData>(json) ?? new HighscoreData();
                        Debug.Log($"{_data.entries.Count} Highscores read from: {TextPath}");
                    }
                    else
                    {
                        _data = new HighscoreData();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Highscores] File Load failed: {ex}");
                    _data = new HighscoreData();
                }
                #endif
                #if UNITY_WEBGL
                try
                {
                    if (PlayerPrefs.HasKey(PlayerPrefsKey))
                    {
                        var json = PlayerPrefs.GetString(PlayerPrefsKey, "{}");
                        _data = JsonUtility.FromJson<HighscoreData>(json) ?? new HighscoreData();
                        Debug.Log($"{_data.entries.Count} Highscores read");
                    }
                    else
                    {
                        _data = new HighscoreData();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Highscores] WebGL load failed: {ex}");
                    _data = new HighscoreData();
                }
                #endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load highscores: {ex}");
                _data = new HighscoreData();
            }

            SortAndTrim();
        }

        private void Save()
        {
            var json = JsonUtility.ToJson(_data, true);

        #if UNITY_WEBGL
            try
            {
                PlayerPrefs.SetString(PlayerPrefsKey, json);
                PlayerPrefs.Save(); // important on WebGL
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Highscores] WebGL save failed: {ex}");
            }
        #endif
        #if UNITY_STANDALONE_WIN
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(JsonPath) ?? string.Empty);
                System.IO.File.WriteAllText(JsonPath, json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Highscores] Save failed: {ex}");
            }
        #endif
        }

        private void SortAndTrim()
        {
            _data.entries = _data.entries
                .OrderByDescending(e => e.score)
                .ThenBy(e => e.timestampUtc) // earlier wins tie
                .Take(Mathf.Max(1, maxEntries))
                .ToList();
        }
    }
}