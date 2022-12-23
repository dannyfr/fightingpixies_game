using System;

namespace NFTGame.WebAPI.Data
{
    

    [Serializable]
    public struct RawReward
    { 
        [Serializable]
        public struct Stats
        {
            public int hp;
            public int attack;
            public int defense;
            public int speed;
        }

        [Serializable]
        public struct Data{
            public string name;
            public string story;
            public Stats stats;
            public string[] set_heads;
            public string[] set_body;
            public string[] set_leg;
        }

        public string id;
        public Data data;
        public bool claimed;
    }
}