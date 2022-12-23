using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using System;

namespace NFTGame.Limbs
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Limbs + nameof(Head))]
    public class Head : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpPlacement = "Placements";
        #endregion

        #region fields
        [BoxGroup(grpPlacement),SerializeField]
        private Transform _helmetPlacement,_facialHairPlacement;
        #endregion
        
        #region property
        public Transform facialHairPlacement => _facialHairPlacement;
        public Transform helmetPlacement => _helmetPlacement;
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

