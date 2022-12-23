using System.Collections.Generic;
using Evesoft;
using UnityEngine;

namespace NFTGame.Dummy.Adapter
{
    public static class AdapterCharacterData
    {
        public static Server.DatabaseDataCharacter ToDataDatabase(this CharacterData data){
            return new  Server.DatabaseDataCharacter(){
                id          = data.id,
                name        = data.name, 
                active      = data.status.active,
                headid      = data.limbs.headID,
                bodyid      = data.limbs.bodyID,
                armid       = data.limbs.armID,
                legid       = data.limbs.legID,
                helmetid    = data.accessories.helmetID,
                facialHairid = data.accessories.facialHairID,
                clothid     = data.accessories.clothID,
                sleeveid    = data.accessories.sleeveID,
                pantsid     = data.accessories.pantsID,
                weaponid    = data.accessories.weaponID,
                attack      = Mathf.RoundToInt(data.stats.attack),
                defense     = Mathf.RoundToInt(data.stats.defense),
                speed       = Mathf.RoundToInt(data.stats.speed),
                hp          = Mathf.RoundToInt(data.stats.hp),
                battleCount = data.status.battleCount,
                energy      = data.status.energy,
                maxEnergy   = data.status.maxEnergy,
                winRate     = data.status.winRate,
                story       = data.story
            };
        }
        public static Server.DatabaseDataCharacter[] ToDataDatabase(this IList<CharacterData> data){
            if(data.IsNullOrEmpty())
                return default(Server.DatabaseDataCharacter[]);

            var result = new Server.DatabaseDataCharacter[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToDataDatabase();
            }
            return result;
        }      
        public static CharacterData ToData(this Server.DatabaseDataCharacter data){
            return new CharacterData(){
                id = data.id,
                name = data.name,
                stats = new CharacterStats(){
                    hp      = data.hp,
                    attack  = data.attack,
                    defense = data.defense,
                    speed   = data.speed
                },
                limbs = new CharacterLimbs(){
                    headID = data.headid,
                    bodyID = data.bodyid,
                    armID = data.armid,
                    legID = data.legid
                },
                accessories = new CharacterAccessories(){
                    helmetID = data.helmetid,
                    facialHairID = data.facialHairid,
                    clothID = data.clothid,
                    sleeveID = data.sleeveid,
                    pantsID = data.pantsid,
                    weaponID = data.weaponid
                },
                status = new CharacterStatus(){
                    active = data.active,
                    energy = data.energy,
                    maxEnergy = data.maxEnergy,
                    winRate = data.winRate,
                    battleCount = data.battleCount,
                },
                story = data.story,
            };
        }
        public static CharacterData[] ToData(this IList<Server.DatabaseDataCharacter> data){
            if(data.IsNullOrEmpty())
                return default(CharacterData[]);

            var result = new CharacterData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData();
            }
            return result;
        }
    
    }
}