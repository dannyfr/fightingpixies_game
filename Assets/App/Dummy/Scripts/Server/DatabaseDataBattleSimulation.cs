using System;
using Sirenix.OdinInspector;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataBattleSimulation
    {
        #region fields
        public string char1ID,char2ID;
        public string winID,loseID;
        [TableList]
        public DatabaseDataBattleStep[] steps;
        #endregion

        #region constructor
        public DatabaseDataBattleSimulation(){}
        public DatabaseDataBattleSimulation(string char1,string char2): this(){
            this.char1ID = char1;
            this.char2ID = char2;
        }
        #endregion
    }
}