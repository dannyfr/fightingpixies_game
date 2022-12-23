using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(ThumbnailConfig),fileName = nameof(ThumbnailConfig))]
    public class ThumbnailConfig : SerializedScriptableObject
    {
        #region fields
        [BoxGroup("Size"),HideLabel]
        public Vector2Int size = new Vector2Int(512,512);

        [BoxGroup("Margin"),HideLabel]
        public float margin;
        #endregion
    }
}