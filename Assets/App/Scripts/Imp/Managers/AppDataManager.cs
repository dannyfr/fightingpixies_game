using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using System.Collections.Generic;
using NFTGame.Utils;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(AppDataManager))]
    public class AppDataManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpFields = "Fields";
        #endregion

        #region fields
        [SerializeField,InlineEditor]
        private AppData _appData;
        #endregion

        #region property
        public AppData appData => _appData;
        #endregion

        #region private
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            this.LogCompleted(nameof(Init));
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