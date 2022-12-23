using System;
using Sirenix.OdinInspector;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataPart
    {
        #region fields
        [DisplayAsString] public string id;
        [DisplayAsString] public string type;
        #endregion

        #region constructor
        public DatabaseDataPart(){}
        public DatabaseDataPart(string id,string type){
            this.id = id;
            this.type = type;
        }
        #endregion
    }
}