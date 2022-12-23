
using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Evesoft;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;
using NFTGame.Utils;
using NFTGame.Dummy.Adapter;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Dummy + nameof(WebApiDummy))]
    public class WebApiDummy : SerializedMonoBehaviour,IWebAPI,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpRequired= "Required";
        #endregion

        #region fields
        [BoxGroup(grpConfig),SerializeField,Range(0,100f)]
        private float _requestFailPercentage = 10;

        [BoxGroup(grpConfig),SerializeField,MinMaxSlider(0.001f,5f,true)]
        private Vector2 _requestTimeRange = new Vector2(1,2);

        [BoxGroup(grpRequired),SerializeField]
        private Dummy.Server.ServerDummy _server;
        #endregion

        #region private
        private GameManager _gameManager;
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
            _server.Init();
        }
        public async Task<Exception> GetTokenAsync(string address,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetTokenAsync)}({address})";
            this.LogRun(scope);

            #region check param
            if(address.IsNullOrEmpty())
                return new NullReferenceException(nameof(address)).LogException(this,scope);
            #endregion

            ShowUIProcess("GETTING TOKEN",hideUIProcess);
        
            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
        
            HideUIProcess();
            
            if(fail){
                return new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope);
            }else{
                //_token = address.GetHashCode().ToString();
                this.LogCompleted(scope);
                return null;
            }
        }
        public async Task<Exception> AddCharacterAsync(CharacterData data,bool hideUIProcess = false)
        {
            var scope = $"{nameof(AddCharacterAsync)}({data})";
            this.LogRun(scope);

            #region check param
            if(data.IsNull())
                return new NullReferenceException(nameof(data)).LogException(this,scope);
            #endregion

            ShowUIProcess("UPLOAD PIXIES",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess();

            if(fail){
                return new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope);
            }else{
                _server.AddCharacter(data.ToDataDatabase());
                this.LogCompleted(scope);
                return null;
            }
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

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess();
            
            if(fail){
                return new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope);
            }else{
                _server.ClaimReward(rewardId);
                this.LogCompleted(scope);
                return null;
            }
        }     
        public async Task<(Exception, HistoryData[])> GetHistoryAsync(string address,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetHistoryAsync)}({address})";
            this.LogRun(scope);

            #region check param
            if( address.IsNullOrEmpty())
                return (new NullReferenceException(nameof(address)).LogException(this,scope),default(HistoryData[]));
            #endregion

            ShowUIProcess("GET HISTORY",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));

            HideUIProcess(hideUIProcess);

            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(HistoryData[]));
            
            var result = _server.GetHistory(address)?.ToData();
            this.LogCompleted(scope);
            return (null,result);
        }
        public async Task<(Exception, RewardData[])> GetRewardsAsync(string address,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetRewardsAsync)}({address})";
            this.LogRun(scope);

            #region check param
            if(address.IsNullOrEmpty())
                return (new NullReferenceException(nameof(address)).LogException(this,scope),null);
            #endregion

            ShowUIProcess("GETTING REWARDS",hideUIProcess);
            
            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail){       
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),null);
            }else{
                var rewards = _server.GetRewards(address);
                var result  = rewards.IsNullOrEmpty()? null : new RewardData[rewards.Length];
                
                if(!result.IsNullOrEmpty()){
                    for (var i = 0; i < result.Length; i++)
                    {
                        result[i] = new RewardData(){
                            id   = rewards[i].id,
                            character = _gameManager.characterManager.characterBuilder.Create(rewards[i].data)
                        };
                    }
                }
            
                this.LogCompleted(scope);
                return (null,result);
            }
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

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),null);

            var data   = _server.GetCharacters(new string[]{pixiesId});
            if(data.IsNullOrEmpty())
                return(new KeyNotFoundException(nameof(pixiesId)).LogException(this,scope),null);

            var result = _gameManager.characterManager.characterBuilder.Create(data[0]);
                result?.Hide();

            this.LogCompleted(scope);
            return (null,result);
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

            ShowUIProcess("GETTING PIXIES",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),null);

            var data = _server.GetCharacters(pixiesIds);
            var result = default(Character[]);
            if(!data.IsNullOrEmpty()){
                result = new Character[data.Length];
                for (var i = 0; i < data.Length; i++)
                {
                    result[i] = _gameManager.characterManager.characterBuilder.Create(data[i]);
                    result[i]?.Hide();
                }
            }

            this.LogCompleted(scope);
            return (null,result);
        }
        public async Task<(Exception, string[])> GetCharactersNameAsync(IList<string> pixiesIds,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetCharactersNameAsync)}({pixiesIds.Join()})";
            this.LogRun(scope);

            #region check param
            if(pixiesIds.IsNullOrEmpty()){
                this.LogCompleted(scope);
                return (null,default(string[]));
            }
            #endregion

            ShowUIProcess("GETTING PIXIES NAMES",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),null);
            
            var result = _server.GetCharactersName(pixiesIds);
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

            ShowUIProcess("GET SIMULATION",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));

            HideUIProcess(hideUIProcess);

            if(fail){
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),null);
            }else{
                var data = _server.GetBattleSimulation(battleId).ToData();
                if(data.IsNull())
                    return (new NullReferenceException("Result is Empty").LogException(this,scope),null);

                this.LogCompleted(scope);
                return (null,data);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            gameObject.Destroy();
        }
        #endregion
    }
}

