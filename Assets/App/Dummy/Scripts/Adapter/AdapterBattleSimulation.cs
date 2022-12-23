using System.Data.Common;
using System.Collections.Generic;
using Evesoft;

namespace NFTGame.Dummy.Adapter
{
    public static class AdapterBattleSimulation
    {
        public static Server.DatabaseDataBattleSimulation ToDataDatabase(this BattleSimulationData data){
            if(data.IsNull())
                return default(Server.DatabaseDataBattleSimulation);

            return new Server.DatabaseDataBattleSimulation(){
                char1ID = data.char1ID,
                char2ID = data.char2ID,
                winID  = data.winID,
                loseID = data.loseID,
                steps  = data.steps.ToDataDatabase() 
            };
        }
        public static Server.DatabaseDataBattleSimulation[] ToDataDatabase(this IList<BattleSimulationData> data){
            if(data.IsNullOrEmpty())
                return default(Server.DatabaseDataBattleSimulation[]);

            var result = new Server.DatabaseDataBattleSimulation[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToDataDatabase();
            }
            return result;
        } 
        public static BattleSimulationData ToData(this Server.DatabaseDataBattleSimulation data){
            if(data.IsNull())
                return default(BattleSimulationData);

            return new BattleSimulationData(){
                char1ID = data.char1ID,
                char2ID = data.char2ID,
                winID   = data.winID,
                loseID  = data.loseID,
                steps   = data.steps.ToData()
            };
        }
        public static BattleSimulationData[] ToData(this IList<Server.DatabaseDataBattleSimulation> data){
            if(data.IsNullOrEmpty())
                return default(BattleSimulationData[] );

            var result = new BattleSimulationData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData();
            }
            return result;
        }
      
    }
}