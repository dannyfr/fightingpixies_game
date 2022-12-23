using System;
using UnityEngine;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public class RawSmartContract
    {   
        [TextArea(2,6)]
        public string address;

        [TextArea(2,20)]
        public string abi;
        public string rpc;
    }
}