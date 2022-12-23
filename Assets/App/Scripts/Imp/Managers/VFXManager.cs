using System;
using System.Collections.Generic;
using Evesoft;
using NFTGame.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(VFXManager))]
    public class VFXManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpComponent = "Component";
        const string grpConfigs = "Configs";
        const string grpRuntime = "Runtime";
        const string grpParent = "Parent";
        const string grpVFXS = "VFXs";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpComponent)]
        private Transform _parent;

        [SerializeField,BoxGroup(grpComponent),InlineEditor,ListDrawerSettings(DraggableItems = false,Expanded = true)]
        private Config.VFXConfig[] _presets;
        #endregion

        #region private
        //[ShowInInspector,ReadOnly,FoldoutGroup(grpRuntime)]
        private Dictionary<string,Queue<IVFX>> _pull;
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            _pull = new Dictionary<string, Queue<IVFX>>();
            this.LogCompleted(nameof(Init));      
        }
        public IVFX GetVFX(Params.VFXVariantParams param){
            var key = param.ToString();
            //$"Get variant FVX {variant.ToString()}".Log();

            if(_pull.ContainsKey(key) && !_pull[key].IsNullOrEmpty()){
                //$"Contain VFX".Log();
                return _pull[key].Dequeue();      
            }else{
                //$"Create VFX {variant.ToString()}".Log();

                var vfxConfig = _presets.Find(x=> (x.variant.type == param.type && x.variant.variant == param.variant));
                if(vfxConfig.IsNull()){
                    "$Not Finding VFX".Log();
                    return null;
                }
                    
                var go = GameObject.Instantiate(vfxConfig.prefab.gameObject);
                //$"Game Object {go}".Log();
                
                var vfx = go.GetComponent<IVFX>();
                //$"vfx Object {vfx}".Log();
                
                //$"Create VFX Complete".Log();
            
                if(!vfx.IsNull()){
                    vfx.Init();
                    vfx.onComplete += (vx)=>
                    {
                        AddPull(vfxConfig.variant,vx);
                    };
                    vfx.transform.SetParent(_parent);
                }

                return vfx;
            } 
        }
        private void AddPull(Params.VFXVariantParams param,IVFX vfx){
            var key = param.ToString();
           
            //Initialize
            if(!_pull.ContainsKey(key))
                _pull[key] = new Queue<IVFX>();

            //Add VFX
            vfx.Hide();
           _pull[key].Enqueue(vfx);
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