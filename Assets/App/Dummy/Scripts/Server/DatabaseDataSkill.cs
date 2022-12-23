using System;
using Sirenix.OdinInspector;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataSkill
    {
        #region fields
        [DisplayAsString] public string id;
        [DisplayAsString] public int mindamage,maxdamage;
        [DisplayAsString] public float mincooldown,maxcooldown;
        #endregion

        #region constructor
        public DatabaseDataSkill(){}
        public DatabaseDataSkill(string id,int mindamage,int maxdamage,float mincooldown,float maxcooldown){
            this.id = id;
            this.mindamage = mindamage;
            this.maxdamage = maxdamage;
            this.mincooldown=mincooldown;
            this.maxcooldown=maxcooldown;
        }
        #endregion
    }
}