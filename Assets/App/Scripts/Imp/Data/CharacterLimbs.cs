using System;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct CharacterLimbs
    {
        #region const
        const string grpIndex = "Index";
        const string grpName = "Name";
        #endregion

        #region fields
       
        public string headID;
        public string bodyID;
        public string armID;
        public string legID;
        #endregion

        // #region constructor
        // public CharacterLimbs(string headID,string bodyID,string armID,string legID){
        //     this.headID = headID;
        //     this.bodyID = bodyID;
        //     this.armID = armID;
        //     this.legID = legID;
        // }
        // #endregion
    }
}

