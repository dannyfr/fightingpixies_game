using System;
using System.Collections.Generic;
using Evesoft;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct CharacterLimbsCollections
    {
        #region fields
        public string[] heads;
        public string[] bodys;
        public string[] arms;
        public string[] legs;
        #endregion

        #region methods
        public bool IsNullOrEmpty(){
            var result = this.IsNull();
            result |= heads.IsNullOrEmpty();
            result |= bodys.IsNullOrEmpty();
            result |= arms.IsNullOrEmpty();
            result |= legs.IsNullOrEmpty();
            return result;
        }
        #endregion
    }
}

