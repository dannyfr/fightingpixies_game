using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.ConfigAccessories + nameof(HelmetConfig),fileName = nameof(HelmetConfig))]
    public class HelmetConfig : SerializedScriptableObject
    {
        #region fields
        [ShowInInspector,BoxGroup(nameof(id)),DisplayAsString,PropertyOrder(-1),HideLabel]
        public string id => name;

        [InlineEditor(inlineEditorMode:InlineEditorModes.LargePreview)]
        public Accessories.Helmet pref;
        #endregion
    }
}