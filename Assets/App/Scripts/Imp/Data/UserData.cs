using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public class UserData
    {
        #region fields
        [SerializeField]
        private string _address,_token;
        #endregion

        #region property   
        public string address => _address;
        public string token => _address;
        #endregion

        #region constructor
        public UserData(string address,string token){
            _address = address;
            _token   = token;
        }
        #endregion
    }
}