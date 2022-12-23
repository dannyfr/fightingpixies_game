using Sirenix.OdinInspector;
using Evesoft;
using System;
using UnityEngine;

namespace NFTGame.Accessories
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Accessories + nameof(Cloth))]
    public class Cloth : SerializedMonoBehaviour,IAccessories,IDisposable
    {
        #region const
        const string grPlacement = "Placements";
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

