using System;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct ParticipantData
    {
        #region fields
        [DisplayAsString] 
        public string address;

        [DisplayAsString] 
        public string tokenId;
        #endregion

        #region private
        private string _str;
        #endregion

        #region constructor
        public ParticipantData(string address,string tokenId){
            this.address = address;
            this.tokenId = tokenId;
            this._str = $"{address}:{tokenId}";
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return _str;
        }
        #endregion
    }
}