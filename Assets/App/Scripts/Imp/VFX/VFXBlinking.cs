using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using DG.Tweening;

namespace NFTGame.VFX
{
    [AddComponentMenu(Utils.EditorMenu.VFX + nameof(VFXBlinking))]
    public class VFXBlinking : SerializedMonoBehaviour,IVFX,IDisposable
    {
        #region const
        const string grpVFX = nameof(VFXBasic);
        const string grpTween = nameof(Tween);
        #endregion

        #region fields
        [SerializeField,FoldoutGroup(grpTween)]
        private float _duration;

        [SerializeField,FoldoutGroup(grpTween)]
        private int _loop;

        [SerializeField,FoldoutGroup(grpTween)]
        private Ease _ease = Ease.Linear;

        [SerializeField,FoldoutGroup(grpTween),ColorPalette]
        private Color _color;
        #endregion

        #region private
        private Tween _tween;
        private Color _prevColors;
        private Character _character;
        #endregion
 
        #region IVFX
        public bool isActive => !_tween.IsNull() && !_tween.IsComplete();
        public event Action<IVFX> onComplete;

        public void Init()
        {
          
        }
        public void Show(object param)
        {   
            gameObject.Show();
            var cfg = (Params.VFXBlinkingParams)param;
            _character = cfg.character;
            
            var loops = cfg.loop == 0 ? _loop : cfg.loop;
                loops *= 2;

            if(_character.IsNull()){
                "Wrong Parameter".LogError();
                return;
            }

            _character.color = Color.white;
            _prevColors = _character.color;   
            _tween      = DOTween.To(()=> _character.color, (value)=>_character.color = value,_color,_duration).SetLoops(loops,LoopType.Yoyo).SetEase(_ease).SetAutoKill();
            _tween.onComplete = ()=>
            {
                _tween      = null;
                _character  = null;
                _prevColors = Color.white;
                
                onComplete?.Invoke(this);
            };
        }
        public void Hide()
        {
            gameObject.Hide();

            //Kill Tweens
            _tween?.Kill(false);

            //Reset colors
            _prevColors = Color.white;
        }
        #endregion

        #region callbacks
        private void OnDestroy() {
            onComplete = null;
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