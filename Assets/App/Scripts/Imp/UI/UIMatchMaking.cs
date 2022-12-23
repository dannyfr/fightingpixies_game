using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Doozy.Engine;
using Evesoft;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIMatchMaking : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpConfig)]
        private bool _showByViewName;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Component.CameraRenderView _userCamView,_opponentCamView;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIButton _btnChoosePixies,_btnRefresh,_btnBattle;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private GameObject _emptyCharTemplate;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private GameEventListener _findNewOpponentListener,_useCurrentOpponentListener;
        #endregion

        #region private
        private bool _isFindNewOpponent = true;
        private bool _isShowing;
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _btnRefresh.OnClick.OnTrigger.Event.AddListener(FindOpponent);
            _btnBattle.OnClick.OnTrigger.Event.AddListener(Battle);
            _btnChoosePixies.OnClick.OnTrigger.Event.AddListener(ChoosePixies);
            
            _userCamView.Init();
            _userCamView.Hide();
            _opponentCamView.Init();
            _opponentCamView.Hide();

            _findNewOpponentListener.Event.AddListener((events)=> _isFindNewOpponent = true );
            _useCurrentOpponentListener.Event.AddListener((events)=> _isFindNewOpponent = false );
        }
        public void Show()
        {
            if(_view.IsVisible)
                return;

            if(_showByViewName)
            {
                Doozy.Engine.GameEventMessage.SendEvent(_view.ViewName); 
            }
            else
            {
                _view.Show();
            }  
        }
        public void Hide()
        {
            _view.Hide();
        }
        private void Refresh(){
            var character = _gameManager.characterManager.mainCharacter;
            
            if(character){
                character.ShowFrame();
                character.SetFaceDirection(Direction.Right);
                character.Show();
                _userCamView.Show();
                _userCamView.Fit(character.gameObject);
            }else{
                _userCamView.Hide();
            }
        
            var opponent = _gameManager.matchMakingManager.opponent;
            if(opponent){
                opponent?.ShowFrame();
                opponent?.SetFaceDirection(Direction.Left);
                opponent?.ShowStat(opponent.data.showstat == default(Stats)? _gameManager.matchMakingManager.GetRandomStat():opponent.data.showstat);
                opponent?.Show();
                _opponentCamView.Show();
                _opponentCamView.Fit(opponent.gameObject);
                _emptyCharTemplate.Hide();
            }else
            {
                _emptyCharTemplate.Show();
                _opponentCamView.Show();
                _opponentCamView.Fit(_emptyCharTemplate);
            }
        }       
        #endregion
       
        #region Buttons
        private void ChoosePixies()
        {
            _gameManager.uiManager.uiPixies.mode = UIPixies.Mode.ChoosePixies;
            _gameManager.uiManager.uiPixies.Show();
        }
        private void FindOpponent()
        {
            _emptyCharTemplate.Show();
            _opponentCamView.Show();
            _opponentCamView.Fit(_emptyCharTemplate);

            _btnBattle.Interactable  = false;
            _btnRefresh.Interactable = false;

            var attackerId = _gameManager.characterManager.mainCharacter?.data?.id;
            _gameManager.matchMakingManager.FindOpponent(attackerId);
        }
        private void Battle(){
            if(_gameManager.matchMakingManager.opponent.IsNull()){
                FindOpponent();
            }else{
                var battleId = _gameManager.matchMakingManager.battleID;
                var tokenId  = _gameManager.characterManager.mainCharacter.data.id;
                _gameManager.matchMakingManager.Battle(battleId,tokenId);
            }
        }       
        #endregion

        #region Callback
        private void OnVisibilityChange(float visibility)
        {
            if (visibility > 0 && !_isShowing)
            {
                OnShow();
                _isShowing = true;
            }
            else if(_isShowing && visibility == 0)
            {
                _isShowing = false;
                OnHide();
            }
        }
        private void OnShow()
        {
            _gameManager.characterManager.onSelectedCharacterChange += OnSelectedCharacterChange;
            _gameManager.matchMakingManager.onFindOpponentCompleted += OnFindOpponentCompleted;
            _gameManager.matchMakingManager.onFindOpponentFailed    += OnFindOpponentFailed;
            _gameManager.matchMakingManager.onBattleCompleted       += OnBattleCompleted;
            _gameManager.matchMakingManager.onBattleFailed          += OnBattleFailed;
            _gameManager.matchMakingManager.onBattleUnavailable     += OnBattleUnavailable;

            Refresh();

            if(_isFindNewOpponent){
                FindOpponent();
            }
        }
        private void OnHide()
        {
            _gameManager.characterManager.onSelectedCharacterChange -= OnSelectedCharacterChange;
            _gameManager.matchMakingManager.onFindOpponentCompleted -= OnFindOpponentCompleted;
            _gameManager.matchMakingManager.onFindOpponentFailed    -= OnFindOpponentFailed;
            _gameManager.matchMakingManager.onBattleCompleted       -= OnBattleCompleted;
            _gameManager.matchMakingManager.onBattleFailed          -= OnBattleFailed;
             _gameManager.matchMakingManager.onBattleUnavailable    -= OnBattleUnavailable;

            _userCamView.Hide();
            _opponentCamView.Hide();

            _gameManager.characterManager.mainCharacter?.Hide();
            _gameManager.matchMakingManager.opponent?.Hide();
        }
        private void OnSelectedCharacterChange(CharacterManager cm,Character character)
        {
            Refresh();
        }
        private void OnFindOpponentCompleted(MatchMakingManager mm,MatchMakingManager.MathmakingResult result)
        {
            _btnBattle.Interactable  = true;
            _btnRefresh.Interactable = true; 

            Refresh();
        }
        private async void OnFindOpponentFailed(MatchMakingManager mm,Exception ex)
        {
            var retry = await _gameManager.uiManager.uiMessage.Show(ex.Message + "\nRetry?");
            if(!retry){
                _btnBattle.Interactable  = true;
                _btnRefresh.Interactable = true;
            }else{
                FindOpponent();
            }
        }
        private void OnBattleCompleted(MatchMakingManager mm)
        {
            _gameManager.uiManager.uiPreBattle.Show();
        }
        private async void OnBattleFailed(MatchMakingManager mm, Exception ex)
        {
            var retry = await _gameManager.uiManager.uiMessage.Show(ex.Message + "\nRetry?");
            if(!retry)
                return;

            Battle();
        }
        private async void OnBattleUnavailable(MatchMakingManager mm)
        {
            await _gameManager.uiManager.uiMessage.Show("Battle Unavailable",false);
            FindOpponent();
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