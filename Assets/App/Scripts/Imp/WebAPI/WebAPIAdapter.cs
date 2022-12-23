using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Evesoft;

namespace NFTGame.WebAPI
{
    public static class WebAPIAdapter
    {
        public static HttpContent ToHttpContent(this Config.CharacterSetConfig target){
            if(target.IsNull())
                return null;
            
            var data = new Dictionary<string,object>(){
                    {"id",target.id},
                    {"head_id", target.head?.id},
                    {"body_id", target.body?.id},
                    {"arm_id",target.arm?.id},
                    {"leg_id", target.leg?.id},
                    {"cloth_id", target.cloth?.id},
                    {"helmed_id", target.helmet?.id},
                    {"facial_hair_id", target.facialHair?.id},
                    {"pants_id", target.pants?.id},
                    {"sleeve_id", target.sleeve?.id},
                    {"weapon_id", target.weapon?.id}
                };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this ISkill target){
            if(target.IsNull())
                return null;
            
            var data = new Dictionary<string,object>(){
                    {"id",target.id},
                    {"max_damage", target.maxDamage},
                    {"min_damage", target.minDamage},
                    {"max_cool_down", target.maxCoolDown},
                    {"min_cool_down",target.minCoolDown},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.ArmConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", BodyType.Arm.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.BodyConfig target){
            if(target.IsNull())
                return null;
            
            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", BodyType.Body.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.HeadConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", BodyType.Head.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.LegConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", BodyType.Leg.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.ClothConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", AccessoriesType.Cloth.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.FacialHairConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", "facial_hair"},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.HelmetConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", AccessoriesType.Helmet.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.PantsConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", AccessoriesType.Pants.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.SleeveConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", AccessoriesType.Sleeve.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this Config.WeaponConfig target){
            if(target.IsNull())
                return null;

            var data = new Dictionary<string,object>(){
                {"id",target.id},
                {"type", AccessoriesType.Weapon.ToString().ToLower()},
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static HttpContent ToHttpContent(this CharacterData target){
            if(target.IsNull())
                return null;
            
            var data = new Dictionary<string,object>(){
                {"id", target.id},
                {"owner", null},
                {"name", target.name},
                {"hp", target.stats.hp},
                {"attack", target.stats.attack},
                {"defense", target.stats.defense},
                {"speed", target.stats.speed},
                {"head_id", target.limbs.headID},
                {"body_id", target.limbs.bodyID},
                {"arm_id", target.limbs.armID},
                {"leg_id", target.limbs.legID},
                {"cloth_id", target.accessories.clothID},
                {"helmed_id", target.accessories.helmetID},
                {"facial_hair_id", target.accessories.facialHairID},
                {"pants_id", target.accessories.pantsID},
                {"sleeve_id", target.accessories.sleeveID},
                {"weapon_id", target.accessories.weaponID},
                {"active", target.status.active},
                {"fighting", false},
                {"energy", 0},
                {"max_engergy", 0},
                {"win_rate", target.status.winRate},
                {"battle_attempt", target.status.battleCount}
            };

            (var json,var ex) = data.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }  
    }
}