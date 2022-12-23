using System;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct BattleAttack{
        #region fields
        public Character attacker;
        public Character victim;
        public float damage;
        #endregion

        #region constructor
        public BattleAttack(Character attacker,Character victim,float damage){
            this.attacker = attacker;
            this.victim = victim;
            this.damage = damage;
        }
        #endregion
    }
}