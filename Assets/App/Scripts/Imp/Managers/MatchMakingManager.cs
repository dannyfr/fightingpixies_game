using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using System;
using NFTGame.Utils;
using NFTGame.Config;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(MatchMakingManager))]
    public class MatchMakingManager : SerializedMonoBehaviour,IDisposable
    {
        public struct MathmakingResult
        {
            #region fields
            public uint battleID;
            public Character opponent;
            #endregion

            #region constructor
            public MathmakingResult(uint battleID,Character opponent){
                this.battleID = battleID;
                this.opponent = opponent;
            }
            #endregion
        }

        #region const
        const string grpConfig = "Config";
        const string grpRuntime = "Runtime";
        const string grpComponent = "Component";
        #endregion

        #region fields
        [BoxGroup(grpComponent),SerializeField,Required]
        private Transform _parent;
        #endregion

        #region events
        public event Action<MatchMakingManager,MathmakingResult> onFindOpponentCompleted;
        public event Action<MatchMakingManager,Exception> onFindOpponentFailed;
        public event Action<MatchMakingManager> onBattleCompleted;
        public event Action<MatchMakingManager> onBattleUnavailable;
        public event Action<MatchMakingManager,Exception> onBattleFailed;
        #endregion

        #region property
        [ShowInInspector,BoxGroup(nameof(battleID)),HideLabel]
        public uint battleID => _battleID;

        [ShowInInspector]
        public Stats stats => _stats;

        [ShowInInspector,BoxGroup(nameof(opponent)),HideLabel]
        public Character opponent => _opponent;
        #endregion

        #region private
        private GameManager _gameManager;
        private Character _opponent,_tmpOpponent;
        private uint _battleID;
        private bool _isMatchmaking;
        private bool _isBattle;
        private Stats _stats;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            this.LogCompleted(nameof(Init));
        } 
        public Stats GetRandomStat()
        {
            var values = Enum.GetValues(typeof(Stats));
            _stats = (Stats)values.GetValue(UnityEngine.Random.Range(0,values.Length));
            return _stats;
        }
        #endregion

        #region async
        public async void FindOpponent(string attackerId){
            if(_isMatchmaking || attackerId.IsNullOrEmpty())
                return;

            _isMatchmaking = true;
    
            //Get Battle
            (var ex,var battle) = await _gameManager.blockChainApi.GetBattleAsync();
            if(!ex.IsNull()){
                _isMatchmaking = false;
                onFindOpponentFailed?.Invoke(this,ex);
                return;
            }

            _battleID   = battle[0];
            var tokenID = battle[1];
    
            //Check empty battle
            if(_battleID == default(uint) || tokenID == default(uint)){
                _tmpOpponent = _opponent;
                _opponent    = null;
                _isMatchmaking = false;          
                onFindOpponentFailed?.Invoke(this,new Exception("No match found"));
                return;
            }else{
                if(opponent.IsNull() && !_tmpOpponent.IsNull()){
                    _opponent = _tmpOpponent;
                    _tmpOpponent = null;
                }
            }

            if(_opponent.IsNull() || tokenID.ToString() != _opponent.data.id){
                _opponent?.Dispose();
                _tmpOpponent?.Dispose();
                _opponent    = null;
                _tmpOpponent = null;

                //Get character data from server
                (ex,_opponent) = await _gameManager.webApi.GetCharacterAsync(tokenID.ToString());    
                if(!ex.IsNull()){
                    _isMatchmaking = false;        
                    onFindOpponentFailed?.Invoke(this,ex);
                    return;
                }
            }

            //Create character
            _isMatchmaking = false;
            _opponent?.Hide();
            _opponent?.transform.SetParent(_parent);
            onFindOpponentCompleted?.Invoke(this,new MathmakingResult(_battleID,_opponent));
        }  
        public async void Battle(uint battleId,string pixiesId){
            if(_opponent.IsNull() || _isBattle)
                return;
            
            //Check room still available
            _isBattle = true;
            (var ex,var available) = await _gameManager.blockChainApi.GetBattleAvailable(_battleID);
            if(!ex.IsNull() || !available){
                _isBattle = false;
                onBattleUnavailable?.Invoke(this);
                return;
            }

            //Battle
            ex = await _gameManager.blockChainApi.SetBattleAsync(uint.Parse(pixiesId),_battleID);
            if(!ex.IsNull()){
                _isBattle = false;
                onBattleFailed?.Invoke(this,ex);
                return;
            }
            
            _isBattle = false;
            onBattleCompleted?.Invoke(this);
        }
        #endregion

        #region callbacks
        private void OnDestroy(){
            onFindOpponentCompleted = null;
            onFindOpponentFailed = null;
            onBattleCompleted = null;
            onBattleFailed = null;
            onBattleUnavailable = null;
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

