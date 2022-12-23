using System.Collections.Generic;
using Evesoft;

namespace NFTGame.Dummy.Adapter
{
    public static class AdapterCharacterSet
    {
        public static  Server.DatabaseDataCharacterPreset ToDataDatabase(this Config.CharacterSetConfig data){
            return new  Server.DatabaseDataCharacterPreset(){
                id              = data.id,
                headid          = data.head? data.head.id : "",
                bodyid          = data.body? data.body.id : "",
                armid           = data.arm? data.arm.id : "",
                legid           = data.leg? data.leg.id : "",     
                helmetid        = data.helmet? data.helmet.id : "",
                facialHairid    = data.facialHair? data.facialHair.id : "",
                clothid         = data.cloth? data.cloth.id : "",
                sleeveid        = data.sleeve? data.sleeve.id : "",
                pantsid         = data.pants? data.pants.id : "",
                weaponid        = data.weapon? data.weapon.id : ""
            };
        }
        public static Server.DatabaseDataCharacterPreset[] ToDataDatabase(this IList<Config.CharacterSetConfig> data){
            if(data.IsNullOrEmpty())
                return default(Server.DatabaseDataCharacterPreset[]);

            var result = new Server.DatabaseDataCharacterPreset[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToDataDatabase();
            }
            return result;
        } 
        
    }
}