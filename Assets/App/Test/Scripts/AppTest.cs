using System;
using System.Collections.Generic;
using Evesoft;
using Sirenix.OdinInspector;
using UnityEngine;
using NFTGame.WebAPI;
using NFTGame.Utils;
using System.Threading.Tasks;

namespace NFTGame.Test
{
    [HideMonoScript]
    public class AppTest : SerializedMonoBehaviour
    {
        [SerializeField]
        private string wallet;

        [SerializeField]
        private IBlockChainAPI blockChainAPI;

        [SerializeField]
        private Dummy.Server.ServerDummy server;

        [SerializeField]
        private Config.SmartContractConfig _battleSC;

        [SerializeField]
        private IWebAPI webAPI;

        [Button]
        private async void AddCharacters(){
            (var ex,var tokens) = await blockChainAPI.GetPixiesAsync(wallet);
            if(!ex.IsNull()){
                return;
            }

            if(tokens.IsNullOrEmpty())
                return;

            await webAPI.GetTokenAsync(wallet);

            for (var i = 0; i < tokens.Length; i++)
            {  
                tokens[i].Log();  
                // var character = server.GetRandomCharacter(1,true);
                // character.id = tokens[i].ToString();
                // await webAPI.AddCharacterAsync(character);
            }
        }

        public HistoryData[] _history;

        public BattleSimulationData _simulation;

        [Button]
        private async void TestGetHistory(){
            (var ex,var history) = await webAPI.GetHistoryAsync(wallet);
            if(!ex.IsNull())
                return;

            _history = history;
        }

        [Button]
        private async void TestGetCharactersName(){
            var ids = new string[]{"586","587"};
            (var ex,var names) = await webAPI.GetCharactersNameAsync(ids);
            if(!ex.IsNull())
                return;

            names.Join().Log();
        }
    
        [Button]
        private async void TestGetSimulation(string roomID){
            (var ex,var simulation) = await webAPI.GetBattleSimulationAsync(roomID);
            if(!ex.IsNull())
                return;

            _simulation = simulation;
        }
    
        [Button]
        private async void TestGetAvaibaleRoom(string roomId){
            (var ex,var available) = await blockChainAPI.GetBattleAvailable(uint.Parse(roomId));
            if(!ex.IsNull())
                return;

            available.Log();
        }   

        [Button]
        private async void TestGetBattleRoom(string tokenId){
            (var ex,var response) = await blockChainAPI.GetBattleRoomOf(uint.Parse(tokenId));
            response.Log();
        }

        [Button]
        private async void TestOwnerOf(string tokenId){
            (var ex, var response) = await blockChainAPI.OwnerOfAsync(uint.Parse(tokenId));
            response.Log();
        }

        [Button]
        private async void TestGetTokenInBattle(){
           (var ex, var response) = await blockChainAPI.GetInBattlePixiesAsync(wallet);
            response.Join().Log();
        }

        [Button]
        private async void TestGetPIxies(){
            (var ex,var ids) = await blockChainAPI.GetPixiesAsync(wallet);
        }
    
        [Button]
        private async void TestGetOpponent(){
            (var ex, var response) = await blockChainAPI.GetBattleAsync();
            response.Join().Log();
        }
    }
}