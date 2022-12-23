using System;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct CharacterAccessories
    {
        #region const
        const string grpIndex = "Index";
        const string grpName = "Name";
        #endregion

        #region fields
       
        public string clothID;
        public string facialHairID;
        public string helmetID;
        public string pantsID;
        public string sleeveID;
        public string weaponID;
        #endregion
    }
}

