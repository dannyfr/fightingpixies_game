using System.Collections;
using System.Collections.Generic;
using Evesoft;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace NFTGame.Test
{
    [HideMonoScript]
    public class Web3Test : SerializedMonoBehaviour
    {
        [SerializeField]
        private string chain = "ethereum";

        [SerializeField]
        private string network = "rinkeby";

        [SerializeField]
        private string battleSC;

        [SerializeField,TextArea(5,26)]
        private string battleABI;

        [Button]
        public async void Test(uint roomId){
            try {
                var response = await EVM.Call(chain,network,battleSC,battleABI,"battleRoom",$"[\"{roomId}\"]");
                response.Log();
            } catch (Exception ex) {
                ex.Message.LogError();
            };
        }
    }
}


