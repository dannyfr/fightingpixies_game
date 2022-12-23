using UnityEngine;
using Sirenix.OdinInspector;
using DarkTonic.MasterAudio;

namespace NFTGame.Component
{
    [HideMonoScript]
    [AddComponentMenu(Utils.EditorMenu.Component + nameof(SoundPlay))]
    public class SoundPlay : SerializedMonoBehaviour
    {
        #region const
        const string grpConfig = "Config";
        #endregion
        

        #region fields
        [SerializeField,BoxGroup(grpConfig),SoundGroup]
        private string _sound;

        [SerializeField,BoxGroup(grpConfig)]
        private float _delay;
        #endregion

        #region methods
        public void PlaySound(){
            MasterAudio.PlaySound(sType:_sound,delaySoundTime:_delay);
        }
        #endregion
    }
}