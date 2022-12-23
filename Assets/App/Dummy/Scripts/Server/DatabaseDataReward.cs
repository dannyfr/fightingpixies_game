using System;
using Sirenix.OdinInspector;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataReward
    {
        #region fields
        [DisplayAsString] public string id;
        [DisplayAsString] public string address;
        [DisplayAsString] public CharacterData data;
        [DisplayAsString] public bool claimed;
        #endregion

        #region constructor
        public DatabaseDataReward(){}
        public DatabaseDataReward(string id,string address,CharacterData data,bool claimed = false){
            this.id = id;
            this.address = address;
            this.data = data;
            this.claimed = claimed;
        }
        #endregion
    }
}