using System;
using Evesoft;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public struct RawMintData
    {
        #region fields
        public string name;
        public string description;
        public string image;
        public string external_url;
        public RawMintAttribute[] attributes;
        #endregion

        #region constructor
        public RawMintData(string name,string description,string image,string externalUrl,RawMintAttribute[] attributes){
            this.name = name;
            this.description = description;
            this.image = image;
            this.external_url = externalUrl;
            this.attributes = attributes;
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return $"name:{name}\ndescription:{description}\nimage:{image}\nexternal_url:{external_url}\nattributes:{attributes?.Join()}";
        }
        #endregion
    }
}