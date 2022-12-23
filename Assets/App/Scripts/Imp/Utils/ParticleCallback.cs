using System;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame.Utils
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class ParticleCallback : SerializedMonoBehaviour
    {
        #region events
        public event Action onStop;
        #endregion
        
        #region callbacks
        private void OnParticleSystemStopped(){
            //"Particle Stop Callback".Log();
            onStop?.Invoke();
        }
        #endregion
    }
}


