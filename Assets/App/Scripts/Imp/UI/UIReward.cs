using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;
using DG.Tweening;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIReward : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig    = "Config";
        const string grpComponent = "Component";
        const string grpRuntime   = "Runtime";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpConfig)]
        private bool _showByViewName;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Component.CameraRenderView _renderView;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private RectTransform _icon;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private RectTransform _panel;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIButton _btnClaim,_btnCustomize,_btnOpenPackage;
        #endregion

        #region private
        private bool _isShowing;
        private int _rewardIndex;
        private bool _isGetRewardComplete;
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _btnClaim.OnClick.OnTrigger.Event.AddListener(Claim);
            _btnCustomize.OnClick.OnTrigger.Event.AddListener(Customize);
            _btnOpenPackage.OnClick.OnTrigger.Event.AddListener(OpenPackage);
            
            _icon.gameObject.Hide();
            _btnClaim.gameObject.Hide();
            _btnCustomize.gameObject.Hide();
            _btnOpenPackage.gameObject.Hide();
            _panel.gameObject.Hide();

            _renderView.Init();
            _renderView.Hide();
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
        private void ShowReward(int rewardIndex){
            if(_gameManager.rewardManager.availableRewards.IsNullOrEmpty() 
            || rewardIndex < 0 
            || rewardIndex >= _gameManager.rewardManager.availableRewards.Length)
                return;

            var character = _gameManager.rewardManager.availableRewards[rewardIndex].character; 
            if(character.IsNull())
                return;
            
            var duration = 0.25f;
            _renderView.Show();
            _icon.gameObject.Show();
            _panel.gameObject.Show();
             
            //Tween thumbnail
            character.Show();
            character.ShowFrame();
            _renderView.Fit(character.gameObject);
            _icon.gameObject.transform.localScale = Vector3.zero;
            _icon.DOScale(Vector3.one,duration);

            var canCustomize = this.CanCustomize(character.data);
            _btnCustomize.gameObject.SetActive(canCustomize);
            _btnClaim.gameObject.SetActive(!canCustomize);

            if(!canCustomize){
                //Tween Button claim
                _btnClaim.gameObject.transform.localScale = Vector3.zero;
                _btnClaim.gameObject.Show();
                _btnClaim.RectTransform.DOScale(Vector3.one,duration);
            }else{
                //Twenn Button customize
                character.color = Color.black;
                _btnCustomize.gameObject.transform.localScale = Vector3.zero;
                _btnCustomize.gameObject.Show();
                _btnCustomize.RectTransform.DOScale(Vector3.one,duration);
            }
        }
        private void GetReward(){
            var address = _gameManager.blockChainApi.account;
            _gameManager.rewardManager.GetRewardAsync(address);
        }
        private bool CanCustomize(CharacterData data){
            var result = false;
            if(data.setsCollections.IsNullOrEmpty())
                return result;

            result |= !data.setsCollections.heads.IsNullOrEmpty() && data.setsCollections.heads.Length > 1;
            result |= !data.setsCollections.bodys.IsNullOrEmpty() && data.setsCollections.bodys.Length > 1;
            result |= !data.setsCollections.legs.IsNullOrEmpty() && data.setsCollections.legs.Length > 1;
     
            return result;
        }
        #endregion
       
        #region Buttons
        private void OpenPackage()
        {
            _btnOpenPackage.gameObject.Hide();
            ShowReward(_rewardIndex);
        }
        private void Claim(){
            var address = _gameManager.blockChainApi.account;

            var onClaimCompleteHandler = default(Action<RewardManager,int>);
            var onClaimFailedHandler   = default(Action<RewardManager,Exception,int>);
                
            onClaimCompleteHandler = (rm,index)=>
            {
                rm.onClaimCompleted -= onClaimCompleteHandler;
                rm.onClaimFailed    -= onClaimFailedHandler;
                
                OnClaimCompleted(index);
            };
            onClaimFailedHandler  = (rm,ex,index)=>{
                rm.onClaimCompleted -= onClaimCompleteHandler;
                rm.onClaimFailed    -= onClaimFailedHandler;

                OnClaimFailed(ex,index);
            };

            _gameManager.rewardManager.onClaimCompleted += onClaimCompleteHandler;
            _gameManager.rewardManager.onClaimFailed    += onClaimFailedHandler;
            
            //_btnClaim.gameObject.Hide();
            _gameManager.rewardManager.ClaimRewardAsync(address,_rewardIndex);
        }
        private void Customize(){ 
            _gameManager.uiManager.uiCreator.SetCharacter(_rewardIndex);
            _gameManager.uiManager.uiCreator.Show();
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
            if(!_isGetRewardComplete){
                _gameManager.rewardManager.onGetRewardCompleted += OnGetRewardComplete;
                _gameManager.rewardManager.onGetRewardFailed    += OnGetRewardFailed;
                GetReward();
            }else{
                ShowReward(_rewardIndex);
            }
        }
        private void OnHide()
        {
            _gameManager.rewardManager.onGetRewardCompleted -= OnGetRewardComplete;
            _gameManager.rewardManager.onGetRewardFailed    -= OnGetRewardFailed;
            _renderView.Hide();
        }       
        private void OnGetRewardComplete(RewardManager rm)
        {
            _rewardIndex = 0;
            _isGetRewardComplete = true;

            if(!rm.availableRewards.IsNullOrEmpty()){  
                ShowReward(_rewardIndex);
            }else{
                _gameManager.uiManager.uiMainMenu.Show();
            }
        }  
        private async void OnGetRewardFailed(RewardManager rm, Exception ex)
        {
            await _gameManager.uiManager.uiMessage.Show(ex.Message,false);
            GetReward();
        }       
        private void OnClaimCompleted(int rewardIndex){
            _gameManager.rewardManager.availableRewards[rewardIndex].character?.Hide();
            _rewardIndex = rewardIndex + 1;

            if(_rewardIndex < _gameManager.rewardManager.availableRewards.Length){
                ShowReward(_rewardIndex);
            }else{
                _gameManager.uiManager.uiRewardPreview.Show();
            }
        }
        private async void OnClaimFailed(Exception ex,int rewardIndex){
            var retry = await _gameManager.uiManager.uiMessage.Show(ex.Message + "\nRetry?");
            if(!retry)
                return;
            
            Claim();
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