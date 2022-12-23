using System;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct CharacterStats
    {
        #region fields
        public float hp;
        public float attack;
        public float defense;
        public float speed;
        #endregion

        // #region constructor
        // public CharacterStats(float hp,float attack,float defense,float speed){
        //     this.hp = hp;
        //     this.attack = attack;
        //     this.defense = defense;
        //     this.speed = speed;
        // }
        // #endregion
    }
}

