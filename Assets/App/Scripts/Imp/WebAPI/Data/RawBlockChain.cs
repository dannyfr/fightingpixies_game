using System;
using UnityEngine;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public class RawBlockChain
    { 
        public string chain;
        public string network;
        public RawSmartContract uniqSC;
        public RawSmartContract battleSC;
    }
}