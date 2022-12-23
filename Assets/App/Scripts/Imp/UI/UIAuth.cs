using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIAuth : SerializedMonoBehaviour,IDisposable
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

        [SerializeField,BoxGroup(grpComponent)]
        private UIButton _btnLogin;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _btnLogin.OnClick.OnTrigger.Event.AddListener(Login);
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

        #region buttons
        private void Login()
        {
            _gameManager.authManager.LoginAsync();
        }
        #endregion
       
        #region callback
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
            _gameManager.authManager.onLoginComplete += OnLoginCompleted;
            _gameManager.authManager.onLoginFailed   += OnLoginFailed;
        }
        private void OnHide()
        {
            _gameManager.authManager.onLoginComplete -= OnLoginCompleted;
            _gameManager.authManager.onLoginFailed   -= OnLoginFailed;
        }
        private void OnLoginCompleted(AuthManager am)
        {
            _gameManager.uiManager.uiReward.Show();
        }
        private async void OnLoginFailed(AuthManager am, Exception ex)
        {
            var retry = await _gameManager.uiManager.uiMessage.Show(ex.Message + "\nRetry?");
            if(!retry)
              return;

            Login();
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