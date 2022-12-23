using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Doozy.Engine.UI;
using Evesoft;
using TMPro;

namespace NFTGame.UI
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class UIRewardPreviewPixies : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field
        [SerializeField,FoldoutGroup(grpComponent)]
        private Image _image;
        #endregion

        #region private
        private Texture2D _tmpTexture;
        private GameManager _gameManager;
        private Character _character;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
        }
        public void Set(Character character){    
            _character    = character;
        }
        public void Show()
        {
            gameObject.Show();
        }
        public void Hide()
        {
           gameObject.Hide();
        }
        public void Refresh(){
            gameObject.SetActive(!_character.IsNull());
            if(_character.IsNull()) 
                return;
                
            _image.sprite = _character.data.thumbnail;
        }
        #endregion
       
        #region Callback
        private void OnDestroy()
        { 
           
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