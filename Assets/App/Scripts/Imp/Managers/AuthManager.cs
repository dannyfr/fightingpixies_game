using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Evesoft;
using NFTGame.Utils;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(AuthManager))]
    public class AuthManager : SerializedMonoBehaviour,IDisposable
    {
        #region events
        public event Action<AuthManager> onLogin;
        public event Action<AuthManager> onLoginComplete;
        public event Action<AuthManager,Exception> onLoginFailed;
        public event Action<AuthManager> onLogout;
        #endregion

        #region private
        private GameManager _gameManager;
        private bool _isSigning;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            
            this.LogCompleted(nameof(Init));
        }
        public async void LoginAsync(){
            if(_isSigning)
                return;

            _isSigning = true;
            onLogin?.Invoke(this);

            var address = _gameManager.blockChainApi.account;
            var ex      = default(Exception);
            
            //Connect wallet
            if(address.IsNullOrEmpty()){
                (ex,address) = await _gameManager.blockChainApi.ConnectWalletAsync();
                if(!ex.IsNull()){
                    _isSigning = false;
                    onLoginFailed?.Invoke(this,ex);
                    return;
                }
            }
            
            //Get Token from BE
            //if(token.IsNullOrEmpty()){
                ex = await _gameManager.webApi.GetTokenAsync(address,true);
                if(!ex.IsNull()){
                    _isSigning = false;
                    onLoginFailed?.Invoke(this,ex);
                    return;
                }
            //}
            
            _isSigning = false;
            onLoginComplete?.Invoke(this);
        }
        #endregion

        #region callbacks
        private void OnDestroy() {
            onLoginComplete = null;
            onLoginFailed = null;
            onLogout = null;
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
