using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evesoft;
using NFTGame.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(WebAPIManager))]
    public class WebAPIManager : SerializedMonoBehaviour,IDisposable,IWebAPI
    {
        #region const
        const string grpConfig = "Config";
        const string grpReq = "Required";
        #endregion

        #region fields
        [BoxGroup(grpReq),SerializeField]
        private Dictionary<Environment,IWebAPI> _api;
        #endregion

        #region private
        private Environment _env;
        #endregion

        #region IWebAPI
        public void Init(GameManager gameManager)
        {
            _env = gameManager.environment;
            foreach (var item in _api){
                item.Value.Init(gameManager);
            }
        }    
        public Task<Exception> GetTokenAsync(string address,bool hideUIProcess = false)
        {
            return _api[_env].GetTokenAsync(address,hideUIProcess);
        }   
        public Task<Exception> AddCharacterAsync(CharacterData data,bool hideUIProcess = false)
        {
            return _api[_env].AddCharacterAsync(data,hideUIProcess);
        }
        public Task<Exception> ClaimRewardAsync(string rewardId,bool hideUIProcess = false)
        {
            return _api[_env].ClaimRewardAsync(rewardId,hideUIProcess);
        }     
        public Task<(Exception, HistoryData[])> GetHistoryAsync(string address,bool hideUIProcess = false)
        {
            return _api[_env].GetHistoryAsync(address,hideUIProcess);
        }
        public Task<(Exception, RewardData[])> GetRewardsAsync(string address,bool hideUIProcess = false)
        {
            return _api[_env].GetRewardsAsync(address,hideUIProcess);
        }
        public Task<(Exception, Character)> GetCharacterAsync(string pixiesId,bool hideUIProcess = false)
        {
            return _api[_env].GetCharacterAsync(pixiesId,hideUIProcess);
        }
        public Task<(Exception, Character[])> GetCharactersAsync(IList<string> pixiesIds,bool hideUIProcess = false)
        {
            return _api[_env].GetCharactersAsync(pixiesIds,hideUIProcess);
        }       
        public Task<(Exception, string[])> GetCharactersNameAsync(IList<string> pixiesIds,bool hideUIProcess = false)
        {
            return _api[_env].GetCharactersNameAsync(pixiesIds,hideUIProcess);
        }
        public Task<(Exception, BattleSimulationData)> GetBattleSimulationAsync(string battleId,bool hideUIProcess = false)
        {
            return _api[_env].GetBattleSimulationAsync(battleId,hideUIProcess);
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