using System;
using System.Collections.Generic;
using Evesoft;
using NFTGame.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(SkillManager))]
    public class SkillManager : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpComponent = "Component";
        const string grpConfigs = "Configs";
        const string grpRuntime = "Runtime";
        const string grpParent = "Parent";
        const string grpVFXS = "VFXs";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpComponent),InlineEditor,ListDrawerSettings(DraggableItems = false,Expanded = true)]
        private  List<ISkill>  _skills;
        #endregion

        #region private
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            for (var i = 0; i < _skills.Count; i++)
            {
                _skills[i].Init(_gameManager);
            }

            this.LogCompleted(nameof(Init));
        }
        public ISkill GetSkill(string id){
            return _skills.Find(x => x.id == id);
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