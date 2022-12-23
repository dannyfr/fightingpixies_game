using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(StoryConfig),fileName = nameof(StoryConfig))]
    public class StoryConfig : SerializedScriptableObject
    {
        #region fields
        [TextArea(3,5)]
        [ListDrawerSettings(DraggableItems = false,Expanded = true)]
        public List<string> stories;
        #endregion
    }
}