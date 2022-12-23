using System.Collections.Generic;
using Evesoft;

namespace NFTGame.Dummy.Adapter
{
    public static class AdapterSkill
    {
        public static  Server.DatabaseDataSkill ToDataDatabase(this ISkill data){
            return new  Server.DatabaseDataSkill(){
                id = data.id,
                mindamage = data.minDamage,
                maxdamage = data.maxDamage,
                mincooldown = data.minCoolDown,
                maxcooldown = data.maxCoolDown
            };
        }
        public static Server.DatabaseDataSkill[] ToDataDatabase(this IList<ISkill> data){
            if(data.IsNullOrEmpty())
                return default(Server.DatabaseDataSkill[]);

            var result = new Server.DatabaseDataSkill[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToDataDatabase();
            }
            return result;
        }   
    }
}