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
    public class UIPreBattle : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        const string grpTween = "Tween";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpConfig)]
        private bool _showByViewName;

        [SerializeField,BoxGroup(grpTween)]
        private Ease _ease = Ease.InOutSine;

        [SerializeField,BoxGroup(grpTween)]
        private float _offset,_duration;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private RectTransform _userRect,_opponentRect;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private Component.CameraRenderView _userCamView,_opponentCamView;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private TextMeshProUGUI _labelLoading;

        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private Vector2 _userRectInitPos,_opponentRectInitPos;
        private bool _tweenCompleted;
        private Tween _loadingTween;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _userCamView.Init();
            _userCamView.Hide();
            _opponentCamView.Init();
            _opponentCamView.Hide();
            _userRectInitPos = _userRect.anchoredPosition;
            _opponentRectInitPos = _opponentRect.anchoredPosition;
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
        
            var pos = _userRect.anchoredPosition - (Vector2.right * _offset);
            _userRect.anchoredPosition = pos;

            pos = _opponentRect.anchoredPosition + (Vector2.right * _offset);
            _opponentRect.anchoredPosition = pos;
        }
        public void Hide()
        {
            _view.Hide();
        }    
        private void Refresh(){
            var character = _gameManager.characterManager.mainCharacter;   
            if(character){
                character.ShowFrame();
                character.SetFaceDirection(Direction.Right);
                character.Show();
                _userCamView.Show();
                _userCamView.Fit(character.gameObject);
            }
            
            var opponent = _gameManager.matchMakingManager.opponent;
            if(opponent){
                opponent.ShowFrame();
                opponent.SetFaceDirection(Direction.Left);
                opponent.ShowStat(_gameManager.matchMakingManager.stats);
                opponent.Show();
                _opponentCamView.Show();
                _opponentCamView.Fit(opponent.gameObject);
            }
        }
        private void Tween(){
            _tweenCompleted = false;
            _userRect.DOAnchorPos(_userRectInitPos,_duration).SetEase(_ease).SetAutoKill();
            _opponentRect.DOAnchorPos(_opponentRectInitPos,_duration).SetEase(_ease).SetAutoKill().onComplete = ()=>
            {
                _tweenCompleted = true;
            };
            _loadingTween = DOTween.To(()=> _labelLoading.text.Length - 3,(x)=> _labelLoading.maxVisibleCharacters = x,_labelLoading.text.Length,_duration*2).SetLoops(-1).SetEase(_ease);
        }
        private void PrepareBattle(){
            var battleId = _gameManager.matchMakingManager.battleID;
            _gameManager.battleManager.PrepareBattleAsync(battleId.ToString());
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
            _gameManager.characterManager.SetRequiredPull(true);
            _gameManager.battleManager.onPrepareBattleFailed    += OnPrepareBattleFailed;
            _gameManager.battleManager.onPrepareBattleCompleted += OnPrepareBattleCompleted;

            Refresh();
            Tween();
            
            PrepareBattle();
        }
        private void OnHide()
        {
            _gameManager.battleManager.onPrepareBattleFailed    -= OnPrepareBattleFailed;
            _gameManager.battleManager.onPrepareBattleCompleted -= OnPrepareBattleCompleted;

            _userCamView.Hide();
            _opponentCamView.Hide();
            _loadingTween?.Kill();
        } 
        private async void OnPrepareBattleFailed(BattleManager bm, Exception ex)
        {
            await _gameManager.uiManager.uiMessage.Show(ex.Message,false);
            PrepareBattle();
        }
        private async void OnPrepareBattleCompleted(BattleManager bm)
        {
            await new WaitUntil(()=> _tweenCompleted);
            await new WaitForSeconds(1);
           _gameManager.uiManager.uiBattle.Show();
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