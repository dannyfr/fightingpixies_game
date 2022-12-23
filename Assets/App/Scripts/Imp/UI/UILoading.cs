using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using DG.Tweening;
using TMPro;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UILoading : SerializedMonoBehaviour,IDisposable
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
        private string _waitMessage;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private GameObject _icon;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textMessage;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private string _defaultMessage;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _defaultMessage = _textMessage.text;
        }
        public void Show(string message = null)
        {
            _textMessage.text = message.IsNullOrEmpty()? _waitMessage : message; 

            if(_view.IsVisible)
                return;

            if(_showByViewName){
                Doozy.Engine.GameEventMessage.SendEvent(_view.ViewName); 
            }else{
                _view.Show();
            }
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
            _icon.transform.DORotate(Vector3.forward * -360,1.5f,RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
        }
        private void OnHide()
        {
            _icon.transform.DOKill();
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