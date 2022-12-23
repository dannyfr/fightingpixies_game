using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Evesoft;
using Random = UnityEngine.Random;
using NFTGame.Dummy.Adapter;
namespace NFTGame.Dummy.Server
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Dummy + nameof(ServerDummy))]
    public class ServerDummy : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpRequired = "Required";
        #endregion

        #region fields
        [TabGroup(grpConfig),SerializeField,InlineEditor,Required]
        private NFTGame.Config.NameConfig _namesCfg;

        [TabGroup(grpConfig),SerializeField,InlineEditor,Required]
        private NFTGame.Config.StoryConfig _storyCfg;

        [TabGroup(grpConfig),SerializeField,InlineEditor,Required]
        private NFTGame.Config.StatsConfig _statsCfg;
        
        [SerializeField,BoxGroup(grpRequired),Required]
        private DatabaseDummy _database;

        [SerializeField,BoxGroup(grpRequired),Required]
        private BlockChain.SmartContractDummy _smartContract;
        #endregion

        #region methods
        public void Init(){
            _database.Init(this);
            _smartContract.Init(_database);
            _smartContract.onBattleStart += OnBattleStart;
            _smartContract.onClaim += OnClaim;
        }
        public void AddCharacter(DatabaseDataCharacter data){
            if(data.IsNull())
                return;

            //Remove Duplicate Pixies
            var pixies = _database.pixies.Find(x => x.id == data.id);
            if(!pixies.IsNull()){
                _database.pixies.Remove(pixies);
            }

            //Add new one
            _database.pixies.Add(data);
        }
        public void ClaimReward(string rewardId){
            if(rewardId.IsNullOrEmpty())
                return;

            var reward = _database.rewards.Find(x => x.id == rewardId);
            if(reward.IsNull())
                return;

            reward.claimed = true;
        }
        public void AddHistory(string roomId,DateTime date,BattleStatus status,string winner,DatabaseDataParticipant[] participant,DatabaseDataBattleSimulation simulation){
            //Check null
            if(roomId.IsNullOrEmpty() || participant.IsNullOrEmpty() || winner.IsNullOrEmpty())
                return;

            var id = (_database.history.Count + 1).ToString();
            var history = new DatabaseDataHistory(id,date,roomId,status,winner,participant,simulation);
            _database.history.Add(history);

            //UpdateDatabase
            for (var i = 0; i < history.participant.Length; i++)
            {
                var pixies = _database.pixies.Find(x=> x.id == history.participant[i].tokenId);
                    pixies.battleCount++;
                
                if(pixies.id == history.winner){
                    pixies.win++;
                }else{
                    pixies.lose++;
                }

                pixies.winRate = (pixies.win/(float)pixies.battleCount) * 100;
            }
        }
        public DatabaseDataHistory[] GetHistory(string address){
            if(address.IsNullOrEmpty())
                return null;

            if(_database.history.IsNullOrEmpty())
                return null;

            var result = new List<DatabaseDataHistory>();
            for (var i = 0; i < _database.history.Count; i++)
            {
                var history = _database.history[i];
                for (var j = 0; j < history.participant.Length; j++)
                {
                    var participant = _database.history[i].participant[j];
                    if(participant.address != address)
                        continue;

                    result.Add(history);
                    break;
                }
            }

            return result.ToArray();
        }
        public DatabaseDataReward[] GetRewards(string address){
            if(address.IsNullOrEmpty())
                return null;

            var allrewards = _database.rewards.FindAll(x => x.address == address);
            var result = default(DatabaseDataReward[]);

            //NewUser
            if(allrewards.IsNullOrEmpty()){
                var variants = new int[]{1,1,5};
                result = new DatabaseDataReward[variants.Length]; 
                for (var i = 0; i < variants.Length; i++)
                {
                    result[i] = new DatabaseDataReward(){
                        id      = (_database.rewards.Count + 1).ToString(),
                        address = address,
                        data    = GetRandomCharacter(variants[i],true),
                        claimed = false
                    };
                     
                    _database.rewards.Add(result[i]);
                }
            }else{
                result = allrewards.FindAll(x => x.claimed == false).ToArray();
            }

            return result;
        }       
        public CharacterData GetRandomCharacter(int variants=-1,bool random = false){    
            
            variants        = variants < 0 ? _database.presets.Count : variants;
            
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

            var statValues     = Enum.GetValues(typeof(Stats));
            var randomShowStat = (Stats)statValues.GetValue(UnityEngine.Random.Range(0,statValues.Length));

            var character   = new CharacterData(){
                name        = _namesCfg.names[Random.Range(0,_namesCfg.names.Count)],
                showstat    = randomShowStat,
                stats       = new CharacterStats(){
                    hp      = Random.Range(_statsCfg.hp.x,_statsCfg.hp.y),
                    attack  = Random.Range(_statsCfg.attack.x,_statsCfg.attack.y),
                    defense = Random.Range(_statsCfg.defense.x,_statsCfg.defense.y),
                    speed   = Random.Range(_statsCfg.speed.x,_statsCfg.speed.y)
                },          
                limbs       = new CharacterLimbs(),
                accessories = new CharacterAccessories(),
                setsCollections  = new CharacterSetCollections(){
                    heads = new string[variants],
                    bodys = new string[variants],
                    legs  = new string[variants], 
                },
                limbsCollections = new CharacterLimbsCollections(),
                accessoriesCollections = new CharacterAccessoriesCollections(),
                status = new CharacterStatus(){
                    energy = 3,
                    maxEnergy = 3
                },
                story = _storyCfg.stories[Random.Range(0,_storyCfg.stories.Count)]
            };        
           
            var availableSetHeads = new List<DatabaseDataCharacterPreset>(_database.presets);
            var availableSetBody  = new List<DatabaseDataCharacterPreset>(_database.presets);
            var availableSetLeg   = new List<DatabaseDataCharacterPreset>(_database.presets);
            var usedSets          = new List<DatabaseDataCharacterPreset>();

            for (var i = 0; i < variants; i++)
            {
                var headIndex = Random.Range(0,availableSetHeads.Count);
                var bodyIndex = Random.Range(0,availableSetBody.Count);
                var legIndex  = Random.Range(0,availableSetLeg.Count);

                //Set Preset
                character.setsCollections.heads[i] = availableSetHeads[headIndex].id;
                character.setsCollections.bodys[i] = availableSetBody[bodyIndex].id;
                character.setsCollections.legs[i]  = availableSetLeg[legIndex].id;

                //Set Limbs
                character.limbs.armID   = availableSetBody[bodyIndex].armid;
                character.limbs.bodyID  = availableSetBody[bodyIndex].bodyid;
                character.limbs.headID  = availableSetHeads[headIndex].headid;
                character.limbs.legID   = availableSetLeg[legIndex].legid;

                //Set accesories
                character.accessories.clothID       = availableSetBody[bodyIndex].clothid;
                character.accessories.facialHairID  = availableSetHeads[headIndex].facialHairid;
                character.accessories.helmetID      = availableSetHeads[headIndex].helmetid;
                character.accessories.pantsID       = availableSetLeg[legIndex].pantsid;
                character.accessories.sleeveID      = availableSetBody[bodyIndex].sleeveid;
                character.accessories.weaponID      = availableSetBody[bodyIndex].weaponid;

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
                if(!usedSets[i].armid.IsNullOrEmpty()){
                    if(arms.IsNull())
                        arms = new List<string>();

                    if(!arms.Contains(usedSets[i].armid))
                        arms?.Add(usedSets[i].armid);
                }
                    
                if(!usedSets[i].bodyid.IsNullOrEmpty()){
                    if(bodys.IsNull())
                        bodys = new List<string>();

                    if(!bodys.Contains(usedSets[i].bodyid))
                        bodys?.Add(usedSets[i].bodyid);
                }
                    
                if(!usedSets[i].headid.IsNullOrEmpty()){
                    if(heads.IsNull())
                        heads = new List<string>();
                
                    if(!heads.Contains(usedSets[i].headid))
                        heads?.Add(usedSets[i].headid);
                }
                    
                if(!usedSets[i].legid.IsNullOrEmpty()){
                    if(legs.IsNull())
                        legs = new List<string>();

                    if(!legs.Contains(usedSets[i].legid))
                        legs?.Add(usedSets[i].legid);
                }
                    

                //Accessories
                if(!usedSets[i].clothid.IsNullOrEmpty()){
                    if(cloths.IsNull())
                        cloths = new List<string>();
                
                    if(!cloths.Contains(usedSets[i].clothid))
                        cloths?.Add(usedSets[i].clothid);
                }
                    
                if(!usedSets[i].facialHairid.IsNullOrEmpty()){
                    if(facialHairs.IsNull())
                        facialHairs = new List<string>();

                    if(!facialHairs.Contains(usedSets[i].facialHairid))
                        facialHairs?.Add(usedSets[i].facialHairid);
                }
                   
                if(!usedSets[i].helmetid.IsNullOrEmpty()){
                    if(helmets.IsNull())
                        helmets = new List<string>();

                    if(!helmets.Contains(usedSets[i].helmetid))
                        helmets?.Add(usedSets[i].helmetid);
                }
                    
                if(!usedSets[i].pantsid.IsNullOrEmpty()){
                    if(pants.IsNull())
                        pants = new List<string>();

                    if(!pants.Contains(usedSets[i].pantsid))
                        pants?.Add(usedSets[i].pantsid);
                }
                    
                if(!usedSets[i].sleeveid.IsNullOrEmpty()){
                    if(sleeves.IsNull())
                        sleeves = new List<string>();

                    if(!sleeves.Contains(usedSets[i].sleeveid))
                        sleeves?.Add(usedSets[i].sleeveid);
                }
                    
                if(!usedSets[i].weaponid.IsNullOrEmpty()){
                    if(weapons.IsNull())
                        weapons = new List<string>();

                    if(!weapons.Contains(usedSets[i].weaponid))
                        weapons?.Add(usedSets[i].weaponid);
                }
            
            }

            character.limbsCollections.arms = arms?.ToArray();
            character.limbsCollections.bodys = bodys?.ToArray();
            character.limbsCollections.heads = heads?.ToArray();
            character.limbsCollections.legs = legs?.ToArray();

            character.accessoriesCollections.cloths = cloths?.ToArray();
            character.accessoriesCollections.facialHairs = facialHairs?.ToArray();
            character.accessoriesCollections.helmets = helmets?.ToArray();
            character.accessoriesCollections.pants = pants?.ToArray();
            character.accessoriesCollections.sleeves = sleeves?.ToArray();
            character.accessoriesCollections.weapons = weapons?.ToArray();
            
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

            return character;
        }            
        public CharacterData[] GetCharacters(IList<string> ids){
            var dbData = _database.pixies.FindAll(x => ids.Contains(x.id));
            return dbData?.ToData();
        }     
        public string[] GetCharactersName(IList<string> ids){
            var dbData = _database.pixies.FindAll(x => ids.Contains(x.id));
            if(dbData.IsNullOrEmpty())
                return default(string[]);

            var result = new string[dbData.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = dbData[i].name;
            }
            return result;
        }
        public DatabaseDataBattleSimulation GetBattleSimulation(string battleId){
            return _database.history.Find(x=> x.room == battleId).simulation;
        }   
        private DatabaseDataBattleSimulation GetBattleSimulation(string pixies1Id,string pixies2Id){
            //Get Data from Database
            var char1 = _database.pixies.Find(x=> x.id == pixies1Id);
            var char2 = _database.pixies.Find(x=> x.id == pixies2Id);

            //check empty character
            if(char1.IsNull() || char2.IsNull()){
                "Some Character is not exist".LogError();
                return null;
            }
                
            var multiply = 1f;
            var hp1 = char1.hp * multiply;
            var hp2 = char2.hp * multiply;
            var result = new DatabaseDataBattleSimulation(char1.id,char2.id);

            var cache1 = char1.speed;
            var cache2 = char2.speed;
            var steps  = new List<DatabaseDataBattleStep>();

            while(hp1 > 0 && hp2 > 0){
                var skill = GetRandomSkill();
                var cache = Random.Range(0,cache1 + cache2); 
                var cooldDown = Random.Range(skill.mincooldown,skill.maxcooldown);
                //Char 1
                if(cache <= cache1){
                    var damage = GetDamage(skill,char1,char2);
                    steps.Add(new DatabaseDataBattleStep(char1.id,skill.id,damage,cooldDown));
                    hp2 -= damage;
                }
                //Char 2
                else{
                    var damage = GetDamage(skill,char2,char1);
                    steps.Add(new DatabaseDataBattleStep(char2.id,skill.id,damage,cooldDown));
                    hp1 -= damage;
                }

                result.winID  = hp1 < 0? char2.id:char1.id;
                result.loseID = hp1 < 0? char1.id:char2.id;
            }
            
            result.steps = steps.ToArray();
            return result;
        }
        private DatabaseDataSkill GetRandomSkill(){
            return _database.skills[Random.Range(0,_database.skills.Count)];
        }
        private float GetDamage(DatabaseDataSkill skill,DatabaseDataCharacter from,DatabaseDataCharacter to){
            return Mathf.Clamp(from.attack - to.defense,skill.mindamage,skill.maxdamage);
        }   
        #endregion

        #region callbacks
        private void OnDestroy() {
            if(_smartContract){
                _smartContract.onBattleStart -= OnBattleStart; 
            }
        }
        private void OnBattleStart(uint battleId, string address1, string address2, uint tokenId1, uint tokenId2)
        {
            //Calculation Battle
            var participant = new DatabaseDataParticipant[]{
                new DatabaseDataParticipant(address1,tokenId1.ToString()),
                new DatabaseDataParticipant(address2,tokenId2.ToString())
            };
            var simulation  = GetBattleSimulation(participant[0].tokenId,participant[1].tokenId);
            
            //SetWinnerToSmartContract
            _smartContract.SetWinner(battleId,uint.Parse(simulation.winID));

            //Add History
            var date = DateTime.Now;
            AddHistory(battleId.ToString(),date,BattleStatus.FINISH,simulation.winID,participant,simulation);
        }
        private void OnClaim(uint battleId)
        {
            //Update claimed
            var history = _database.history.Find(x => x.room == battleId.ToString());
            if(history.IsNull())
                return;

            history.claimed = true;
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


