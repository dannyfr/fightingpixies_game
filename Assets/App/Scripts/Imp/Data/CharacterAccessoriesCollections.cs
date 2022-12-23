using System;
using System.Collections.Generic;
using Evesoft;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct CharacterAccessoriesCollections
    {
        #region fields
        public string[] cloths;
        public string[] facialHairs;
        public string[] helmets;
        public string[] pants;
        public string[] sleeves;
        public string[] weapons;
        #endregion

        #region methods
        public bool IsNullOrEmpty(){
            var result = this.IsNull();
            result |= cloths.IsNullOrEmpty();
            result |= facialHairs.IsNullOrEmpty();
            result |= helmets.IsNullOrEmpty();
            result |= pants.IsNullOrEmpty();
            result |= sleeves.IsNullOrEmpty();
            result |= weapons.IsNullOrEmpty();
            return result;
        }
        #endregion
    }
}

