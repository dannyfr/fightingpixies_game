using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public class BattleSimulationData:IDisposable
    {
        #region fields
        public string char1ID,char2ID;
        public string winID,loseID;
        [TableList]
        public BattleStepData[] steps;
        #endregion

        #region constructors
        public BattleSimulationData(){
            //steps = new List<BattleStepData>();
        }
        public BattleSimulationData(string char1,string char2) : this(){
            this.char1ID = char1;
            this.char2ID = char2;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            char1ID= char2ID = null;
            //steps.Clear();
            steps = null;
            winID = loseID = null;
        }
        #endregion
    }
}