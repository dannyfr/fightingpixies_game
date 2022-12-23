using System;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct RewardData : IDisposable
    {
        #region fields
        [DisplayAsString] public string id;
        [DisplayAsString] public Character character;
        #endregion

        #region IDisposable
        public void Dispose()
        {
            id = null;
            character.Dispose();
        }
        #endregion
    }
}