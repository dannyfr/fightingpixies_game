using System;
using Evesoft;
using NFTGame.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(RewardManager))]
    public class RewardManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpComponent = "Component";
        const string grpConfigs = "Configs";
        const string grpRuntime = "Runtime";
        #endregion

        #region events
        public event Action<RewardManager> onGetRewardCompleted;
        public event Action<RewardManager,Exception> onGetRewardFailed;
        public event Action<RewardManager,int> onClaimCompleted;
        public event Action<RewardManager,Exception,int> onClaimFailed;
        #endregion

        #region fields
        [BoxGroup(grpComponent),SerializeField]
        private Transform _parent;
        #endregion
        
        #region property
        [ShowInInspector,BoxGroup(nameof(availableRewards)),HideLabel,ListDrawerSettings(Expanded = true)]
        public RewardData[] availableRewards => _availableRewards;
        #endregion

        #region private
        private GameManager _gameManager;
        private bool _isGettingReward;
        private bool _isClaiming;
        private RewardData[] _availableRewards;
        private uint[] _mintedTokenId;
        private bool[] _pushedPixies;
        private bool[] _claimedRewards;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            this.LogCompleted(nameof(Init));  
        }     
        public void Clear(){
            _availableRewards?.Dispose();
            _availableRewards = null;
            _mintedTokenId    = null;
            _claimedRewards   = null;
            _pushedPixies     = null; 
        }
        #endregion

        #region async
        public async void GetRewardAsync(string address){
            if(_isGettingReward)
                return;

            //Reset
            Clear();
            _isGettingReward  = true;

            //GetRewardData
            var ex = default(Exception);
            (ex,_availableRewards) = await _gameManager.webApi.GetRewardsAsync(address);
            _isGettingReward = false;
            if(!ex.IsNull()){
                onGetRewardFailed?.Invoke(this,ex);
            }else{
                if(!_availableRewards.IsNullOrEmpty()){
                    _mintedTokenId  = new uint[_availableRewards.Length];
                    _pushedPixies   = new bool[_availableRewards.Length];
                    _claimedRewards = new bool[_availableRewards.Length];

                    for (var i = 0; i < _availableRewards.Length; i++)
                    {
                        _availableRewards[i].character?.Hide();
                        _availableRewards[i].character?.transform.SetParent(_parent);
                    }
                }
                onGetRewardCompleted?.Invoke(this);
            }
        }    
        public async void ClaimRewardAsync(string address,int index){
            if(_isClaiming)
                return;
            
            if(_availableRewards.IsNullOrEmpty())
                return;

            if(address.IsNullOrEmpty())
                return;

            if(index < 0 || index >= _availableRewards.Length)
                return;

            if(_availableRewards[index].IsNull() || _availableRewards[index].character.data.IsNull())
                return;


            _isClaiming = true;
            
            //Minting process
            if(_mintedTokenId[index] == default(uint)){
                var character = _availableRewards[index].character;
                (var ex,var tokenId) = await _gameManager.blockChainApi.SetMintAsync(address,character);
                if(!ex.IsNull()){
                    _isClaiming = false;
                    onClaimFailed?.Invoke(this,ex,index);
                    return;
                }

                _mintedTokenId[index] = tokenId;
            }
                      
            //Upload to database
            if(!_pushedPixies[index]){
                _availableRewards[index].character.data.id = _mintedTokenId[index].ToString();
                var ex = await _gameManager.characterManager.PushCharacterAsync(_availableRewards[index].character.data); 
                _pushedPixies[index] = ex.IsNull();
                if(!ex.IsNull()){
                    _isClaiming = false;
                    onClaimFailed?.Invoke(this,ex,index);
                    return;
                }
            }

            //SetClaimed rewards
            if(!_claimedRewards[index]){
                var ex = await _gameManager.webApi.ClaimRewardAsync(_availableRewards[index].id);
                _claimedRewards[index] = ex.IsNull();
                if(!ex.IsNull()){
                    _isClaiming = false;
                    onClaimFailed?.Invoke(this,ex,index);
                    return;
                }
            }

            _isClaiming = false;    
            onClaimCompleted?.Invoke(this,index);
        }
        #endregion

        #region callbacks
        private void OnDestroy() {
            onGetRewardCompleted = null;
            onGetRewardFailed    = null;
            onClaimCompleted     = null;
            onClaimFailed        = null;
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