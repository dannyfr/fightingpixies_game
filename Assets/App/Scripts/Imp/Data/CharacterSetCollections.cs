using System;
using System.Collections.Generic;
using Evesoft;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct CharacterSetCollections
    {
        #region fields
        public string[] heads;
        public string[] bodys;
        public string[] legs;
        #endregion

        #region methods
        public bool IsNullOrEmpty(){
            var result = this.IsNull();
            result |= heads.IsNullOrEmpty();
            result |= bodys.IsNullOrEmpty();
            result |= legs.IsNullOrEmpty();
            return result;
        }
        #endregion
    }
}

