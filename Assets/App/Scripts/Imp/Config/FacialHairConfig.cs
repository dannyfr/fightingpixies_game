using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.ConfigAccessories + nameof(FacialHairConfig),fileName = nameof(FacialHairConfig))]
    public class FacialHairConfig : SerializedScriptableObject
    {
        #region fields
        [ShowInInspector,BoxGroup(nameof(id)),DisplayAsString,PropertyOrder(-1),HideLabel]
        public string id => name;

        [InlineEditor(inlineEditorMode:InlineEditorModes.LargePreview)]
        public Accessories.FacialHair pref;
        #endregion
    }
}