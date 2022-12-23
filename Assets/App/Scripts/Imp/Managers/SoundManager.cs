using System;
using System.Collections.Generic;
using Evesoft;
using Sirenix.OdinInspector;
using UnityEngine;
using DarkTonic.MasterAudio;
using NFTGame.Utils;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(SoundManager))]
    public class SoundManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpComponent = "Component";
        const string grpConfigs = "Configs";
        const string grpRuntime = "Runtime";
        #endregion

        #region fields
        
        #endregion

        #region private
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            this.LogCompleted(nameof(Init));
        }
        public void PlaySFX(string groupName,string variant){
            MasterAudio.PlaySound(sType:groupName,variationName:variant);
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