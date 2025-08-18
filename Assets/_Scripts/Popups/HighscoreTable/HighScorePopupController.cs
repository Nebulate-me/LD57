using _Scripts.Score;
using TMPro;
using UnityEngine;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Popups.HighscoreTable
{
    public class HighScorePopupController : MonoBehaviour
    {
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject highscoreEntryRowPrefab;

        [Inject] private IScoreSaver _scoreSaver;
        [Inject] private IPrefabPool _prefabPool;
        
        public void Refresh()
        {
            foreach (Transform c in contentParent) Destroy(c.gameObject);

            var entries = _scoreSaver.Entries;
            for (var i = 0; i < entries.Count; i++)
            {
                var row = _prefabPool.Spawn(highscoreEntryRowPrefab, contentParent).GetComponent<HighScoreEntryRow>();
                row.SetUp(i+1, entries[i]);
                // TODO: Replace with a SetUp
            }
        }

        // Buttons
        public void ClearAll()
        {
            _scoreSaver.ClearAll();
            Refresh();
        }

        public void ExportTxt()
        {
            _scoreSaver.SaveToTextFile();
        }
    }
}