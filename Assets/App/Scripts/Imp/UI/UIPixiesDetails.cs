using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIPixiesDetails : SerializedMonoBehaviour,IDisposable
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
        private string _marketPlaceUrl;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textBattleAttempt,_textWinrate,_textDescription,_textEnergy;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Image _thumbnail;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIButton _btnUnActive;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIButton _btnClose,_btnToBattle,_btnMarketPlace;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private GameObject _statusActive;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private GameObject _statusInBattle;
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
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _btnClose.OnClick.OnTrigger.Event.AddListener(Hide);
            _btnToBattle.OnClick.OnTrigger.Event.AddListener(ToBattle);
            //_btnActive.OnClick.OnTrigger.Event.AddListener(UnActiveClick);
            _btnUnActive.OnClick.OnTrigger.Event.AddListener(SetInBattleClick);
            _btnMarketPlace.OnClick.OnTrigger.Event.AddListener(GotoMarketPlace);
        }
        public void Show(Character character){
            _character = character;
            Show();
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
        public void Refresh(){
            _thumbnail.sprite       = _character.data.thumbnail;
            _textBattleAttempt.text = _character.data.status.battleCount.ToString();
            _textWinrate.text       = $"{_character.data.status.winRate}%";
            _textEnergy.text        =  $"{_character.data.status.energy}/{_character.data.status.maxEnergy}";
            _textDescription.text   = _character.data.story;

            _btnToBattle.gameObject.SetActive(!_character.data.status.active);
            _statusInBattle.gameObject.SetActive(_character.data.status.active);

            _statusActive.gameObject.SetActive(_character.data.status.active);
            _btnUnActive.gameObject.SetActive(!_character.data.status.active);
            _btnUnActive.Interactable = !_character.data.status.active;
        }
        #endregion

        #region buttons
        private async void SetInBattleClick(){
            _btnUnActive.Interactable = false;
            
            var retry   = false;
            do
            {   
                var ex = await _character.SetInBattleAsync();
                retry = !ex.IsNull()? await _gameManager.uiManager.uiMessage.Show(ex.Message + "\nRetry?") : false;
            } while (retry);

            Refresh();
        }
        private void ToBattle()
        {
            Hide();
            var index = Array.IndexOf(_gameManager.characterManager.characters,_character);
            _gameManager.characterManager.SetMainCharacter(index);
            _gameManager.uiManager.uiMatchMaking.Show();
           
        }
        private void GotoMarketPlace()
        {
            Application.OpenURL(_marketPlaceUrl);
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
            _character.onDataUpdated += OnCharacterDataUpdated;
            OnCharacterDataUpdated(_character,_character.data);
        }
        private void OnHide()
        {
            _character.onDataUpdated -= OnCharacterDataUpdated;
        }  
        private void OnCharacterDataUpdated(Character character, CharacterData data)
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