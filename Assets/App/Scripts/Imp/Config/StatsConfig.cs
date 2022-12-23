using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(StatsConfig),fileName = nameof(StatsConfig))]
    public class StatsConfig : SerializedScriptableObject
    {
        const int max = 100;

        #region fields
        [MinMaxSlider(0,500,true)]
        public Vector2Int hp;

        [MinMaxSlider(0,max,true)]
        public Vector2Int attack;

        [MinMaxSlider(0,max,true)]
        public Vector2Int defense;

        [MinMaxSlider(0,max,true)]
        public Vector2Int speed;
        #endregion
    }
}