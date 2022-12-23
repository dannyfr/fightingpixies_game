using System;
using Evesoft;
using Sirenix.OdinInspector;
using UnityEngine;
using NFTGame.Utils;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(CameraManager))]
    public class CameraManager : SerializedMonoBehaviour,IDisposable
    {
        #region Property
        public new Camera camera => _camera;
        #endregion

        #region private
        private GameManager _gameManager;
        private Camera _camera;
        private Vector3 _initedPosition;
        private float _initedSize;
        #endregion

        #region Methods
        public void Init(GameManager gameManager)
        {
            _gameManager    = gameManager;
            //_uiCamera       = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            _camera         = Camera.main;
            _initedPosition = _camera.transform.position;
            _initedSize     = _camera.orthographicSize;
            this.LogCompleted(nameof(Init));
        } 
        public void CropFit(Rect rect)
        {
            var camera          = _camera;
            var forgroundSize   = rect.size;
            var camSize         = camera.OrthographicRect().size;

            var delta               = camera.orthographicSize / camSize.x;
            var beginSize           = camera.orthographicSize;
            var endSize             = forgroundSize.x * delta;
            camera.orthographicSize = endSize;
            camSize                 = camera.OrthographicRect().size;

            if(camSize.y > forgroundSize.y)
            {
                delta   = camera.orthographicSize / camSize.y;
                endSize = forgroundSize.y * delta;
                camera.orthographicSize = endSize;
            }

            var position = (Vector3)rect.Center();
                position.z = _initedPosition.z;

            _camera.transform.position = position;
            _camera.orthographicSize   = endSize;
        }       
        public void BoundaryView(Rect rect)
        {
            if(_camera.IsNull())
                return;

            var resolveOffset = GetBoundaryOffsetPosition(rect,_camera.OrthographicRect());
            if(resolveOffset != Vector2.zero)
                _camera.gameObject.transform.Translate(resolveOffset);
        }
        public void Reset(){
            if(!Application.isPlaying)
                return;
                
            _camera.transform.position = _initedPosition;
            _camera.orthographicSize = _initedSize;
        }
        private Vector2 GetBoundaryOffsetPosition(Rect rect,Rect camRect)
        {
            var resolveOffset = Vector2.zero;
            var changed = Vector2.zero;

            //Check x min
            if(camRect.xMin < rect.xMin)
            {
                resolveOffset.x += rect.xMin - camRect.xMin;
                changed.x +=1;
            }
                
            //Check x Max
            if(camRect.xMax > rect.xMax)
            {
                resolveOffset.x -= camRect.xMax - rect.xMax;
                changed.x -= 1;
            }
                
            //Check y min
            if(camRect.yMin < rect.yMin)
            {
                resolveOffset.y += rect.yMin - camRect.yMin;
                changed.y +=1;
            }
                
            //Check y max
            if(camRect.yMax > rect.yMax)
            {
                resolveOffset.y -= camRect.yMax - rect.yMax;
                changed.y -=1;
            }
                
            if(changed.x == 0 )
                resolveOffset.x = 0;

            if(changed.y == 0 )
                resolveOffset.y = 0;

            return resolveOffset;
        }
        #endregion

        #region callback
        private void OnDestroy()
        {
            _gameManager    = null;
            _camera         = null;
        }
        #endregion
    
        #region IDisposable
        public void Dispose()
        {
            gameObject.Destroy();
        }
        #endregion
    }
}


