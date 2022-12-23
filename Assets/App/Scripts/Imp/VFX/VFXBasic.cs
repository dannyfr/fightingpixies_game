using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Evesoft;

namespace NFTGame.VFX
{
    [HideMonoScript, HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.VFX + nameof(VFXBasic))]
    public class VFXBasic : SerializedMonoBehaviour, IVFX,IDisposable
    {
        #region fields
        const string grpComponent = "Component";
        #endregion

        #region fields
        [SerializeField,FoldoutGroup(grpComponent)]
        private ParticleSystem _particle;

        [SerializeField,FoldoutGroup(grpComponent)]
        private Utils.ParticleCallback _particleCallback;
        #endregion

        #region private
        private bool _inited;
        #endregion

        #region IVFX
        public bool isActive => _particle.isPlaying;
    
        public event Action<IVFX> onComplete;

        public void Init()
        {
            if(_inited)
                return;

            _particleCallback.onStop += OnComplete;

            _inited = true;
        }
        public void Show(object param)
        {
            gameObject.Show();
            _particle.Play(true);
            
        }
        public void Hide()
        {
            _particle.Stop(true);
            gameObject.Hide();
        }
        #endregion

        #region callbacks
        private void OnComplete(){
            onComplete?.Invoke(this);
        }
        private void OnDestroy() {
            _particleCallback.onStop -= OnComplete;
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


