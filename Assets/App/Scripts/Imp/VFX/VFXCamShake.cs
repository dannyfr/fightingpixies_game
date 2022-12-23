using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using DG.Tweening;

namespace NFTGame.VFX
{
    [AddComponentMenu(Utils.EditorMenu.VFX + nameof(VFXCamShake))]
    public class VFXCamShake : SerializedMonoBehaviour,IVFX,IDisposable
    {
        #region const
        const string grpVFX = nameof(VFXBasic);
        const string grpTween = nameof(Tween);
        #endregion

        #region fields

        [SerializeField,FoldoutGroup(grpTween)]
        private float _duration = 0.5f;

        [SerializeField,FoldoutGroup(grpTween)]
        private float _strength = 3;

        [SerializeField,FoldoutGroup(grpTween)]
        private int _vibrato = 10;

        [SerializeField,FoldoutGroup(grpTween)]
        private int _randomness = 90;
    
        #endregion

        #region private
        private Tween _tween;
        private Camera _camera;
        #endregion

        #region IVFX
        public bool isActive => !_tween.IsNull();
        public event Action<IVFX> onComplete;

        public void Init()
        {
           _camera = Camera.main;
        }
        public void Show(object param)
        {
            gameObject.Show();
            _tween = _camera.DOShakePosition(_duration,_strength,_vibrato,_randomness,true);
            _tween.onComplete = ()=>
            {
                _tween = null;
                onComplete?.Invoke(this);
            };
        }
        public void Hide()
        {
            gameObject.Hide();
            _tween?.Kill(false);
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