using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIHeader : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpConfig)]
        private bool _showByViewName;

        [SerializeField,BoxGroup(grpConfig)]
        private int _maxCharAddress = 10;

        [SerializeField,BoxGroup(grpConfig)]
        private int _charShow = 4;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textAddress;
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
            _gameManager.authManager.onLoginComplete += OnLoginCompleted;
        }

        private void Refresh(){
            var address = _gameManager.blockChainApi.account;
            if(address.Length > _maxCharAddress){
                var first  = "";
                var last   = "";
                
                for (var i = 0; i < _charShow; i++){
                    first += address[i];
                    last  += address[address.Length-_charShow+i];
                }

                address = $"{first}...{last}";
            }

            _textAddress.text = address;
        }
        #endregion

        #region Callback
        private void OnDestroy() {
            if(!_gameManager.IsNull()){
                _gameManager.authManager.onLoginComplete -= OnLoginCompleted;
            }
        }
        private void OnVisibilityChange(float visibility)
        {
            if (visibility > 0 && !_isShowing)
            {
                //OnShow();
                _isShowing = true;
            }
            else if(_isShowing && visibility == 0)
            {
                _isShowing = false;
                //OnHide();
            }
        }
        private void OnLoginCompleted(AuthManager auth)
        {
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