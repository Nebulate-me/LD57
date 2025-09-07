using System;
using System.Collections.Generic;

namespace _Scripts.Player
{
    [Serializable]
    public class PlayerProfilesData
    {
        public string currentPlayerId;
        public List<PlayerProfile> players;

        public PlayerProfilesData()
        {
            currentPlayerId = string.Empty;
            players = new List<PlayerProfile>();
        }
        public PlayerProfilesData(string currentPlayerId, List<PlayerProfile> players)
        {
            this.currentPlayerId = currentPlayerId;
            this.players = players;
        }
    }
}