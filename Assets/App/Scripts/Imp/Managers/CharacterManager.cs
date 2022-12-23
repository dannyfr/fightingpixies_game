using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using System;
using System.Threading.Tasks;
using NFTGame.Utils;
using System.Collections.Generic;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(CharacterManager))]
    public class CharacterManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field 
        [BoxGroup(grpComponent),SerializeField,Required]
        private Transform _parent;

        [BoxGroup(grpComponent),SerializeField,Required]
        private Component.CharacterBuilder _characterBuilder;
        #endregion

        #region events
        public event Action<CharacterManager,Character> onSelectedCharacterChange;
        public event Action<CharacterManager> onPullCharactersCompleted;
        public event Action<CharacterManager,Exception> onPullCharactersFailed;
        #endregion

        #region property
        [ShowInInspector]
        public bool isPullingCharacter => _isPulling;
        public Component.CharacterBuilder characterBuilder => _characterBuilder;

        [ShowInInspector,BoxGroup(nameof(mainCharacter)),HideLabel,ReadOnly]
        public Character mainCharacter => _characters.IsNullOrEmpty() ? null : _characters[_mainCharacterIndex]; 

        [ShowInInspector,BoxGroup(nameof(characters)),ListDrawerSettings(Expanded = true)]
        public Character[] characters => _characters;
        #endregion

        #region private
        private GameManager _gameManager;
        private List<Character> _cacheCharacters;
        private Character[] _characters;
        private uint balanceToken;
        private uint balanceInbattle;
        private int _mainCharacterIndex;
        private int _pullProgress;
        private int _pullCompleted;
        private int _pullTotal;
        private bool _isPulling;
        
        [ShowInInspector]
        private bool isPullSuccess{
            get{
                return  _pullCompleted == _pullTotal;
            }
        }
        
        [ShowInInspector]
        private bool isPulling{
            get{
                return _isPulling || _pullProgress < _pullTotal;
            }
        }
        #endregion

        #region methods
        public void Init(GameManager gameManager){   
            _gameManager = gameManager;
            _characterBuilder.Init(gameManager);
            _cacheCharacters = new List<Character>();
            this.LogCompleted(nameof(Init));
            onPullCharactersFailed += OnPullCharactersFailed;
        }
        public void SetMainCharacter(int index){
            if(index > _characters.Length - 1 || index == _mainCharacterIndex)
                return;

            _mainCharacterIndex = index;
            onSelectedCharacterChange?.Invoke(this,mainCharacter);
        }
        public int GetMainCharacterIndex(){
            return _mainCharacterIndex;
        }
        public bool IsCharacterAvailable(int index){
            if(index >= _characters.Length)
                return false;

            return !_characters[index].IsNull();
        }
        #endregion

        #region Async
        public async Task<Exception> PushCharacterAsync(CharacterData data)
        {
            return await _gameManager.webApi.AddCharacterAsync(data);
        }       
        public async void PullCharactersAsync(string address){
            if(address.IsNullOrEmpty() || _isPulling)
                return;

            _isPulling = true;
            var ex   = default(Exception);
            var data = default(uint[]);
            var ids  = default(IList<string>);
           
            //Get characters
            (ex,data) = await _gameManager.blockChainApi.GetPixiesAsync(address);
            if(!ex.IsNull()){
                _isPulling = false;
                onPullCharactersFailed?.Invoke(this,ex);
                return;
            }

            //Add to ids
            if(!data.IsNullOrEmpty()){
                if(ids.IsNull()){
                    ids = new List<string>();
                }
                    
                for (var i = 0; i < data.Length; i++){
                    ids.Add(data[i].ToString());
                }
            }
                
            //Get InBattle characters
            (ex,data) = await _gameManager.blockChainApi.GetInBattlePixiesAsync(address);
            if(!ex.IsNull()){
                _isPulling = false;
                onPullCharactersFailed?.Invoke(this,ex);
                return;
            }

            //Add to ids
            if(!data.IsNullOrEmpty()){
                if(ids.IsNull()){
                    ids = new List<string>();
                }
                    
                for (var i = 0; i < data.Length; i++){
                    var newId = data[i].ToString();
                    if(ids.Contains(newId))
                        continue;

                    ids.Add(newId);
                }
            }
            
            _cacheCharacters?.Clear();
            if(!_characters.IsNullOrEmpty())
                _cacheCharacters?.AddRange(_characters);
            
            //_characters?.Dispose();
            _characters = null;

            
            if(!ids.IsNullOrEmpty()){
                var fetch_ids = new List<string>(ids);
                
                //remove unused characters
                _cacheCharacters.RemoveAll(x => {
                    var remove =  !fetch_ids.Contains(x.data.id);
                    if(remove){
                        x.Dispose();
                    }
                    return remove;
                });
                
                //filter ids not downloaded
                fetch_ids.RemoveAll(x => !_cacheCharacters.Find(y => y.data.id == x).IsNull());
                
                //Get Characters
                (ex,_characters) = await _gameManager.webApi.GetCharactersAsync(fetch_ids);
                if(!ex.IsNull()){   
                    _isPulling = false;    
                    onPullCharactersFailed?.Invoke(this,ex);
                    return;
                }

                //Combine cache and new one
                if(!_characters.IsNullOrEmpty()){
                    _cacheCharacters.AddRange(_characters);
                }
        
                _characters = _cacheCharacters?.ToArray();
                _cacheCharacters?.Clear();
            }

            if(!_characters.IsNullOrEmpty()){
                for (var i = 0; i < _characters.Length; i++)
                {
                    _characters[i]?.Hide();
                    _characters[i]?.transform.SetParent(_parent);

                    //Set status active / not
                    if(!data.IsNullOrEmpty())
                        _characters[i].data.status.active = Array.Exists(data,e => e.ToString() == _characters[i].data.id);
                }
            }

           _isPulling = false;
           onPullCharactersCompleted?.Invoke(this);
        }
        #endregion

        #region doozyMethods
        private bool _requiredPull;
        public void SetRequiredPull(bool value){
            _requiredPull = value;
        }
        public void PullCharactersAsync(){
            var address = _gameManager.blockChainApi.account;
            PullCharactersAsync(address);
        }
        public void PullCharactersWhenRequired(){
            if(_requiredPull)
                PullCharactersAsync();

            _requiredPull = false;
        }
        #endregion
                
        #region callbacks
        private void OnDestroy() {
            onSelectedCharacterChange = null;
            onPullCharactersCompleted = null;
            onPullCharactersFailed    = null;
        }
        private async void OnPullCharactersFailed(CharacterManager cm, Exception ex)
        {
            await _gameManager.uiManager.uiMessage.Show(ex.Message,false);
            PullCharactersAsync();
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

