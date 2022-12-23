using System.Runtime;
using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Evesoft;
using TMPro;
using DG.Tweening;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIBattleStat : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpComponent)]
        private Image _hpFill,_hpImg,_shadowFill,_shadowImg;

        [SerializeField,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textName;

        [SerializeField,BoxGroup(grpComponent)]
        private Gradient _gradient;
        #endregion

        #region property
        public Character character => _character;
        public RectTransform rectTransform {
            get{
                if(_rectTransform.IsNull())
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }   
        public bool enableTween {get => _enableTween; set => _enableTween = value;}
        
        [ShowInInspector,BoxGroup(grpRuntime),ReadOnly]
        public float HP {
            get => _character.IsNull()? 0 : _hpFill.fillAmount * _character.data.stats.hp;
            set {
                if(_hpFill.IsNull())
                    return;
                    
                var duration   = 0.3f;
                var amount = value / _character.data.stats.hp;
                var color  = _gradient.Evaluate(amount);
                var shadowColor   = _gradient.Evaluate(amount);
                    shadowColor.a = _shadowFill.color.a;

                _shadowFill.DOKill();
                _hpFill.DOKill();

                if(_enableTween){
                    _hpFill.DOFillAmount(amount,duration).SetAutoKill();
                    _hpImg.DOColor(color,duration).SetAutoKill();
                    _shadowFill.DOFillAmount(amount,duration).SetAutoKill().SetDelay(duration);
                    //_shadowImg.DOColor(shadowColor,duration).SetAutoKill().SetDelay(duration);
                }else{
                    _hpFill.fillAmount = amount;
                    _hpImg.color = color;
                    _shadowFill.fillAmount = amount;
                    //_shadowImg.color = shadowColor;
                }
            }
        }       
        #endregion
        
        #region private
        private Character _character;
        private GameManager _gameManager;
        private RectTransform _rectTransform;
        private bool _enableTween;
        #endregion

        #region methods
        public void SetCharacter(Character character){
            _character = character;
            _textName.text = _character?.data.name;
        }
        public void TweenMaxHP(){
            var time = 1.5f;
            _hpFill.DOFillAmount(1,time).SetAutoKill();
            _shadowFill.DOFillAmount(1,time).SetAutoKill();
            _hpImg.DOColor(_gradient.Evaluate(1),time).SetAutoKill();
        }
        #endregion

        #region Callback
        private void OnDestroy()
        { 
           
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