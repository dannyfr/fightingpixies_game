using System;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public struct RawBattleStep{
        public string pixiesid;
        public string skillId;
        public float damage;
        public float cooldown;
    }
}