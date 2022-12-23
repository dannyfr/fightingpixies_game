using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(NameConfig),fileName = nameof(NameConfig))]
    public class NameConfig : SerializedScriptableObject
    {
        #region fields
        [ListDrawerSettings(DraggableItems = false,Expanded = true)]
        public List<string> names;
        #endregion
    }
}