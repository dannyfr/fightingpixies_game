using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Utils
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class GizmosDrawer : SerializedMonoBehaviour
    {
        #region fields
        [SerializeField]
        private Color _color;

        [SerializeField]
        private float _size;
        #endregion

        private void OnDrawGizmos() {
            Gizmos.color = _color;
            Gizmos.DrawWireCube(transform.position,Vector2.one*_size);
        }
    }
}

