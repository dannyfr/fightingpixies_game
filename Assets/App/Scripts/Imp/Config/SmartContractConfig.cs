using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(SmartContractConfig),fileName = nameof(SmartContractConfig))]
    public class SmartContractConfig : SerializedScriptableObject
    {
        #region fields
        [TextArea(2,6)]
        public string address;

        [TextArea(2,20)]
        public string abi;
        #endregion
    }
}