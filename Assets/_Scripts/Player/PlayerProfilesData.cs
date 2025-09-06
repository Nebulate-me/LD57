using System;
using System.Collections.Generic;

namespace _Scripts.Player
{
    [Serializable]
    public class PlayerProfilesData
    {
        public List<PlayerProfile> players;

        public PlayerProfilesData()
        {
            players = new List<PlayerProfile>();
        }
        public PlayerProfilesData(List<PlayerProfile> players)
        {
            this.players = players;
        }
    }
}