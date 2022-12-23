using System;
using NFTGame.WebAPI.Data;
using Evesoft;
using System.Collections.Generic;

namespace NFTGame
{
    public static class AdapterRewardData
    {
        public static RewardData ToData(this RawReward data,Component.CharacterBuilder builder){
            if(data.IsNull() || builder.IsNull())
                return default(RewardData);

            var setHead  = builder.GetSetsVariantConfig(data.data.set_heads[0]);
            var setBody  = builder.GetSetsVariantConfig(data.data.set_body[0]);
            var setLeg   = builder.GetSetsVariantConfig(data.data.set_leg[0]);
            var charData = new CharacterData(){
                name  = data.data.name,
                story = data.data.story,
                stats = new CharacterStats(){
                    hp      = data.data.stats.hp,
                    attack  = data.data.stats.attack,
                    defense = data.data.stats.defense,
                    speed   = data.data.stats.speed,
                },
                setsCollections = new CharacterSetCollections(){
                    heads = data.data.set_heads,
                    bodys = data.data.set_body,
                    legs  = data.data.set_leg,
                },
                limbs = new CharacterLimbs(){
                    headID = setHead?.head?.id,
                    bodyID = setBody?.body?.id,
                    armID  = setBody?.arm?.id,
                    legID  = setLeg?.leg?.id,
                },
                accessories = new CharacterAccessories(){
                    helmetID     = setHead?.helmet?.id,
                    facialHairID = setHead?.facialHair?.id,
                    clothID      = setBody?.cloth?.id,
                    sleeveID     = setBody?.sleeve?.id,
                    weaponID     = setBody?.weapon?.id,
                    pantsID      = setLeg?.pants?.id,
                },
                status = new CharacterStatus(),
            };
            var character= builder.Create(charData);
                character?.Hide();

            var result = new RewardData(){
                id = data.id,
                character = character
            };
            return result;
        }
        public static RewardData[] ToData(this IList<RawReward> data,Component.CharacterBuilder builder){
            if(data.IsNullOrEmpty())
                return default(RewardData[]);

            var result = new RewardData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData(builder);
            }
            return result;
        }
    }
}