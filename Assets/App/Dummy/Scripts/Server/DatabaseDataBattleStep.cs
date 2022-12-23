using System;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataBattleStep
    {
        #region fields
        public string charID;
        public string skillID;
        public float damage;
        public float coldown;
        #endregion

        #region constructor;
        public DatabaseDataBattleStep(){}
        public DatabaseDataBattleStep(string chardID,string skillID,float damage,float coldown = 0){
            this.charID = chardID;
            this.skillID = skillID;
            this.damage = damage;
            this.coldown = coldown;
        }
        #endregion
    }
}