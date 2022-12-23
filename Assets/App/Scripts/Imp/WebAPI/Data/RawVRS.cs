using System;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public class RawVRS
    {
        #region fields
        public string v;
        public string r;
        public string s;
        public uint tokenId;
        #endregion

        #region constructor
        public RawVRS(string v,string r,string s, uint tokenId){
            this.v = v;
            this.r = r;
            this.tokenId = tokenId;
        }
        #endregion
    }
}