using System;
using UnityEngine;

namespace _Scripts.Player
{
    [Serializable]
    public class PlayerProfile
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private int score;
        [SerializeField] private long createdUtcTicks;
        
        public string Id => id;
        public string Name => name;
        public int Score => score;
        public DateTime CreatedUtc => new DateTime(createdUtcTicks, DateTimeKind.Utc);

        public PlayerProfile(string id, string name)
        {
            this.id = id?.Trim();
            this.name = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
            score = 0;
            this.createdUtcTicks = DateTime.UtcNow.Ticks;
        }

        public bool SetScore(int newScore)
        {
            if (newScore <= score) return false;
            
            score = newScore;
            return true;

        }
    }
}