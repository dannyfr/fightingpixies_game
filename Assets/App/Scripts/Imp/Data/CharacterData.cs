using System;
using Evesoft;
using Sirenix.OdinInspector;
using UnityEngine;
using Newtonsoft.Json;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public class CharacterData : IDisposable
    {
        #region const
        const string grpIdentifier = "identifier";
        //const string grpName = nameof(CharacterData);
        #endregion

        #region fields
        [BoxGroup(grpIdentifier)]
        public string id;

        [BoxGroup(grpIdentifier)]
        public string name;

        [BoxGroup(grpIdentifier),InlineEditor(inlineEditorMode:InlineEditorModes.LargePreview)]
        public Sprite thumbnail;

        [BoxGroup(nameof(CharacterStats)),HideLabel]
        public Stats showstat = Stats.HP;

        [BoxGroup(nameof(CharacterStats)),HideLabel]
        public CharacterStats stats;

        [BoxGroup(nameof(CharacterLimbs)),HideLabel]
        public CharacterLimbs limbs;

        [BoxGroup(nameof(CharacterAccessories)),HideLabel]
        public CharacterAccessories accessories;

        [JsonIgnore]
        [BoxGroup(nameof(CharacterSetCollections)),HideLabel]
        public CharacterSetCollections setsCollections;

        [JsonIgnore]
        [BoxGroup(nameof(CharacterLimbsCollections)),HideLabel]
        public CharacterLimbsCollections limbsCollections;

        [JsonIgnore]
        [BoxGroup(nameof(CharacterAccessoriesCollections)),HideLabel]
        public CharacterAccessoriesCollections accessoriesCollections;

        [BoxGroup(nameof(CharacterStatus)),HideLabel]
        public CharacterStatus status;

        [BoxGroup("Story"),HideLabel,TextArea(3,10)]
        public string story;
        #endregion

        #region constructor
        public CharacterData(){}
        public CharacterData(string id,
            string name,
            CharacterStats stats,
            CharacterLimbs limbs,
            CharacterAccessories accessories,
            CharacterSetCollections setCollections,
            CharacterLimbsCollections limbsCollections,
            CharacterAccessoriesCollections accessoriesCollections,
            CharacterStatus status,
            string story)
        {
            
            this.id     = id;
            this.name   = name;
            this.stats  = stats;
            this.limbs = limbs;
            this.accessories = accessories;
            this.setsCollections = setCollections;
            this.limbsCollections = limbsCollections;
            this.accessoriesCollections = accessoriesCollections;
            this.status = status;
            this.story  = story;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
           thumbnail?.Destroy();
           thumbnail = null;
        }
        #endregion

        public CharacterData Copy(){
            return (CharacterData)this.MemberwiseClone();
        }
    }
}

