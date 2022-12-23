using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIClaim : SerializedMonoBehaviour,IDisposable
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
        private Component.CameraRenderView _renderView;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIButton _btnClaim;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Animator _animator;
        
        [SerializeField,Required,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textTotal;

        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private Character _character;
        private bool _isNextClaim = false;
        private string _battleId;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.historyManager.onPullHistoryonCompleted += OnPullHistoryCompleted;
            _gameManager.historyManager.onPullHistoryonFailed  += OnPullHistoryFailed;
            _gameManager.historyManager.onClaimResultCompleted += OnClaimBattleResultCompleted;
            _gameManager.historyManager.onClaimResultFailed    += OnClaimBattleResultFailed;

            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _btnClaim.OnClick.OnTrigger.Event.AddListener(Claim);

            _renderView.Init();
            _renderView.Hide();
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
        private async void ClaimBattleResult(params string[] battleIds){
            if(battleIds.IsNullOrEmpty())
                return;

            for (var i = 0; i < battleIds.Length; i++)
            {
                _isNextClaim = false;
                _battleId    = battleIds[i];
                _character   = _gameManager.historyManager.GetRewardByBattleId(_battleId);
                if(_character.IsNull())
                    continue;
                
                await new WaitUntil(()=> !_gameManager.characterManager.isPullingCharacter);
                _textTotal.text = $"{i+1}/{battleIds.Length}";
                Show();
                await new WaitUntil(()=> _isNextClaim);
            }

            _isNextClaim = false;
            _battleId    = null;
            _character   = null;

            Hide();
            _gameManager.uiManager.uiMainMenu.Show();
        }
        #endregion
       
        #region buttons
        private void Claim()
        {
            _gameManager.historyManager.Claim(_battleId);
        }
        #endregion
        
        #region Callback
        private void OnDestroy()
        { 
            if(!_gameManager.IsNull()){
                _gameManager.historyManager.onPullHistoryonCompleted -= OnPullHistoryCompleted;
                _gameManager.historyManager.onPullHistoryonFailed    -= OnPullHistoryFailed;
                _gameManager.historyManager.onClaimResultCompleted -= OnClaimBattleResultCompleted;
                _gameManager.historyManager.onClaimResultFailed    -= OnClaimBattleResultFailed;
            }
        }
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
            _character?.ShowFrame();
            _character?.SetFaceDirection(Direction.Left);
            _character?.Show();
            _renderView.Fit(_character.gameObject);
            _renderView.Show();

            _animator.enabled = true;
            _animator.Rebind();
            
            _gameManager.characterManager.SetRequiredPull(true);
        }
        private void OnHide()
        {
            _character?.Hide();
            _renderView.Hide();
            _animator.enabled = false;
        }      
        private void OnPullHistoryCompleted(HistoryManager hm)
        {
            ClaimBattleResult(hm.GetAvailableClaim());
        }
        private void OnPullHistoryFailed(HistoryManager hm, Exception ex)
        {
            hm.PullHistory();
        }
        private void OnClaimBattleResultCompleted(HistoryManager hm, string battleId)
        {
            _isNextClaim = true;
        }
        private async void OnClaimBattleResultFailed(HistoryManager hm, string battleId, Exception ex)
        {
            var retry = await _gameManager.uiManager.uiMessage.Show(ex.Message + "\nRetry?");
            if(!retry)
                return;

            Claim();
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