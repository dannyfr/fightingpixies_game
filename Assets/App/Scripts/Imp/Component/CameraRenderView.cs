using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame.Component
{
    [HideMonoScript,HideReferenceObjectPicker,RequireComponent(typeof(Camera))]
    [AddComponentMenu(Utils.EditorMenu.Component + nameof(CameraRenderView))]
    public class CameraRenderView : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpPreference = "Preference";
        const string grpMarin = "Margin";
        #endregion

        #region private
        private Camera _camera;
        private Vector3 _initPos;
        #endregion

        #region methods
        public void Init() {
            _camera = GetComponent<Camera>();
            _initPos = _camera.transform.position;
        }
        public void Show(){
            gameObject.Show();
        }
        public void Hide(){
            gameObject.Hide();
        }
        public void Fit(GameObject target){  
            target.transform.position = _initPos;
            
            //Set new hight & width base on size bound to renderer texture
            var bounds = GetBounds(target);
            var camSize= _camera.OrthographicRect().size;
            var fit    = 0f;
            var camDiv = 0f;

            if(bounds.size.x > bounds.size.y){
                fit = bounds.size.x;
                camDiv = camSize.x;
            }else{
                fit = bounds.size.y;
                camDiv = camSize.y;
            }
              
            //Fit to Bounds width
            var delta        = _camera.orthographicSize / camDiv;
            var endSize      = fit * delta;
            _camera.orthographicSize = endSize;

            //Positioning sprite to Camera
            var pos = _camera.transform.position;
            pos.x =  bounds.center.x;
            pos.y =  bounds.center.y;
            _camera.transform.position = pos;
        }      
        public void Align(GameObject target,Vector2 offset = default(Vector2)){
            target.transform.position = _initPos + (Vector3)offset;
            _camera.transform.position = _initPos ;
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