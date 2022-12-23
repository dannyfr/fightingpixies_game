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
    public class TestJson : SerializedMonoBehaviour
    {
        [SerializeField,TextArea(5,20)]
        private string json;

        [NonSerialized,ShowInInspector]
        public WebAPIRespone<RawBattleSimulation> _data;
        
        [Button]
        public void ParsingJson(){
           (var data,var ex) = json.FromJsonUnity<WebAPIRespone<RawBattleSimulation>>();
            _data = data;
        }

        
    }
}

