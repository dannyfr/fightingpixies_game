using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using DG.Tweening;
using UnityEngine.UI;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIDefeat : SerializedMonoBehaviour,IDisposable
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

        // [SerializeField,Required,BoxGroup(grpComponent)]
        // private UIButton _btnFindMatch;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Component.CameraRenderView _renderView;
        
        [SerializeField,Required,BoxGroup(grpComponent)]
        private Animator _animator;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private Character _character;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.battleManager.onBattleEnd += OnBattleEnd;

            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            //_btnFindMatch.OnClick.OnTrigger.Event.AddListener(FindMatch);
            

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
        #endregion

        // #region buttons
        // private void FindMatch()
        // {
        //    _gameManager.uiManager.uiMatchMaking.Show();
        // }
        // #endregion
       
        #region Callback
        private void OnDestroy()
        { 
            if(!_gameManager.IsNull()){
                _gameManager.battleManager.onBattleEnd -= OnBattleEnd;
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
        }
        private void OnHide()
        {
            _character?.Hide();
            _renderView.Hide();
            _animator.enabled = false;
        }      
        private void OnBattleEnd(BattleManager bm, BattleResult result)
        {
            if(result.win)
                return;

            _character = result.character;       
            Show();
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