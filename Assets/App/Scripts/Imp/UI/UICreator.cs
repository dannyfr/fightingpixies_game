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
    public class UICreator : SerializedMonoBehaviour,IDisposable
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
        private Vector2 _offsetView;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Component.CameraRenderView _viewCamera;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textName,_textStory;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIButton _btnCreate;

        [SerializeField,Required,BoxGroup(grpComponent),ListDrawerSettings(Expanded = true,DraggableItems = false)]
        private UICreatorType[] _creatorTypes;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private Character _character;
        private Vector3 _prevCharPos;
        private int _rewardIndex;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            for (var i = 0; i < _creatorTypes.Length; i++)
            {
                _creatorTypes[i].Init(gameManager);
                _creatorTypes[i].onChangeType += OnChangeType;
            }

            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _btnCreate.OnClick.OnTrigger.Event.AddListener(Create);
            _viewCamera.Init();
            _viewCamera.gameObject.Hide();
        }
        public void SetCharacter(int rewardIndex){
            _rewardIndex = rewardIndex;
            _character   = _gameManager.rewardManager.availableRewards[_rewardIndex].character;
            _character.color = Color.white;
            _character.HideFrame();
            _character.Show();
            _prevCharPos = _character.transform.position;
            
            var pos = _character.transform.position;
            pos.x = _viewCamera.transform.position.x;
            pos.y = _viewCamera.transform.position.y;
            _character.transform.position = pos;

            for (var i = 0; i < _creatorTypes.Length; i++)
            {
                _creatorTypes[i].SetCharacter(_character);
            }

            Refresh();
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
            _textName.text  = _character.data.name;
            _textStory.text = _character.data.story;

            //Fit to Bounds width
            _viewCamera.Align(_character.gameObject,_offsetView);
        }
        #endregion

        #region Buttons
        private async void Create(){ 
            var accept = await _gameManager.uiManager.uiMessage.Show("This action is irreversible",true,"ARE YOU SURE?");
            if(!accept)
                return;
            
            //flatting character
            _gameManager.characterManager.characterBuilder.Flatten(_character);

            //Apply Data
            for (var i = 0; i < _creatorTypes.Length; i++)
            {
                _creatorTypes[i].Apply();
            }

            //Back
            Doozy.Engine.UI.Input.BackButton.Instance.Execute();
        }
        #endregion
        
        #region Callback
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
            _viewCamera.gameObject.Show();
        }
        private void OnHide()
        {
           _character.transform.position = _prevCharPos;
           _character = null;
           _viewCamera.gameObject.Hide();
        }       
        private void OnChangeType(UICreatorType type){
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