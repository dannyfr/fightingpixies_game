using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using NFTGame.WebAPI;
using NFTGame.WebAPI.Data;
using NFTGame.BlockChain;

namespace NFTGame.Test
{   
    [HideMonoScript]
    public class TestUploadIPFS : SerializedMonoBehaviour
    {   
        [SerializeField]
        private BlockChainApi _blockChainApi;

    
        [Button]
        private async void Upload(Character character){
            await _blockChainApi.SetMintAsync(_blockChainApi.account,character);
        }

        [Button]
        private void Test(){
            var address= "0x6656e037F3281b95EC0E2fBBc68F809431eeb1dA";
            var tokenId = 20;
            var v = 10;
            var r = 30;
            var s = 40;
            var hass= "opjkasliawdad";
            var fee = $"[{{recipient: \'{address}\', value : 1000}}]";
            var data = $"[\"{tokenId}\",\"{v}\",\"{r}\",\"{s}\",\"{fee}\",\"{hass}\"]";
            data.Log();
        }
    }
}

