using System;
using Sirenix.OdinInspector;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public struct DatabaseDataDeposit
    {
        #region fields
        [DisplayAsString] public string address;
        [DisplayAsString] public bool deposit;
        #endregion
    }
}