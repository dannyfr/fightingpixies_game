using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIMainMenu : SerializedMonoBehaviour,IDisposable
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
        UIButton _btnBattle,_btnPixies;
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
            _btnBattle.OnClick.OnTrigger.Event.AddListener(MatchMaking);
            _btnPixies.OnClick.OnTrigger.Event.AddListener(ViewPixies);
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
        private void MatchMaking()
        {
            _gameManager.uiManager.uiPixies.mode = UI.UIPixies.Mode.ChoosePixies;
            _gameManager.uiManager.uiPixies.Show();
        }
        private void ViewPixies()
        {
            _gameManager.uiManager.uiPixies.mode = UI.UIPixies.Mode.ViewPixies;
            _gameManager.uiManager.uiPixies.Show();
        }
        #endregion
       
        #region Callback
        private void OnDestroy()
        { 
           
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
            _gameManager.characterManager.mainCharacter?.Hide();
            _gameManager.matchMakingManager.opponent?.Hide();
        }
        private void OnHide()
        {
            
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