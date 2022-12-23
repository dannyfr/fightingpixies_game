using System;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public struct RawCharacter
    {
        public string id;
        public string owner;
        public string name;
        public string story;
        public string showstat;
        public int hp;
        public int attack;
        public int defense;
        public int speed;
        public string head_id;
        public string body_id;
        public string arm_id;
        public string leg_id;
        public string cloth_id;
        public string helmet_id;
        public string facial_hair_id;
        public string pants_id;
        public string sleeve_id;
        public string weapon_id;
        public bool active;
        public bool fighting;
        public int energy;
        public int max_energy;
        public float win_rate;
        public int battle_attempt;
    }
}