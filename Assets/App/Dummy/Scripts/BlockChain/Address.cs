using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame.Dummy.BlockChain
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.DataDummy + nameof(Address),fileName = nameof(Address))]
    public class Address : SerializedScriptableObject
    {
        [BoxGroup(nameof(address)),HideLabel]
        public string address;
    }
}