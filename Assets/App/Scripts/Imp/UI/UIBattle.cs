using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using DG.Tweening;
using DarkTonic.MasterAudio;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIBattle : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpConfig)]
        private bool _showByViewName;
        
        [SerializeField,BoxGroup(grpConfig),SoundGroup]
        private string _fightSFX;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIView _view;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private RectTransform _ribbon;
        
        [SerializeField,Required,BoxGroup(grpComponent)]
        private UIBattleStat _battleStatLeft,_battleStatRight;

        [SerializeField,Required,BoxGroup(grpComponent)]
        private CanvasGroup _fightLabel;
        #endregion

        #region private
        private bool _isShowing;
        private GameManager _gameManager;
        private Vector3[] _initPos;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _view.OnVisibilityChanged.AddListener(OnVisibilityChange);
            _fightLabel.gameObject.Hide();
            _initPos = new Vector3[3];
            _initPos[0] = _battleStatLeft.rectTransform.anchoredPosition;
            _initPos[1] = _battleStatRight.rectTransform.anchoredPosition;
            _initPos[2] = _ribbon.anchoredPosition;
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
        private void TweenStats(Action onComplete = null){
            var offset = 200;
            var duration = 0.5f;
            _battleStatLeft.rectTransform.anchoredPosition  -= Vector2.right * offset;
            _battleStatRight.rectTransform.anchoredPosition += Vector2.right * offset;
            _ribbon.anchoredPosition += Vector2.up * offset;

            _battleStatLeft.rectTransform.DOAnchorPos(_initPos[0],duration).SetAutoKill();
            _battleStatRight.rectTransform.DOAnchorPos(_initPos[1],duration).SetAutoKill();
            _ribbon.DOAnchorPos(_initPos[2],duration).SetAutoKill().onComplete = ()=>
            {
                onComplete?.Invoke();
            };
        }
        private void TweenFight(Action onComplete = null){
            _fightLabel.transform.localScale = Vector3.one * 5;
            _fightLabel.alpha = 0;
            _fightLabel.gameObject.Show();
            
            var duration = 0.3f;
            _fightLabel.transform.DOScale(Vector3.one,duration);
            _fightLabel.DOFade(1,duration).onComplete = ()=>
            {
                MasterAudio.PlaySound(_fightSFX);
                _fightLabel.DOFade(0,duration).SetLoops(5,LoopType.Yoyo).SetAutoKill().SetEase(Ease.Linear).onComplete = ()=>
                {
                    _fightLabel.gameObject.Hide();
                    onComplete?.Invoke();
                };
            };
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
            _gameManager.battleManager.onBattleAttack += OnBattleAttack;
            _battleStatLeft.SetCharacter(_gameManager.battleManager.characters[0]);
            _battleStatRight.SetCharacter(_gameManager.battleManager.characters[1]);
            _battleStatLeft.HP  = 0;
            _battleStatRight.HP = 0;
            
            TweenStats(()=>
            {
                _battleStatLeft.enableTween = true;
                _battleStatRight.enableTween = true;
                _battleStatLeft.TweenMaxHP();
                _battleStatRight.TweenMaxHP();

                TweenFight();
            });
              
            var arena = _gameManager.arenaManager.GetRandomArena();
            _gameManager.battleManager.Simulate(arena);
        }
        private void OnHide()
        {
            _gameManager.battleManager.onBattleAttack -= OnBattleAttack; 
        }    
        private void OnBattleAttack(BattleManager BM, BattleAttack battleAttack)
        {
            if(battleAttack.victim == _battleStatLeft.character){
                _battleStatLeft.HP -= battleAttack.damage;
            }   

            if(battleAttack.victim == _battleStatRight.character){
                _battleStatRight.HP -= battleAttack.damage;
            }
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