using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Evesoft;
using Random = UnityEngine.Random;

namespace NFTGame.Dummy.Server
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Dummy + nameof(DatabaseDummy))]
    public class DatabaseDummy : SerializedMonoBehaviour
    {
        #region const
        const string grpConfig = "Config";
        const string grpData = "Data";
        #endregion

        #region fields
        [BoxGroup(grpConfig),SerializeField,Range(0,1000)]
        private int _pixiesCount = 30;

        [BoxGroup(grpConfig),SerializeField,InlineEditor,Required]
        private NFTGame.Config.CharacterSetConfig[] _presetsCfg;

        [BoxGroup(grpConfig),SerializeField,InlineEditor,Required]
        private NFTGame.ISkill[] _skillsCfg;

        [SerializeField,BoxGroup(grpData),TableList(IsReadOnly = true)]
        public List<DatabaseDataSkill> skills;

        [SerializeField,BoxGroup(grpData),ListDrawerSettings(IsReadOnly = true)]
        public List<DatabaseDataCharacterPreset> presets;

        [SerializeField,BoxGroup(grpData),TableList(IsReadOnly = true)]
        public List<DatabaseDataPart> parts;

        [SerializeField,BoxGroup(grpData),TableList(IsReadOnly = true)]
        public List<DatabaseDataReward> rewards;

        [SerializeField,BoxGroup(grpData),TableList(IsReadOnly = true)]
        public List<DatabaseDataHistory> history;

        [SerializeField,BoxGroup(grpData),ListDrawerSettings(IsReadOnly = true)]
        public List<DatabaseDataCharacter> pixies;
        #endregion

        #region methods
        public bool ContainPart(string id){
            return !parts.Find(x => x.id == id).IsNull();
        }
        #endregion      
    
        public void Init(ServerDummy server){
            InitPresetAndPart();
            InitSkills();
            InitPixies(server);
        }
        private void InitPresetAndPart(){
            presets?.Clear();
            parts?.Clear();

            foreach (var item in _presetsCfg)
            {
                var data = Adapter.AdapterCharacterSet.ToDataDatabase(item);
                if(!data.IsNull()){
                   presets.Add(data);
                }
                    
                if(item.head && !ContainPart(item.head.id))
                    parts.Add(new DatabaseDataPart(item.head.id,"head"));

                if(item.body && !ContainPart(item.body.id))
                    parts.Add(new DatabaseDataPart(item.body.id,"body"));

                if(item.arm && !ContainPart(item.arm.id))
                    parts.Add(new DatabaseDataPart(item.arm.id,"arm"));

                if(item.leg && !ContainPart(item.leg.id))
                    parts.Add(new DatabaseDataPart(item.leg.id,"leg"));

                if(item.helmet && !ContainPart(item.helmet.id))
                    parts.Add(new DatabaseDataPart(item.helmet.id,"helmet"));

                if(item.facialHair && !ContainPart(item.facialHair.id))
                    parts.Add(new DatabaseDataPart(item.facialHair.id,"facialhair"));
                
                if(item.cloth && !ContainPart(item.cloth.id))
                    parts.Add(new DatabaseDataPart(item.cloth.id,"cloth"));
                
                if(item.sleeve && !ContainPart(item.sleeve.id))
                    parts.Add(new DatabaseDataPart(item.sleeve.id,"sleeve"));
                
                if(item.pants && !ContainPart(item.pants.id))
                    parts.Add(new DatabaseDataPart(item.pants.id,"pants"));
                
                if(item.weapon && !ContainPart(item.weapon.id))
                    parts.Add(new DatabaseDataPart(item.weapon.id,"weapon"));
            }          
        }
        private void InitSkills(){
            skills?.Clear();

            foreach (var item in _skillsCfg)
            {
                var data = Adapter.AdapterSkill.ToDataDatabase(item);
                if(data.IsNull())
                    continue;
                
                skills.Add(data);
            }
        }
        private void InitPixies(ServerDummy server){
            pixies?.Clear();

            for (var i = 0; i < _pixiesCount; i++)
            {
                var data = Adapter.AdapterCharacterData.ToDataDatabase(server.GetRandomCharacter(1,true));
                if(data.IsNull())
                    continue;

                pixies.Add(data);
            }
        }     
    }
}