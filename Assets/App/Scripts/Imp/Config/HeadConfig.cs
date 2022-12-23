using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.ConfigLimb + nameof(HeadConfig),fileName = nameof(HeadConfig))]
    public class HeadConfig : SerializedScriptableObject
    {
        #region fields
        [ShowInInspector,BoxGroup(nameof(id)),DisplayAsString,PropertyOrder(-1),HideLabel]
        public string id => name;

        [InlineEditor(inlineEditorMode:InlineEditorModes.LargePreview)]
        public Limbs.Head pref;
        #endregion
    }
}