using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame.Component
{
    [HideMonoScript,HideReferenceObjectPicker,RequireComponent(typeof(Camera))]
    [AddComponentMenu(Utils.EditorMenu.Component + nameof(ThumbnailBuilder))]
    public class ThumbnailBuilder : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpPreference = "Preference";
        const string grpMarin = "Margin";
        #endregion

        #region fields
        [SerializeField,InlineEditor]
        private Config.ThumbnailConfig _config;
        #endregion

        #region private
        private Camera _camera;
        private RenderTexture _renderTexture;
        #endregion

        #region methods
        public void Init() {
            _camera = GetComponent<Camera>();
            gameObject.Hide();
        }
        public Texture2D Create(GameObject target){
            base.gameObject.Show();

            //Store Prev Position
            var prevSpritePos = target.transform.position;
            var initedPos     = _camera.transform.position;

            //Positioning sprite to Camera
            target.transform.position = new Vector3(initedPos.x,initedPos.y,prevSpritePos.z);

            //Set new hight & width base on size bound to renderer texture
            var bounds = GetBounds(target);
            var width  = bounds.size.x > bounds.size.y ? _config.size.x : Mathf.RoundToInt(bounds.size.x/bounds.size.y * _config.size.x);
            var height = bounds.size.y > bounds.size.x ? _config.size.y : Mathf.RoundToInt(bounds.size.y/bounds.size.x * _config.size.y);
            
            var renderConfig = new RenderTextureDescriptor();
                renderConfig.width  = width;
                renderConfig.height = height;
                renderConfig.autoGenerateMips = false;
                renderConfig.useDynamicScale  = true;
                renderConfig.volumeDepth = 1;
                renderConfig.msaaSamples = 1;
                renderConfig.colorFormat = RenderTextureFormat.ARGB32;
                renderConfig.dimension   = UnityEngine.Rendering.TextureDimension.Tex2D;
  
            _renderTexture = new RenderTexture(renderConfig);
            _renderTexture.wrapMode   = TextureWrapMode.Clamp;
            _renderTexture.filterMode = FilterMode.Point;
            _camera.targetTexture = _renderTexture;
        
            //Positioning Camera
            _camera.transform.position = new Vector3(bounds.center.x,bounds.center.y,_camera.transform.position.z);

            //Fit to Bounds width
            var camSize      = _camera.OrthographicRect().size;
            var delta        = _camera.orthographicSize / camSize.x;
            var endSize      = (bounds.size.x + _config.margin) * delta;
            _camera.orthographicSize = endSize;

            //Start Capturing
            var prevActive       = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            _camera.Render();

            var texture  = new Texture2D(width,height,TextureFormat.ARGB32,false);
            texture.filterMode = FilterMode.Point;
            texture.ReadPixels(new Rect(0, 0,width,height), 0, 0);
            texture.Apply();
            
            //Reset
            target.transform.position = prevSpritePos;
            RenderTexture.active            = prevActive;
            _camera.transform.position      = initedPos;

            base.gameObject.Hide();
            return texture;
        }      
        private Bounds GetBounds(GameObject target){
            if(target.IsNull())
                return default(Bounds);

            var sprites = target.GetComponentsInChildren<SpriteRenderer>();
            if(sprites.IsNullOrEmpty())
                return default(Bounds);

            var left = 0f;
            var right= 0f;
            var top = 0f;
            var buttom = 0f;
            var front = 0f;
            var back = 0f;
            var zPos = 0f;
        
            for (var i = 0; i < sprites.Length; i++)
            {
                var bounds = sprites[i].bounds;
                var center = bounds.center;
                var size   = bounds.size;

                if(i==0){
                    left  = center.x - (size.x/2);
                    right = center.x + (size.x/2);
                    top = center.y + (size.y/2);
                    buttom = center.y - (size.y/2);
                    front = sprites[i].transform.position.z;
                    back = sprites[i].transform.position.z;
                }else{
                    left = center.x - (size.x/2) < left? center.x - (size.x/2) : left;
                    right = center.x + (size.x/2) > right ? center.x + (size.x/2) : right;
                    top = center.y + (size.y/2) > top ? center.y + (size.y/2) : top;
                    buttom = center.y - (size.y/2) < buttom ? center.y - (size.y/2) : buttom;
                    front = sprites[i].transform.position.z < front? sprites[i].transform.position.z : front;
                    back = sprites[i].transform.position.z > back? sprites[i].transform.position.z : back;
                }

                zPos += sprites[i].transform.position.z;
            }

            zPos/= sprites.Length;

            var _center = new Vector3((left + right) /2f,(buttom + top)/2f,zPos);
            var _size   = new Vector3(right - left,top - buttom,back-front);

            return new Bounds(_center,_size);
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