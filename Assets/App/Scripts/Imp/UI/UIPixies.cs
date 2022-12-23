using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;
using Doozy.Engine;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIPixies : SerializedMonoBehaviour,IDisposable
    {
        public enum Mode{
            ViewPixies,
            ChoosePixies
        }

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
        private TextMeshProUGUI _labelViewPixies,_labelChoosePixies,_emptyLabel;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIPixiesController _controller;

        [SerializeField,BoxGroup(grpComponent)]
        private GameEventListener _viewPixiesListener,_choosePixiesListener;
        #endregion

        #region property
        public Mode mode {get => _mode ;set => _mode = value;}
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private Mode _mode;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _controller.Init(gameManager);
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _viewPixiesListener.Event.AddListener((events)=> OnSetViewPixies());
            _choosePixiesListener.Event.AddListener((events)=> OnSetChoosePixies());

            _gameManager.characterManager.onPullCharactersCompleted += OnPullCharactersCompleted;
            _gameManager.characterManager.onPullCharactersFailed    += OnPullCharactersFailed;
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
            _labelViewPixies.gameObject.SetActive(_mode == Mode.ViewPixies);
            _labelChoosePixies.gameObject.SetActive(_mode == Mode.ChoosePixies);
        }
        #endregion
       
        #region Callback
        private void OnDestroy() {
            if(!_gameManager.IsNull()){
                _gameManager.characterManager.onPullCharactersCompleted -= OnPullCharactersCompleted;
                _gameManager.characterManager.onPullCharactersFailed    -= OnPullCharactersFailed;
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
            Refresh();
        }
        private void OnHide()
        {
            
        }
        private void OnSetViewPixies(){
            _mode = Mode.ViewPixies;
        }
        private void OnSetChoosePixies(){
            _mode = Mode.ChoosePixies;
        }
        
        private void OnPullCharactersCompleted(CharacterManager cm)
        {
            _emptyLabel.gameObject.SetActive(cm.characters.IsNullOrEmpty());
        }
        private void OnPullCharactersFailed(CharacterManager cm, Exception ex)
        {
            _emptyLabel.gameObject.Hide();
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