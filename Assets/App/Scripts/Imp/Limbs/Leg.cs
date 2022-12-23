using Sirenix.OdinInspector;
using Evesoft;
using System;
using UnityEngine;

namespace NFTGame.Limbs
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Limbs + nameof(Leg))]
    public class Leg : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grPlacement = "Placements";
        #endregion

        #region fields
      
        #endregion
        
        #region property
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

