using System;
using NFTGame.WebAPI.Data;
using Evesoft;
using System.Collections.Generic;

namespace NFTGame
{
    public static class AdapterCharacterData
    {
        public static CharacterData ToData(this RawCharacter data){
            if(data.IsNull())
                return default(CharacterData);

            var result = new CharacterData(){
                id = data.id,
                name = data.name,
                story = data.story,
                showstat = data.showstat.IsNullOrEmpty()? default(Stats): (Stats)Enum.Parse(typeof(Stats),data.showstat,true),
                stats = new CharacterStats(){
                    hp = data.hp,
                    attack = data.attack,
                    defense = data.defense,
                    speed = data.speed, 
                },
                limbs = new CharacterLimbs(){
                    headID = data.head_id,
                    bodyID = data.body_id,
                    armID  = data.arm_id,
                    legID  = data.leg_id,
                },
                accessories = new CharacterAccessories(){
                    helmetID = data.helmet_id,
                    facialHairID = data.facial_hair_id,
                    clothID = data.cloth_id,
                    sleeveID = data.sleeve_id,
                    weaponID = data.weapon_id,
                    pantsID = data.pants_id,
                },
                status = new CharacterStatus(){
                    active = data.active,
                    battleCount = data.battle_attempt,
                    energy = data.energy,
                    maxEnergy = data.max_energy,
                    winRate = data.win_rate,
                }
            };
            return result;
        }
        public static CharacterData[] ToData(this IList<RawCharacter> data){
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