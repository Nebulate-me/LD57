using System;
using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Screens
{
    [Serializable]
    internal class FakePlayerProfileDto
    {
        [SerializeField] private int score;
        [SerializeField] private string playerName;
        public PlayerProfile ToPlayerProfile()
        {
            var id = Guid.NewGuid().ToString();
            var profile = new PlayerProfile(id, playerName);
            profile.SetScore(score);
            
            return profile;
        }
    }
}