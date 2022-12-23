using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataParticipant
    {
        #region fields
        [DisplayAsString,HorizontalGroup] 
        public string address;

        [DisplayAsString,HorizontalGroup] 
        public string tokenId;
        #endregion

        #region private
        private string _str;
        #endregion

        #region constructor
        public DatabaseDataParticipant(){}
        public DatabaseDataParticipant(string address,string tokenId){
            this.address = address;
            this.tokenId = tokenId;
            this._str = $"{address}:{tokenId}";
        }
        #endregion

        #region methods
        public override string ToString()
        {
           return this._str;
        }
        #endregion
    }
}