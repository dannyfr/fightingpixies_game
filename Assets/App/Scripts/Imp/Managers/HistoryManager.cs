using System;
using System.Collections.Generic;
using Evesoft;
using Sirenix.OdinInspector;
using UnityEngine;
using DarkTonic.MasterAudio;
using NFTGame.Utils;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(HistoryManager))]
    public class HistoryManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpComponent = "Component";
        const string grpConfigs = "Configs";
        const string grpRuntime = "Runtime";
        #endregion

        [SerializeField,BoxGroup(grpComponent)]
        private Transform _parent;

        #region events
        public event Action<HistoryManager> onPullHistoryonCompleted;
        public event Action<HistoryManager,Exception> onPullHistoryonFailed;
        public event Action<HistoryManager,string>  onClaimResultCompleted;
        public event Action<HistoryManager,string,Exception> onClaimResultFailed;
        #endregion

        #region property
        [ShowInInspector,BoxGroup(grpRuntime),TableList]
        public HistoryData[] history => _history;
        #endregion

        #region private
        private GameManager _gameManager;
        private HistoryData[] _history;
        private bool _isGettingHistory;
        private bool _isClaiming;
        #endregion

        #region private
        [SerializeField]
        private string[] _availableClaimBattleIds;
        private Dictionary<string,Character> _availableClaimCharacters;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            _availableClaimCharacters = new Dictionary<string, Character>();
            this.LogCompleted(nameof(Init));
        }    
        public string[] GetAvailableClaim(){
           return _availableClaimBattleIds;
        }
        public Character GetRewardByBattleId(string battleId){
            if(battleId.IsNullOrEmpty())
                return null;

            if(!_availableClaimCharacters.ContainsKey(battleId))
                return null;

            return _availableClaimCharacters[battleId];
        }
        #endregion

        #region async
        public async void PullHistory(){
            if(_isGettingHistory)
                return;

            //Clear
            _availableClaimBattleIds = null;
            if(!_availableClaimCharacters.IsNullOrEmpty()){
                foreach (var item in _availableClaimCharacters)
                {
                    item.Value?.Dispose();
                }
            }
            _availableClaimCharacters?.Dispose();
            _availableClaimCharacters?.Clear();

            //Pull Battle History
            _isGettingHistory = true;
            var ex = default(Exception);
            var address = _gameManager.blockChainApi.account;
            (ex,_history) = await _gameManager.webApi.GetHistoryAsync(address,true);
            if(!ex.IsNull()){
                _isGettingHistory = false;
                onPullHistoryonFailed?.Invoke(this,ex);
                return;
            }

            if(!_history.IsNullOrEmpty()){
                //Sort bydate
                Array.Sort(_history,(x,y)=> y.date.CompareTo(x.date));

                //Check if any reward claim
                var battleIds = new List<string>();
                var rewardIds = new List<string>();
                var account   = _gameManager.blockChainApi.account;
                for (var i = 0; i < _history.Length; i++)
                {
                    var canClaim = _history[i].IsWon(account) && !_history[i].claimed;
                    if(canClaim){
                        var battleId = _history[i].room;
                        var reward   = _history[i].GetReward(address);
                        battleIds.Add(battleId);
                        rewardIds.Add(reward);
                        //_availableClaimCharacters[battleId] = null;
                    }
                }

                //GetCharacters
                if(!rewardIds.IsNullOrEmpty()){
                    var characters  = default(Character[]);
                    (ex,characters) =  await _gameManager.webApi.GetCharactersAsync(rewardIds);
                    if(!ex.IsNull()){
                        _isGettingHistory = false;
                        onPullHistoryonFailed?.Invoke(this,ex);
                        return;
                    }

                    if(!characters.IsNullOrEmpty()){
                        for (var i = 0; i < characters.Length; i++)
                        {
                            if(characters[i].IsNull())
                                continue;

                            characters[i].Hide();
                            characters[i].transform.SetParent(_parent);

                            var index = rewardIds.IndexOf(characters[i].data.id);
                            if(index < 0)
                                continue;

                            _availableClaimCharacters[battleIds[index]] = characters[i];
                        }
                    }
                }

                _availableClaimBattleIds = battleIds.ToArray();
                battleIds?.Clear();
                rewardIds?.Clear();
                battleIds = null;
                rewardIds = null;
            }

            _isGettingHistory = false;
            onPullHistoryonCompleted?.Invoke(this);
        }
        public async void Claim(string battleId){
            if(battleId.IsNullOrEmpty() || _isClaiming)
                return;

            _isClaiming = true;

            var ex = await _gameManager.blockChainApi.SetClaim(uint.Parse(battleId));
            if(!ex.IsNull()){
                _isClaiming = false;
                onClaimResultFailed?.Invoke(this,battleId,ex);
                return;
            }

            _isClaiming = false;
            onClaimResultCompleted?.Invoke(this,battleId);
        }
        #endregion

        #region callbacks
        private void OnDestroy() {
            onPullHistoryonCompleted = null;
            onPullHistoryonFailed = null; 
            onClaimResultCompleted = null;
            onClaimResultFailed = null;
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