using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.ConfigAccessories + nameof(PantsConfig),fileName = nameof(PantsConfig))]
    public class PantsConfig : SerializedScriptableObject
    {
        #region fields
        [ShowInInspector,BoxGroup(nameof(id)),DisplayAsString,PropertyOrder(-1),HideLabel]
        public string id => name;

        [InlineEditor(inlineEditorMode:InlineEditorModes.LargePreview)]
        public Accessories.Pants pref;
        #endregion
    }
}