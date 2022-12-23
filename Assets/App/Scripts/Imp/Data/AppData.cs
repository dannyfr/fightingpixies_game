using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Data + nameof(AppData),fileName = nameof(AppData))]
    public class AppData : SerializedScriptableObject
    {
        #region fields
        [ShowInInspector,NonSerialized,ReadOnly,ListDrawerSettings(Expanded = true,ListElementLabelName = "name")]
        public List<CharacterData> characters;
        #endregion

        #region constructor
        public AppData(){
            characters = new List<CharacterData>();
        }
        #endregion
    }
}