using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(BlockChainConfig),fileName = nameof(BlockChainConfig))]
    public class BlockChainConfig : SerializedScriptableObject
    {
        #region fields
        public string chain;
        public string network;
        public string rpc;

        [InlineEditor]
        public SmartContractConfig _uniqSC;

        [InlineEditor]
        public SmartContractConfig _battleSC;
        #endregion
    }
}