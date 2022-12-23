using System;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public struct RawMintAttribute
    {
        #region fields
        public string key;
        public string trait_type;
        public string value;
        #endregion

        #region contructor
        public RawMintAttribute(string key,string trait,string value){
            this.key = key;
            this.trait_type = trait;
            this.value = value;
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return $"key:{key}\ntrait_type:{trait_type}\nvalue:{value}";
        }
        #endregion
    }
}