using System;
using NFTGame.WebAPI.Data;
using Evesoft;
using System.Collections.Generic;

namespace NFTGame
{
    public static class AdapterSimulationData
    {
        public static BattleSimulationData ToData(this RawBattleSimulation data){
            if(data.IsNull())
                return default(BattleSimulationData);

            var result = new BattleSimulationData(){
                char1ID = data.pixies_id_1,
                char2ID = data.pixies_id_2,
                winID   = data.win_id,
                loseID  = data.los_id,
            };

            if(!data.steps.IsNullOrEmpty()){
                result.steps = new BattleStepData[data.steps.Length];
                for (var i = 0; i < data.steps.Length; i++)
                {
                    result.steps[i] = new BattleStepData(){
                        charID  = data.steps[i].pixiesid,
                        skillID = data.steps[i].skillId,
                        coldown = data.steps[i].cooldown,
                        damage  = data.steps[i].damage
                    };
                }
            }

            return result;
        }
        public static BattleSimulationData[] ToData(this IList<RawBattleSimulation> data){
            if(data.IsNullOrEmpty())
                return default(BattleSimulationData[]);

            var result = new BattleSimulationData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData();
            }
            return result;
        }
    }
}