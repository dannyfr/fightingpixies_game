using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;
using DarkTonic.MasterAudio;
using UnityEngine.UI;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIHistory : SerializedMonoBehaviour,IDisposable
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
        private UIHistoryController _controller;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private GameObject _emptyLabel;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private float[] _initSfxVolume,_initBgmVolume;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _controller.Init(gameManager);
            _emptyLabel.Hide();

            _gameManager.historyManager.onPullHistoryonCompleted += OnPullHistoryCompleted;
        }
        public void Show()
        {
           
        }
        public void Hide()
        {
            _view.Hide();
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
            
        }
        private void OnHide()
        {

        }       
        private void OnPullHistoryCompleted(HistoryManager hm)
        {
           _emptyLabel.SetActive(hm.history.IsNullOrEmpty());
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