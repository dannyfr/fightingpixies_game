using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.ConfigAccessories + nameof(ClothConfig),fileName = nameof(ClothConfig))]
    public class ClothConfig : SerializedScriptableObject
    {
        #region fields
        [ShowInInspector,BoxGroup(nameof(id)),DisplayAsString,PropertyOrder(-1),HideLabel]
        public string id => name;

        [InlineEditor(inlineEditorMode:InlineEditorModes.LargePreview)]
        public Accessories.Cloth pref;
        #endregion
    }
}