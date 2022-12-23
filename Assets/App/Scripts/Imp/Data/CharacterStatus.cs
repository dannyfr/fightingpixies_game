using System;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct CharacterStatus
    {
        #region fields
        public bool active;
        public int energy;
        public int maxEnergy;
        public float winRate;
        public int battleCount;    
        #endregion
    }
}

