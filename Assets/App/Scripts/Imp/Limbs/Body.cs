using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using System;

namespace NFTGame.Limbs
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Limbs + nameof(Body))]
    public class Body : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpPlacement = "Placements";
        #endregion
        
        #region fields
        [BoxGroup(grpPlacement),SerializeField]
        private Transform _headPlacement,_ArmPlacement,_legPlacement;
        #endregion

        #region property
        public Transform headPlacement  => _headPlacement; 
        public Transform armPlacement  => _ArmPlacement;
        public Transform legPlacement  => _legPlacement; 
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

