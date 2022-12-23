using System.Net;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Evesoft;
using NFTGame.Utils;
using UnityEngine;
using NFTGame.WebAPI.Data;
using Random = UnityEngine.Random;
using NFTGame.Dummy.Adapter;

namespace NFTGame.WebAPI
{
    [HideMonoScript]
    public class WebAPI : SerializedMonoBehaviour, IWebAPI,IDisposable
    {
        #region const
        const string grpSC = "Smart Contract";
        const string grpConfig = "Config";
        public const string baseUrl = "https://api-brute-game.2dverse.org";
        //public const string baseUrl = "https://fba2-103-147-9-89.ngrok.io";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpConfig),TextArea(2,5)]
        private string apikey  = "uk98IWfFbbG1XLAoTt3hGmqLm9WDCjuDrzPa7Q5n71U2aWmony8lNBYoC6Jgkyeqf6KuWVmlCIKmIapxTSQxtVW4jzd1bxlmF841";
        #endregion

        #region private
        private GameManager _gameManager;
        private Dictionary<string,string> _authorization;
        private BlockChain.BlockChainApi _blockChainAPI;
        #endregion

        #region methods
        private void ShowUIProcess(string message = null,bool hideUIProcess = false){
            if(hideUIProcess)
                return;

            _gameManager.uiManager.uiLoading.Show(message);
        }
        private void HideUIProcess(bool hideUIProcess = false){
            if(hideUIProcess)
                return;

            _gameManager.uiManager.uiLoading.Hide();
        }
        #endregion        
        
        #region IWebAPI
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _blockChainAPI = FindObjectOfType<BlockChain.BlockChainApi>();
        }
        public async Task<Exception> GetTokenAsync(string address,bool hideUIProcess = false)
        {   
            ShowUIProcess("GETTING TOKEN",hideUIProcess);
            await new WaitForSeconds(1);
            HideUIProcess();

            _authorization = new Dictionary<string, string>(){
                {"Authorization",$"Bearer {address}:{apikey}"},
            };
            return null;
        }
        public async Task<Exception> AddCharacterAsync(CharacterData data,bool hideUIProcess = false)
        { 
            var scope = $"{nameof(AddCharacterAsync)}({data})";
            this.LogRun(scope);

            #region check param
            if(data.IsNull())
                return new NullReferenceException(nameof(data)).LogException(this,scope);
            #endregion

            //Convert to json
            (var json,var ex) = new RawCharacter(){
                id = data.id,
                owner = null,
                showstat = data.showstat.ToString().ToLower(),
                name = data.name,
                story = data.story,
                hp = (int)data.stats.hp,
                attack = (int)data.stats.attack,
                defense = (int)data.stats.defense,
                speed = (int)data.stats.hp,
                head_id = data.limbs.headID,
                body_id = data.limbs.bodyID,
                arm_id = data.limbs.armID,
                leg_id = data.limbs.legID, 
                helmet_id = data.accessories.helmetID,
                facial_hair_id = data.accessories.facialHairID,
                cloth_id = data.accessories.clothID,
                sleeve_id = data.accessories.sleeveID, 
                pants_id = data.accessories.pantsID,
                weapon_id = data.accessories.weaponID,
                energy = data.status.energy,
                max_energy = data.status.maxEnergy,
                fighting = false,
                active = data.status.active,
                win_rate= data.status.winRate,
                battle_attempt = data.status.battleCount,
            }.ToJson();
            if(!ex.IsNull()){           
                return ex;
            }
            
            ShowUIProcess("UPLOAD PIXIES",hideUIProcess);

            //Request
            var url = $"{baseUrl}/pixies";
            var respone = await url.HttpRequestPost<WebAPIRespone>(json,_authorization);

            HideUIProcess();

            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope);
            
