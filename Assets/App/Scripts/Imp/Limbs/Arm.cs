using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Evesoft;

namespace NFTGame.Limbs
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Limbs + nameof(Arm))]
    public class Arm : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpPlacement = "Placements";
        #endregion

        #region fields
        [BoxGroup(grpPlacement),SerializeField]
        private Transform _handPlacement;
        #endregion
        
        #region property
        public Transform handPlacement => _handPlacement;
        public SpriteRenderer spriteRenderer {
            get{
                if(_spriteRenderer.IsNull())
                    _spriteRenderer = GetComponent<SpriteRenderer>();

                return _spriteRenderer;
            }
        }
        #endregion

        #region private
        private SpriteRenderer _spriteRenderer;
        #endregion

        #region IDisposable
        public void Dispose()
        {
            gameObject.Destroy();
        }  
        #endregion
        
    }

}

