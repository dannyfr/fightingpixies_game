using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using Evesoft;
using Doozy.Engine.UI;
using System;
using DG.Tweening;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIPixiesRowCellView : SerializedMonoBehaviour
    {
        #region const
        const string grpComponent = "Component";
        #endregion

        #region fields
        [SerializeField,FoldoutGroup(grpComponent)]
        private GameObject container;

        [SerializeField,FoldoutGroup(grpComponent)]
        private Image _thumbnail;

        [SerializeField,FoldoutGroup(grpComponent)]
        private TextMeshProUGUI _status;

        [SerializeField,FoldoutGroup(grpComponent)]
        private UIButton _unActive;

        [SerializeField,FoldoutGroup(grpComponent)]
        private UIButton _active;

        [SerializeField,FoldoutGroup(grpComponent)]
        private UIButton _button;

        [SerializeField,FoldoutGroup(grpComponent)]
        private GameObject _loading,_loadingFill;
        #endregion

        #region private
        private UIPixiesData _data;
        private GameManager _gameManager;
        private bool _inited;
        private Sprite _defaultThumbnail;
        #endregion

        #region methods
        public void Init(GameManager gameManager){         
            if(_inited)
                return;
                
            _gameManager      = gameManager;
            _defaultThumbnail = _thumbnail.sprite;
            _active.OnClick.OnTrigger.Event.AddListener(UnActiveClick);
            _unActive.OnClick.OnTrigger.Event.AddListener(ActiveClick);
            _button.OnClick.OnTrigger.Event.AddListener(OnClick);
            _inited = true;
        }
        public void SetData(UIPixiesData data)
        {
            _data = data;

            if(!_data.IsNull() && _data.character){
                _data.character.onDataUpdated -= OnCharacterDataUpdated;
                _data.character.onDataUpdated += OnCharacterDataUpdated;
            }
         
            Refresh();
        }
        public void Refresh(){
            container.SetActive(!_data.IsNull());

            if(!_data.IsNull() && !_data.character.IsNull()){
                _thumbnail.sprite = _data.character.data.thumbnail;
                _status.text      = _data.character.data.status.active? "IN BATTLE" : "AVAILABLE";
                _active.gameObject.SetActive(_data.character.data.status.active);
                _unActive.gameObject.SetActive(!_data.character.data.status.active);
                _unActive.Interactable = !_data.character.data.status.active;
                _active.Interactable = _data.character.data.status.active;

                _button.Interactable = true;
                HideLoading();
            }else{
                _thumbnail.sprite = _defaultThumbnail;
                _status.text = "";
                _active.gameObject.Hide();
                _unActive.gameObject.Show();
                _unActive.Interactable = false;
                _button.Interactable = false;
                ShowLoading();
            }
        }
        private void ShowLoading(){
            _loading.Show();
            _loadingFill.transform.DORotate(Vector3.forward * -360,1.5f,RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
        }
        private void HideLoading(){
            _loading.Hide();
            _loadingFill.transform.DOKill();
        }
        #endregion
        
        #region Buttons
        private async void ActiveClick(){
            _active.Interactable =  _unActive.Interactable  = false;
            var retry = false;
            do
            {
                //_gameManager.uiManager.uiLoading.Show();
                var ex = await _data.character.SetInBattleAsync();
                //_gameManager.uiManager.uiLoading.Hide();
                retry = !ex.IsNull() ? await _gameManager.uiManager.uiMessage.Show(ex.Message + "\nRetry?") : false; 
            } while (retry);

            Refresh();
        }
        private async void UnActiveClick(){
            _active.Interactable =  _unActive.Interactable  = false;
            var retry = false;
            do
            {
                //_gameManager.uiManager.uiLoading.Show();
                var ex = await _data.character.RetrieveFromBattle();
                //_gameManager.uiManager.uiLoading.Hide();
                retry = !ex.IsNull() ? await _gameManager.uiManager.uiMessage.Show(ex.Message) : false; 
            } while (retry);
            Refresh();
        }
        private void OnClick()
        {
            switch(_gameManager.uiManager.uiPixies.mode){
                case UIPixies.Mode.ViewPixies:{
                    _gameManager.uiManager.uiPixiesDetails.Show(_data.character);
                    break;
                }

                case UIPixies.Mode.ChoosePixies:{       
                    if(_data.character.data.status.active){
                        var message = "Your selected pixies currently in battle";
                        _gameManager.uiManager.uiMessage.Show(message,false);
                    }else{
                        //SetMainCharacter
                        var index = Array.IndexOf(_gameManager.characterManager.characters,_data.character);
                        _gameManager.characterManager.SetMainCharacter(index);
                        _gameManager.uiManager.uiMatchMaking.Show();
                    }
                    break;
                }
            }
        }    
        #endregion
        
        #region callbacks
        private void OnCharacterDataUpdated(Character character, CharacterData data)
        {
            Refresh();
        }     
        #endregion
    }
}