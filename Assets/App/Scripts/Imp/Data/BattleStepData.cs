using System;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct BattleStepData{
        #region fields
        public string charID;
        public string skillID;
        public float damage;
        public float coldown;
        #endregion

        #region constructor;
        public BattleStepData(string chardID,string skillID,float damage,float coldown = 0){
            this.charID = chardID;
            this.skillID = skillID;
            this.damage = damage;
            this.coldown = coldown;
        }
        #endregion
    }
}