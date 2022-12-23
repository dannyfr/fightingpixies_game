using System.Collections.Generic;
using Evesoft;

namespace NFTGame.Dummy.Adapter
{
    public static class AdapterBattleStep
    {
        public static Server.DatabaseDataBattleStep ToDataDatabase(this BattleStepData data){
            if(data.IsNull())
                return default(Server.DatabaseDataBattleStep);

            return new Server.DatabaseDataBattleStep(data.charID,data.skillID,data.damage,data.coldown);
        }
        public static Server.DatabaseDataBattleStep[] ToDataDatabase(this IList<BattleStepData> data){
            if(data.IsNullOrEmpty())
                return default(Server.DatabaseDataBattleStep[]);

            var result = new Server.DatabaseDataBattleStep[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToDataDatabase();
            }
            return result;
        }   
        public static BattleStepData ToData(this Server.DatabaseDataBattleStep data){
            if(data.IsNull())
                return default(BattleStepData);

            return new BattleStepData(data.charID,data.skillID,data.damage,data.coldown);
        }
        public static BattleStepData[] ToData(this IList<Server.DatabaseDataBattleStep> data){
            if(data.IsNullOrEmpty())
                return default(BattleStepData[] );

            var result = new BattleStepData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData();
            }
            return result;
        }
    }
}