            this.LogCompleted(scope);
            return null;
        }
        public async Task<Exception> ClaimRewardAsync(string rewardId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(ClaimRewardAsync)}({rewardId})";
            this.LogRun(scope);

            #region check param
            if(rewardId.IsNullOrEmpty())
                return new NullReferenceException(nameof(rewardId)).LogException(this,scope);
            #endregion

            ShowUIProcess("CLAIM REWARD",hideUIProcess);
            
            //Request
            var url = $"{baseUrl}/reward/{rewardId}";
            var respone = await url.HttpRequestPut<WebAPIRespone>(rewardId,_authorization);

            HideUIProcess();
            
            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope);

            this.LogCompleted(scope);
            return null;
        }       
        public async Task<(Exception, HistoryData[])> GetHistoryAsync(string address,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetHistoryAsync)}({address})";
            this.LogRun(scope);

            #region check param
            if(address.IsNullOrEmpty())
                return (new NullReferenceException(nameof(address)).LogException(this,scope),null);
            #endregion

            ShowUIProcess("GET HISTORY",hideUIProcess);

            //Request
            var url = $"{baseUrl}/history/battle/{address}";
            var respone = await url.HttpRequestGet<WebAPIRespone<RawHistory[]>>();
            
            HideUIProcess(hideUIProcess);

            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return (new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope),default(HistoryData[]));

            var result = respone.result.ToData();
            if(!result.IsNullOrEmpty() && _blockChainAPI){
                var completed = 0;
                var onComplete= default(Action<bool,int>);
                var onFailed  = default(Action<Exception,int>);
                var getClaim  = default(Action<int>);
                
                onComplete = (claimed,index)=>{
                    completed++;
                    result[index].claimed = claimed;
                };
                onFailed  = (ex,index)=>
                {
                    getClaim.Invoke(index);
                };
                getClaim  = async (index)=>{
                    var roomId = default(uint);
                    uint.TryParse(result[index].room,out roomId);
                    (var ex,var claimed)  = await _blockChainAPI.GetClaim(roomId,hideUIProcess);
                    if(!ex.IsNull()){
                        onFailed?.Invoke(ex,index);
                    }else{
                        onComplete?.Invoke(claimed,index);
                    }
                };

                for (var i = 0; i < result.Length; i++)
                {
                    var index = i;
                    getClaim.Invoke(index);
                }
                
                await new WaitUntil(()=> completed == result.Length);
            }
            
            this.LogCompleted(scope);
            return (null,result);
        }   
        public async Task<(Exception, RewardData[])> GetRewardsAsync(string address,bool hideUIProcess = false)
        {
            //return GetRewardsAsyncEmpty(address);

            var scope = $"{nameof(GetRewardsAsync)}({address})";
            this.LogRun(scope);

            #region check param
            if(address.IsNullOrEmpty())
                return (new NullReferenceException(nameof(address)).LogException(this,scope),default(RewardData[]));
            #endregion

            ShowUIProcess("GETTING REWARDS",hideUIProcess);

            //Request
            var url     = $"{baseUrl}/reward";
            var respone = await url.HttpRequestGet<WebAPIRespone<RawReward[]>>(_authorization);
            
            HideUIProcess(hideUIProcess);

            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return (new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope),default(RewardData[]));
            
            //Parsing
            var result = default(RewardData[]);
            if(!respone.result.IsNullOrEmpty()){
                result = respone.result.ToData(_gameManager.characterManager.characterBuilder);
            }

            this.LogCompleted(scope);
            return (null,result);
        }
        public async Task<(Exception, RewardData[])> GetRewardsAsyncEmpty(string address,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetRewardsAsync)}({address})";
            this.LogRun(scope);

            #region check param
            if(address.IsNullOrEmpty())
                return (new NullReferenceException(nameof(address)).LogException(this,scope),null);
            #endregion

            ShowUIProcess("GETTING REWARDS",hideUIProcess);

            await new WaitForSeconds(1);

            HideUIProcess(hideUIProcess);

            this.LogCompleted(scope);
            return (null,default(RewardData[]));
        }   
        public async Task<(Exception, Character)> GetCharacterAsync(string pixiesId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetCharacterAsync)}({pixiesId})";
            this.LogRun(scope);

            #region check param
            if(pixiesId.IsNullOrEmpty())
                return (new NullReferenceException(nameof(pixiesId)).LogException(this,scope),null);
            #endregion

            ShowUIProcess("GETTING PIXIES",hideUIProcess);

            //Request
            var url = $"{baseUrl}/pixies/{pixiesId}";
            var respone = await url.HttpRequestGet<WebAPIRespone<RawCharacter>>();
            
            HideUIProcess(hideUIProcess);

            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return (new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope),default(Character));

            //parse respone
            var character = default(Character);
            if(!respone.result.IsNull()){
                var data  = respone.result.ToData();
                character = _gameManager.characterManager.characterBuilder.Create(data);
            }
        
            this.LogCompleted(scope);
            return (null,character);
        }
        public async Task<(Exception, Character[])> GetCharactersAsync(IList<string> pixiesIds,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetCharactersAsync)}({pixiesIds.Join()})";
            this.LogRun(scope);

            #region check param
            if(pixiesIds.IsNullOrEmpty()){
                this.LogCompleted(scope);
                return (null,default(Character[]));
            }
            #endregion

            //Convert to json
            (var json,var ex) = new Dictionary<string,object>(){
                {"ids", pixiesIds},
            }.ToJson();
            if(!ex.IsNull())
                return (ex,null);

            ShowUIProcess("GETTING PIXIES",hideUIProcess);

            //Request
            var url = $"{baseUrl}/pixies/list-id";
            var respone = await url.HttpRequestPost<WebAPIRespone<RawCharacter[]>>(json);
            
            HideUIProcess(hideUIProcess);

            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return (new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope),default(Character[]));
            
            //parse respone
            var character = default(Character[]);
            if(!respone.result.IsNullOrEmpty()){
                character = new Character[respone.result.Length];
                for (var i = 0; i < respone.result.Length; i++)
                {
                    var data = respone.result[i].ToData();
                    character[i] = _gameManager.characterManager.characterBuilder.Create(data);
                } 
            }
        
            this.LogCompleted(scope);
            return (null,character);
        }
        public async Task<(Exception, string[])> GetCharactersNameAsync(IList<string> pixiesIds,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetCharactersNameAsync)}({pixiesIds})";
            this.LogRun(scope);

            #region check param
            if(pixiesIds.IsNullOrEmpty()){
                this.LogCompleted(scope);
                return (null,default(string[]));
            }
            #endregion

            //Convert to json
            (var json,var ex) = new Dictionary<string,IList<string>>(){
                {"ids",pixiesIds}
            }.ToJson();
            if(!ex.IsNull()){
                return (ex,default(string[]));
            }

            ShowUIProcess("GETTING PIXIES NAMES",hideUIProcess);
            
            var url = $"{baseUrl}/pixies/list-name";
            var respone = await url.HttpRequestPost<WebAPIRespone<string[]>>(json);

            HideUIProcess(hideUIProcess);

            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return (new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope),default(string[]));

            var result = respone.result;
            this.LogCompleted(scope);
            return (null,result);
        }
        public async Task<(Exception, BattleSimulationData)> GetBattleSimulationAsync(string battleId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetBattleSimulationAsync)}({battleId})";
            this.LogRun(scope);

            #region check param
            if(battleId.IsNullOrEmpty())
                return (new NullReferenceException(nameof(battleId)).LogException(this,scope),null);
            #endregion

            //ShowUIProcess("GET SIMULATION",hideUIProcess);
            
            //Request
            var url = $"{baseUrl}/history/simulation/{battleId}";
            var respone = await url.HttpRequestGet<WebAPIRespone<RawBattleSimulation>>();
            
            //(hideUIProcess);
            
            //Check respone
            if(respone.IsNull() || respone.status != HttpStatusCode.OK)
                return (new HttpRequestException($"{respone.status} - {respone.message}").LogException(this,scope),default(BattleSimulationData));

            var result = respone.result.ToData();
            this.LogCompleted(scope);
            return (null,result);
        
        #endregion
    }

        #region IDisposable
        public void Dispose()
        {
            gameObject.Destroy();
        }
        #endregion
    }
}