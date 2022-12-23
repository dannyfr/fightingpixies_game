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
    public class TestUniqAPI : SerializedMonoBehaviour
    {
        [SerializeField,TextArea(2,10)]
        private string wallet;

        [SerializeField]
        private uint[] collections;

        [Button]
        private async void GetCollections(){
            var prod   = false;

            //GetCollection
            var prodUrl = $"https://apixdai2.unique.one/getUserCollectedCollectibleList?userPublicAddress={wallet}&type=collections"; 
            var devUrl  = $"https://apidev.unique.one/getUserCollectedCollectibleList?userPublicAddress={wallet}&type=collections";
            var url     = prod ? prodUrl : devUrl;
            var collectionResult = await url.HttpRequestGet<WebAPIUniqRespone<UniqDataCollectionResult>>();
            if(collectionResult.IsNull()){
                return;
            }
                
            //Get Collection OnSale
            prodUrl = $"https://apixdai2.unique.one/getUserOnsaleCollectibleList?userPublicAddress={wallet}&type=onsale";
            devUrl  = $"https://apidev.unique.one/getUserOnsaleCollectibleList?userPublicAddress={wallet}&type=onsale";
            url     = prod ? prodUrl : devUrl;
            var onSaleResult = await url.HttpRequestGet<WebAPIUniqRespone<UniqDataOnSaleResult>>();
            if(onSaleResult.IsNull()){
                return;
            }

            var onSale    = onSaleResult.tokenIds;
            var available = new List<uint>(collectionResult.tokenIds);
                available.RemoveAll(x=> Array.IndexOf(onSale,x) >= 0);
            this.collections = available.ToArray(); 
        }

        // [Button]
        // private async void GetCollectionsOnSale(){
            
        // }

        [SerializeField,TextArea(5,20)]
        public string jsonRespone;

        public WebAPIUniqRespone<UniqDataCollectionResult> respone;

        [Button]
        public void ParsingJson(){
            var ex = default(Exception);
            (respone, ex) = jsonRespone.FromJsonUnity<WebAPIUniqRespone<UniqDataCollectionResult>>();
        }
    }
}

