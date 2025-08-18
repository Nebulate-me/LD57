using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                if (File.Exists(JsonPath))
                {
                    var json = File.ReadAllText(JsonPath);
                    _data = JsonUtility.FromJson<HighscoreData>(json) ?? new HighscoreData();
                }
                #endif
                #if UNITY_WEBGL
                // TODO
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
            try
            {
                var directoryPath = Path.GetDirectoryName(JsonPath); 
                Directory.CreateDirectory(directoryPath);
                var json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(JsonPath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save highscores: {ex}");
            }
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