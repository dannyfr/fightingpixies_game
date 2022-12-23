using System;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public class RawIPFSData
    {
        #region fields
        public string Name;
        public string Hash;
        public string Size;
        #endregion

        #region methods
        public override string ToString()
        {
            return $"name:{Name}\nHash:{Hash}\nSize:{Size}";
        }
        #endregion
    }
}