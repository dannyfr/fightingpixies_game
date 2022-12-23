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
    public class UIRewardPreview : SerializedMonoBehaviour,IDisposable
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
        private UIButton _btnClaim;

        [SerializeField,Required,BoxGroup(grpComponent),ListDrawerSettings(DraggableItems = false,Expanded = true)]
        private UIRewardPreviewPixies[] _pixies;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            //Init pixies
            for (var i = 0; i < _pixies.Length; i++)
            {
                _pixies[i].Init(gameManager);
            }

            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _btnClaim.OnClick.OnTrigger.Event.AddListener(Next);
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
            for (var i = 0; i < _pixies.Length; i++)
            {
                _pixies[i].Refresh();
            }
        }
        #endregion
        
        #region Buttons
        private void Next(){
            _gameManager.rewardManager.Clear();
            _gameManager.uiManager.uiMainMenu.Show();
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
                //OnHide();
            }
        }
        private void OnShow()
        {
            var characters = _gameManager.rewardManager.availableRewards;
            for (var i = 0; i < characters.Length; i++)
            {
                if(i < _pixies.Length){
                    _pixies[i].Set(characters[i].character);
                }
            }  
            
            Refresh();
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