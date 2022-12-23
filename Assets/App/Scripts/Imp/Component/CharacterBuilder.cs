using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Evesoft;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NFTGame.Component
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Component + nameof(CharacterBuilder))]
    public class CharacterBuilder : SerializedMonoBehaviour,IDisposable
    {       
        #region const
        const string grpComponent = "Component";
        const string grpConfig = "Config";
        const string grpLimbs = "Limbs";
        #endregion

        #region fields
        [SerializeField,InlineEditor,Required]
        private Config.CharacterBuilderConfig _config;

        // [SerializeField]
        // public AssetReference _configRef;

        [SerializeField,InlineEditor]
        private Config.StatsConfig _statsCfg;

        [SerializeField,InlineEditor]
        private Config.NameConfig _nameCfg;

        [SerializeField,InlineEditor]
        private Config.StoryConfig _storiesCfg;

        [SerializeField,BoxGroup(grpComponent),Required]
        private Character _characterPref;

        [SerializeField,BoxGroup(grpComponent),Required]
        private ThumbnailBuilder _thumbnailBuilder;
        #endregion

        #region private
        private GameManager _gameManager;
        //private int[] _setNumbers;
        #endregion
         
        #region methods
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            _characterPref.Hide();
            _thumbnailBuilder.Init();
            
            // var onComplete = default(Action<AsyncOperationHandle<Config.CharacterBuilderConfig>>);
            //     onComplete = (handle)=>{
            //         handle.Completed -= onComplete;

            //         if(!handle.OperationException.IsNull()){
            //             _configRef.LoadAssetAsync<Config.CharacterBuilderConfig>().Completed += onComplete;;
            //         }else{
            //             _config = handle.Result;
            //             $"Load CharacterBuilde Config Completed, CharacterCount : {_config.characters.Count}".Log();
            //         }
            //     };

            //_configRef.LoadAssetAsync<Config.CharacterBuilderConfig>().Completed += onComplete;
        }

        public CharacterData CreateData(Params.CharacterParams param = null){
            var id   = Guid.NewGuid().ToString();
            var name = _nameCfg.names[UnityEngine.Random.Range(0,_nameCfg.names.Count)];
            
            if(param.IsNull()){
                param = new Params.CharacterParams(true);
            }
            
            // //CharacterSets
            // var characterSetVariants = GetCharacterSetVariant(param.setsVariant,param.random);

            (var limbs ,var accessories,var setCollections,var limbsCollections,var accessoriesCollections) = GetVariant(param.setsVariant,param.random);
            var status = new CharacterStatus(){
                energy = 3,
                maxEnergy = 3,
            };
            var story = _storiesCfg.stories[UnityEngine.Random.Range(0,_storiesCfg.stories.Count)];
            
            return new CharacterData(id,name,RandomStats(),limbs,accessories,setCollections,limbsCollections,accessoriesCollections,status,story);   
        }
        public Character Create(CharacterData data){

            //Set default limbs collections
            if(data.limbsCollections.heads.IsNullOrEmpty() && !data.limbs.headID.IsNullOrEmpty()){
                data.limbsCollections.heads = new string[]{data.limbs.headID};
            }
            if(data.limbsCollections.bodys.IsNullOrEmpty() && !data.limbs.bodyID.IsNullOrEmpty()){
                data.limbsCollections.bodys = new string[]{data.limbs.bodyID};
            }
            if(data.limbsCollections.arms.IsNullOrEmpty() && !data.limbs.armID.IsNullOrEmpty()){
                data.limbsCollections.arms = new string[]{data.limbs.armID};
            }
            if(data.limbsCollections.legs.IsNullOrEmpty() && !data.limbs.legID.IsNullOrEmpty()){
                data.limbsCollections.legs = new string[]{data.limbs.legID};
            }

            //Set default accessories collections
            if(data.accessoriesCollections.cloths.IsNullOrEmpty() && !data.accessories.clothID.IsNullOrEmpty()){
                data.accessoriesCollections.cloths = new string[]{data.accessories.clothID};
            }
            if(data.accessoriesCollections.facialHairs.IsNullOrEmpty() && !data.accessories.facialHairID.IsNullOrEmpty()){
                data.accessoriesCollections.facialHairs = new string[]{data.accessories.facialHairID};
            }
            if(data.accessoriesCollections.helmets.IsNullOrEmpty() && !data.accessories.helmetID.IsNullOrEmpty()){
                data.accessoriesCollections.helmets = new string[]{data.accessories.helmetID};
            }
            if(data.accessoriesCollections.pants.IsNullOrEmpty() && !data.accessories.pantsID.IsNullOrEmpty()){
                data.accessoriesCollections.pants = new string[]{data.accessories.pantsID};
            }
            if(data.accessoriesCollections.sleeves.IsNullOrEmpty() && !data.accessories.sleeveID.IsNullOrEmpty()){
                data.accessoriesCollections.sleeves = new string[]{data.accessories.sleeveID};
            }
            if(data.accessoriesCollections.weapons.IsNullOrEmpty() && !data.accessories.weaponID.IsNullOrEmpty()){
                data.accessoriesCollections.weapons = new string[]{data.accessories.weaponID};
            }

            var character = GameObject.Instantiate(_characterPref);
                character.Init(_gameManager,data);
                character.Show();

            //Limbs
            SetLimbsHeadVariant(character,data.limbs.headID);
            SetLimbsBodyVariant(character,data.limbs.bodyID);
            SetLimbsArmVariant(character,data.limbs.armID);
            SetLimbsLegVariant(character,data.limbs.legID);

            //Accessories
            if(!character.limbs.head.facialHairPlacement.IsNull()){
                SetAccessoriesFacialHairVariant(character,data.accessories.facialHairID);
            }else{
                data.accessories.facialHairID = null;
                data.accessoriesCollections.facialHairs = null;
            }
                
            if(!character.limbs.head.helmetPlacement.IsNull()){
                SetAccessoriesHelmetVariant(character,data.accessories.helmetID);
            }else{
                data.accessories.helmetID = null;
                data.accessoriesCollections.helmets = null;
            }
                
            if(!character.limbs.arm.handPlacement.IsNull()){
                SetAccessoriesWeaponVariant(character,data.accessories.weaponID);
            }else{
                data.accessories.weaponID = null;
                data.accessoriesCollections.weapons = null;
            }
                
            SetAccessoriesClothVariant(character,data.accessories.clothID);
            SetAccessoriesPantsVariant(character,data.accessories.pantsID);
            SetAccessoriesSleeveVariant(character,data.accessories.sleeveID);

            //Create thumbnail
            if(data.thumbnail.IsNull()){
                Flatten(character);
            }

            return character;
        }
        public CharacterStats RandomStats(){
            return new CharacterStats(){
                hp      = UnityEngine.Random.Range(_statsCfg.hp.x,_statsCfg.hp.y),
                attack  = UnityEngine.Random.Range(_statsCfg.attack.x,_statsCfg.attack.y),
                defense =  UnityEngine.Random.Range(_statsCfg.defense.x,_statsCfg.defense.y),
                speed   = UnityEngine.Random.Range(_statsCfg.speed.x,_statsCfg.speed.y)
            };
        }
        public void Flatten(Character character){
            character.ShowFrame();
            character.data.thumbnail?.Destroy();
            character.data.thumbnail = _thumbnailBuilder.Create(character.gameObject).ToSprite(_config.pixelPerUnit);
            character.HideFrame();
        }

        
        #region Get Limbs Variants
        private (string,string[]) GetLimbsHeadVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.HeadConfig>(_config.headsCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetLimbsBodyVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.BodyConfig>(_config.bodysCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetLimbsArmVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.ArmConfig>(_config.armsCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetLimbsLegVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.LegConfig>(_config.legsCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        #endregion

        #region Get Accessories
        private (string,string[]) GetAccessoriesClothVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.ClothConfig>(_config.clothsCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetAccessoriesFacialHairVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.FacialHairConfig>(_config.facialHairCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetAccessoriesHelmetVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.HelmetConfig>(_config.helmetCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetAccessoriesPantsVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.PantsConfig>(_config.pantsCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetAccessoriesSleeveVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.SleeveConfig>(_config.sleevesCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        private (string,string[]) GetAccessoriesWeaponVariant(int count = -1,bool random = false){
            if(count == 0)
                return (null,null);
            
            var configs  = new List<Config.WeaponConfig>(_config.weaponsCfg);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            var index = random ? UnityEngine.Random.Range(0,variants.Length) : 0;
            return(variants[index],variants);
        }
        #endregion

        public Config.CharacterSetConfig GetSetsVariantConfig(string id)
        {
            return _config.characters.Find(x=> x.id == id);
        }
        private string[] GetCharacterSetVariant(int count = -1,bool random = false){
            if(count == 0)
                return null;
            
            var configs  = new List<Config.CharacterSetConfig>(_config.characters);
            var variants = new string[ count < 0 ? configs.Count : Mathf.Clamp(count,1,configs.Count)];

            if(random){
                configs.Shuffle();
            }
                
            for (var i = 0; i < variants.Length; i++)
            {
                variants[i] = configs[i].id;
            }

            configs.Clear();
            configs = null;
            return variants;
        }   

        private (CharacterLimbs,CharacterAccessories,CharacterSetCollections,CharacterLimbsCollections,CharacterAccessoriesCollections) GetVariant(int variants=-1,bool random = false){       
            
            variants  = variants < 0 ? _config.characters.Count : variants;
            
            var arms        = default(List<string>);
            var bodys       = default(List<string>);
            var heads       = default(List<string>);
            var legs        = default(List<string>);

            var cloths      = default(List<string>);
            var facialHairs = default(List<string>);
            var helmets     = default(List<string>);
            var pants       = default(List<string>);
            var sleeves     = default(List<string>);
            var weapons     = default(List<string>);


            var setsCollections  = new CharacterSetCollections(){
                heads = new string[variants],
                bodys = new string[variants],
                legs  = new string[variants],
            };           
            var limbs            = new CharacterLimbs();
            var accessories      = new CharacterAccessories();
            var limbsCollections = new CharacterLimbsCollections();
            var accessoriesCollections = new CharacterAccessoriesCollections();
            

            var availableSetHeads = new List<Config.CharacterSetConfig>(_config.characters);
            var availableSetBody  = new List<Config.CharacterSetConfig>(_config.characters);
            var availableSetLeg   = new List<Config.CharacterSetConfig>(_config.characters);
            var usedSets          = new List<Config.CharacterSetConfig>();

            for (var i = 0; i < variants; i++)
            {
                var headIndex = UnityEngine.Random.Range(0,availableSetHeads.Count);
                var bodyIndex = UnityEngine.Random.Range(0,availableSetBody.Count);
                var legIndex  = UnityEngine.Random.Range(0,availableSetLeg.Count);

                //Set preset
                setsCollections.heads[i] = availableSetHeads[headIndex].id;
                setsCollections.bodys[i] = availableSetBody[bodyIndex].id;
                setsCollections.legs[i]  = availableSetLeg[legIndex].id;

                //SetLimbs
                limbs.armID   = availableSetBody[bodyIndex].arm.id;
                limbs.bodyID  = availableSetBody[bodyIndex].body.id;
                limbs.headID  = availableSetHeads[headIndex].head.id;
                limbs.legID   = availableSetLeg[legIndex].leg.id;

                //Set accesories
                accessories.clothID       = availableSetBody[bodyIndex].cloth?.id;
                accessories.facialHairID  = availableSetHeads[headIndex].facialHair?.id;
                accessories.helmetID      = availableSetHeads[headIndex].helmet?.id;
                accessories.pantsID       = availableSetLeg[legIndex].pants?.id;
                accessories.sleeveID      = availableSetBody[bodyIndex].sleeve?.id;
                accessories.weaponID      = availableSetBody[bodyIndex].weapon?.id;

                if(!usedSets.Contains(availableSetHeads[headIndex]))
                    usedSets.Add(availableSetHeads[headIndex]);

                if(!usedSets.Contains(availableSetBody[bodyIndex]))
                    usedSets.Add(availableSetBody[bodyIndex]);

                if(!usedSets.Contains(availableSetLeg[legIndex]))
                    usedSets.Add(availableSetLeg[legIndex]);

                availableSetHeads.RemoveAt(headIndex);
                availableSetBody.RemoveAt(bodyIndex);
                availableSetLeg.RemoveAt(legIndex);
            }
           
            for (var i = 0; i < usedSets.Count; i++)
            {       
                //Limbs
                if(!usedSets[i].arm.IsNull() && !usedSets[i].arm.id.IsNullOrEmpty()){
                    if(arms.IsNull())
                        arms = new List<string>();

                    if(!arms.Contains(usedSets[i].arm.id))
                        arms?.Add(usedSets[i].arm.id);
                }
                    
                if(!usedSets[i].body.IsNull() && !usedSets[i].body.id.IsNullOrEmpty()){
                    if(bodys.IsNull())
                        bodys = new List<string>();

                    if(!bodys.Contains(usedSets[i].body.id))
                        bodys?.Add(usedSets[i].body.id);
                }
                    
                if(!usedSets[i].head.IsNull() && !usedSets[i].head.id.IsNullOrEmpty()){
                    if(heads.IsNull())
                        heads = new List<string>();
                
                    if(!heads.Contains(usedSets[i].head.id))
                        heads?.Add(usedSets[i].head.id);
                }
                    
                if(!usedSets[i].leg.IsNull() && !usedSets[i].leg.id.IsNullOrEmpty()){
                    if(legs.IsNull())
                        legs = new List<string>();

                    if(!legs.Contains(usedSets[i].leg.id))
                        legs?.Add(usedSets[i].leg.id);
                }
                    

                //Accessories
                if(!usedSets[i].cloth.IsNull() && !usedSets[i].cloth.id.IsNullOrEmpty()){
                    if(cloths.IsNull())
                        cloths = new List<string>();
                
                    if(!cloths.Contains(usedSets[i].cloth.id))
                        cloths?.Add(usedSets[i].cloth.id);
                }
                    
                if(!usedSets[i].facialHair.IsNull() && !usedSets[i].facialHair.id.IsNullOrEmpty()){
                    if(facialHairs.IsNull())
                        facialHairs = new List<string>();

                    if(!facialHairs.Contains(usedSets[i].facialHair.id))
                        facialHairs?.Add(usedSets[i].facialHair.id);
                }
                   
                if(!usedSets[i].helmet.IsNull() && !usedSets[i].helmet.id.IsNullOrEmpty()){
                    if(helmets.IsNull())
                        helmets = new List<string>();

                    if(!helmets.Contains(usedSets[i].helmet.id))
                        helmets?.Add(usedSets[i].helmet.id);
                }
                    
                if(!usedSets[i].pants.IsNull() && !usedSets[i].pants.id.IsNullOrEmpty()){
                    if(pants.IsNull())
                        pants = new List<string>();

                    if(!pants.Contains(usedSets[i].pants.id))
                        pants?.Add(usedSets[i].pants.id);
                }
                    
                if(!usedSets[i].sleeve.IsNull() && !usedSets[i].sleeve.id.IsNullOrEmpty()){
                    if(sleeves.IsNull())
                        sleeves = new List<string>();

                    if(!sleeves.Contains(usedSets[i].sleeve.id))
                        sleeves?.Add(usedSets[i].sleeve.id);
                }
                    
                if(!usedSets[i].weapon.IsNull() && !usedSets[i].weapon.id.IsNullOrEmpty()){
                    if(weapons.IsNull())
                        weapons = new List<string>();

                    if(!weapons.Contains(usedSets[i].weapon.id))
                        weapons?.Add(usedSets[i].weapon.id);
                }
            
            }

                       
            limbsCollections.arms = arms?.ToArray();
            limbsCollections.bodys = bodys?.ToArray();
            limbsCollections.heads = heads?.ToArray();
            limbsCollections.legs = legs?.ToArray();

            accessoriesCollections.cloths = cloths?.ToArray();
            accessoriesCollections.facialHairs = facialHairs?.ToArray();
            accessoriesCollections.helmets = helmets?.ToArray();
            accessoriesCollections.pants = pants?.ToArray();
            accessoriesCollections.sleeves = sleeves?.ToArray();
            accessoriesCollections.weapons = weapons?.ToArray();
            
            cloths?.Clear();
            facialHairs?.Clear();
            helmets?.Clear();
            pants?.Clear();
            sleeves?.Clear();
            weapons?.Clear();

            cloths = null;
            facialHairs = null;
            helmets = null;
            pants = null;
            sleeves = null;
            weapons = null;

            return(limbs,accessories,setsCollections,limbsCollections,accessoriesCollections);
        }     
        
        private void SetLimbsHeadVariant(Character character,Config.HeadConfig config){
            character.limbs.head?.Dispose();
            character.limbs.head = null;
            character.limbs.head = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.limbs.headID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetLimbsBodyVariant(Character character,Config.BodyConfig config){ 
            character.limbs.body?.Dispose();
            character.limbs.body = null;
            character.limbs.body = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.limbs.bodyID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetLimbsArmVariant(Character character,Config.ArmConfig config){
            character.limbs.arm?.Dispose();
            character.limbs.arm = null;
            character.limbs.arm = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.limbs.armID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetLimbsLegVariant(Character character,Config.LegConfig config){
            character.limbs.leg?.Dispose();
            character.limbs.leg = null;
            character.limbs.leg = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.limbs.legID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        
        private void SetAccessoriesClothVariant(Character character,Config.ClothConfig config){
            character.accessories.cloth?.Dispose();
            character.accessories.cloth = null;
            character.accessories.cloth = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.accessories.clothID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetAccessoriesFacialHairVariant(Character character,Config.FacialHairConfig config){
            character.accessories.facialHair?.Dispose();
            character.accessories.facialHair = null;
            character.accessories.facialHair = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.accessories.facialHairID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetAccessoriesHelmetVariant(Character character,Config.HelmetConfig config){
            character.accessories.helmet?.Dispose();
            character.accessories.helmet = null;
            character.accessories.helmet = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.accessories.helmetID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetAccessoriesPantsVariant(Character character,Config.PantsConfig config){
            character.accessories.pants?.Dispose();
            character.accessories.pants = null;
            character.accessories.pants = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.accessories.pantsID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetAccessoriesSleeveVariant(Character character,Config.SleeveConfig config){
            character.accessories.sleeve?.Dispose();
            character.accessories.sleeve = null;
            character.accessories.sleeve = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.accessories.sleeveID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }
        private void SetAccessoriesWeaponVariant(Character character,Config.WeaponConfig config){
            character.accessories.weapon?.Dispose();
            character.accessories.weapon = null;
            character.accessories.weapon = config.IsNull()? null : GameObject.Instantiate(config.pref);
            character.data.accessories.weaponID = config.IsNull()? null : config.id;
            //UpdateCharacter(character);
            character.UpdateCharacter();
        }

        private void SetAccessoriesClothVariant(Character character,string id){
            if(!character.accessories.cloth.IsNull() && character.data.accessories.clothID == id)
                return;

            var config = _config.clothsCfg.Find(x=> x.id == id);
            SetAccessoriesClothVariant(character,config);
        }
        private void SetAccessoriesFacialHairVariant(Character character,string id){
            if(!character.accessories.facialHair.IsNull() && character.data.accessories.facialHairID == id)
                return;

            var config = _config.facialHairCfg.Find(x=> x.id == id);
            SetAccessoriesFacialHairVariant(character,config);
        }
        private void SetAccessoriesHelmetVariant(Character character,string id){
            if(!character.accessories.helmet.IsNull() && character.data.accessories.helmetID == id)
                return;

            var config = _config.helmetCfg.Find(x=> x.id == id);
            SetAccessoriesHelmetVariant(character,config);
        }
        private void SetAccessoriesPantsVariant(Character character,string id){
            if(!character.accessories.pants.IsNull() && character.data.accessories.pantsID == id)
                return;

            var config = _config.pantsCfg.Find(x=> x.id == id);
            SetAccessoriesPantsVariant(character,config);
        }
        private void SetAccessoriesSleeveVariant(Character character,string id){
            if(!character.accessories.sleeve.IsNull() && character.data.accessories.sleeveID == id)
                return;

            var config = _config.sleevesCfg.Find(x=> x.id == id);
            SetAccessoriesSleeveVariant(character,config);
        }
        private void SetAccessoriesWeaponVariant(Character character,string id){
            if(!character.accessories.weapon.IsNull() && character.data.accessories.weaponID == id)
                return;

            var config = _config.weaponsCfg.Find(x=> x.id == id);
            SetAccessoriesWeaponVariant(character,config);
        }

        private void SetLimbsHeadVariant(Character character,string id){
            if(!character.limbs.head.IsNull() && character.data.limbs.headID == id)
                return;

            var config = _config.headsCfg.Find(x=> x.id == id);
                config = config.IsNull()? _config.headsCfg.First() : config;

            SetLimbsHeadVariant(character,config);
        }
        private void SetLimbsBodyVariant(Character character,string id){ 
            if(!character.limbs.body.IsNull() && character.data.limbs.bodyID == id)
                return;

            var config = _config.bodysCfg.Find(x=> x.id == id);
                config = config.IsNull()? _config.bodysCfg.First() : config;

            SetLimbsBodyVariant(character,config);
        }
        private void SetLimbsArmVariant(Character character,string id){
            if(/*!character.lArm.IsNull() && */!character.limbs.arm.IsNull() && character.data.limbs.armID == id)
                return;

            var config = _config.armsCfg.Find(x=> x.id == id);
                config = config.IsNull()? _config.armsCfg.First() : config;

            SetLimbsArmVariant(character,config);
        }
        private void SetLimbsLegVariant(Character character,string id){
            if(!character.limbs.leg.IsNull() && character.data.limbs.legID == id)
                return;

            var config = _config.legsCfg.Find(x=> x.id == id);
                config = config.IsNull()? _config.legsCfg.First() : config;

            SetLimbsLegVariant(character,config);
        }
        
        public void SetLimbsVariant(Character character,BodyType type,string id){
            switch(type){
                case BodyType.Arm:{
                    SetLimbsArmVariant(character,id);
                    break;
                }

                case BodyType.Body:{
                    SetLimbsBodyVariant(character,id);
                    break;
                }

                case BodyType.Head:{
                    SetLimbsHeadVariant(character,id);
                    break;
                }

                case BodyType.Leg:{
                    SetLimbsLegVariant(character,id);
                    break;
                }
            }
        }
        public void SetAccessoriesVariant(Character character,AccessoriesType type,string id){
            switch(type){
                case AccessoriesType.Cloth:{
                    SetAccessoriesClothVariant(character,id);
                    break;
                }

                case AccessoriesType.FacialHair:{
                    SetAccessoriesFacialHairVariant(character,id);
                    break;
                }

                case AccessoriesType.Helmet:{
                    SetAccessoriesHelmetVariant(character,id);
                    break;
                }

                case AccessoriesType.Pants:{
                    SetAccessoriesPantsVariant(character,id);
                    break;
                }

                case AccessoriesType.Sleeve:{
                    SetAccessoriesSleeveVariant(character,id);
                    break;
                }

                case AccessoriesType.Weapon:{
                    SetAccessoriesWeaponVariant(character,id);
                    break;
                }
            }
        }

        // private void UpdateCharacter(Character character){
        //     #region Setup Limbs
        //     //Check body
        //     if(!character.limbs.body)
        //         return;

        //     var layerName = "Character";
        //     var bodyOrder = 1;
        //     var headOrder = 2;
        //     var armOrder = 3;
        //     var legOrder = 1;

        //     character.limbs.body.transform.SetParent(character.transform);
        //     character.limbs.body.transform.localPosition = Vector2.zero;
        //     character.limbs.body.spriteRenderer.sortingLayerName = layerName;
        //     character.limbs.body.spriteRenderer.sortingOrder = bodyOrder;
            
        //     //setup head
        //     if(character.limbs.head){
        //         character.limbs.head.transform.position = character.limbs.body.headPlacement.position;
        //         character.limbs.head.transform.SetParent(character.transform);
        //         character.limbs.head.spriteRenderer.sortingLayerName = layerName;
        //         character.limbs.head.spriteRenderer.sortingOrder = headOrder;
        //     }
            
        //     //Arms
        //     if(character.limbs.arm){
        //         character.limbs.arm.transform.position = character.limbs.body.armPlacement.position;
        //         character.limbs.arm.transform.SetParent(character.transform);
        //         character.limbs.arm.spriteRenderer.sortingLayerName = layerName;
        //         character.limbs.arm.spriteRenderer.sortingOrder = armOrder;
        //     }

        //     //legs
        //     if(character.limbs.leg){
        //         character.limbs.leg.transform.position = character.limbs.body.legPlacement.position;
        //         character.limbs.leg.transform.SetParent(character.transform);
        //         character.limbs.leg.spriteRenderer.sortingLayerName = layerName;
        //         character.limbs.leg.spriteRenderer.sortingOrder = legOrder;
        //     }
        //     #endregion

        //     #region Accessories
        //     if(character.accessories.cloth){
        //         character.accessories.cloth.transform.position = character.limbs.body.transform.position;
        //         character.accessories.cloth.transform.SetParent( character.limbs.body.transform);
        //         character.accessories.cloth.spriteRenderer.sortingLayerName = layerName;
        //         character.accessories.cloth.spriteRenderer.sortingOrder = bodyOrder;
        //     }
        //     if(character.accessories.facialHair){
        //         if(character.limbs.head.facialHairPlacement){
        //             character.accessories.facialHair.transform.position = character.limbs.head.facialHairPlacement.position;
        //             character.accessories.facialHair.transform.SetParent(character.limbs.head.facialHairPlacement);
        //             character.accessories.facialHair.spriteRenderer.sortingLayerName = layerName;
        //             character.accessories.facialHair.spriteRenderer.sortingOrder = headOrder;
        //             character.accessories.facialHair.gameObject.Show();
        //         }else{
        //             character.accessories.facialHair.gameObject.Hide();
        //         }
        //     }
        //     if(character.accessories.helmet){
        //         if(character.limbs.head.helmetPlacement){
        //             character.accessories.helmet.transform.position = character.limbs.head.helmetPlacement.position;
        //             character.accessories.helmet.transform.SetParent(character.limbs.head.helmetPlacement);
        //             character.accessories.helmet.spriteRenderer.sortingLayerName = layerName;
        //             character.accessories.helmet.spriteRenderer.sortingOrder = headOrder;
        //             character.accessories.helmet.gameObject.Show();
        //         }else{
        //             character.accessories.helmet.gameObject.Hide();
        //         }
        //     }
        //     if(character.accessories.pants){
        //         character.accessories.pants.transform.position = character.limbs.leg.transform.position;
        //         character.accessories.pants.transform.SetParent(character.limbs.leg.transform);
        //         character.accessories.pants.spriteRenderer.sortingLayerName = layerName;
        //         character.accessories.pants.spriteRenderer.sortingOrder = legOrder;
        //     }
        //     if(character.accessories.sleeve){
        //         character.accessories.sleeve.transform.position = character.limbs.arm.transform.position;
        //         character.accessories.sleeve.transform.SetParent(character.limbs.arm.transform);
        //         character.accessories.sleeve.spriteRenderer.sortingLayerName = layerName;
        //         character.accessories.sleeve.spriteRenderer.sortingOrder = armOrder;
        //     }
        //     if(character.accessories.weapon){
        //         if(character.limbs.arm.handPlacement){
        //             character.accessories.weapon.transform.position = character.limbs.arm.handPlacement.position;
        //             character.accessories.weapon.transform.SetParent(character.transform);
        //             character.accessories.weapon.spriteRenderer.sortingLayerName = layerName;
        //             character.accessories.weapon.spriteRenderer.sortingOrder = armOrder;
        //             character.accessories.weapon.gameObject.Show();
        //         }else{
        //             character.accessories.weapon.gameObject.Hide();
        //         }
        //     }
        //     #endregion
        // }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            gameObject.Destroy();
        }  
        #endregion
    }
}

