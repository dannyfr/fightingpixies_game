using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.ConfigVFX + nameof(VFXConfig),fileName = nameof(VFXConfig))]
    public class VFXConfig : SerializedScriptableObject
    {
        #region fields
        [SerializeField,BoxGroup("Variant"),HideLabel]
        private Params.VFXVariantParams _variant;

        [SerializeField,TextArea(5,10),BoxGroup("Description"),HideLabel]
        private string description;

        [SerializeField,HideLabel,BoxGroup("Prefab")]
        private GameObject _prefab;
        #endregion

        #region property
        public GameObject prefab => _prefab;
        public Params.VFXVariantParams variant => _variant;
        #endregion
    }
}