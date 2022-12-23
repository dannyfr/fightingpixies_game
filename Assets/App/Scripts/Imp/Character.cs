using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using Sirenix.OdinInspector;
using Evesoft;
using TMPro;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    public class Character : SerializedMonoBehaviour,IDisposable
    {
        public class Limbs{
            public NFTGame.Limbs.Head head;
            public NFTGame.Limbs.Body body;
            public NFTGame.Limbs.Arm  arm;
            public NFTGame.Limbs.Leg  leg;
        }

        public class Accessories{
            public NFTGame.Accessories.Cloth cloth;
            public NFTGame.Accessories.FacialHair facialHair;
            public NFTGame.Accessories.Helmet helmet;
            public NFTGame.Accessories.Pants pants;
            public NFTGame.Accessories.Sleeve sleeve;
            public NFTGame.Accessories.Weapon weapon;
        }

        #region const
        const string grpConfig = "Config";
        const string grpRuntime = "Runtime";
        const string grpData = "Data";
        const string grpComponent = "Component";
        const string grpFields = "Fields";
        #endregion
        
        #region fields
        [SerializeField,BoxGroup(grpConfig)]
        private Dictionary<Direction,Vector2> _frameOffset;

        [SerializeField,BoxGroup(grpComponent)]
        private GameObject _frame;

        [SerializeField,BoxGroup(grpComponent)]
        private SortingGroup _shortingGroup;

        [SerializeField,BoxGroup(grpComponent)]
        private TextMeshPro _textName,_textAttack,_textSpeed,_textDefense,_textHP;
        #endregion
 
        #region events
        public event Action<Character,CharacterData> onDataUpdated;
        #endregion

        #region property
        public new string name {
            get {
                return _data?.name;
            } 
            set {
                base.name = value;
                
                if(!_data.IsNull()){
                    _data.name = value;
                }
            }
        }
       
        [ShowInInspector,BoxGroup(nameof(direction)),HideLabel]
        public Direction direction => _dir;

        [ShowInInspector,BoxGroup(nameof(color)),HideLabel,ColorPalette,ReadOnly,]
        public Color color{
            get{
                return _color;
            }

            set{
                _color = value;

                if(limbs.head) 
                    limbs.head.spriteRenderer.color = value;   
                
                if(limbs.body) 
                   limbs.body.spriteRenderer.color = value; 
                
                if(limbs.arm) 
                    limbs.arm.spriteRenderer.color = value; 
                
                if(limbs.leg)
                    limbs.leg.spriteRenderer.color = value;

                if(accessories.cloth)
                    accessories.cloth.spriteRenderer.color = value;

                if(accessories.facialHair)
                    accessories.facialHair.spriteRenderer.color = value;

                if(accessories.helmet)
                    accessories.helmet.spriteRenderer.color = value;

                if(accessories.pants)
                    accessories.pants.spriteRenderer.color = value;

                if(accessories.sleeve)
                    accessories.sleeve.spriteRenderer.color = value;

                if(accessories.weapon)
                    accessories.weapon.spriteRenderer.color = value;
            }
        }
        
        [ShowInInspector,BoxGroup(nameof(limbs)),HideLabel,ReadOnly]
        public Limbs limbs => _limbs;

        [ShowInInspector,BoxGroup(nameof(accessories)),HideLabel,ReadOnly]
        public Accessories accessories =>_accessories;
        
        [ShowInInspector,BoxGroup(nameof(data)),HideLabel]
        public CharacterData data => _data;
        #endregion
        
        #region private
        private Direction _dir = Direction.Right;
        private Color _color = Color.white;
        private bool _isDirty;
        private CharacterData _data;
        private Limbs _limbs;
        private Accessories _accessories;
        private GameManager _gameManager;
        #endregion
        
        #region methods
        public void Init(GameManager gameManager,CharacterData data){
            _data  = data;
            _gameManager = gameManager;
            _limbs = new Limbs();
            _accessories = new Accessories();
            base.name = data.name;

            //Set stats
            _textName.text    = data.name;
            ShowStat();
        }

        public void SetFaceDirection(Direction dir){
            _dir = dir;
            UpdateCharacter();
        }
        public void ShowFrame(){
            _frame.Show();
        }
        public void HideFrame(){
            _frame.Hide();
        }
        public void ShowStat(Stats stats = Stats.HP | Stats.ATTACK | Stats.DEFENSE | Stats.SPEED){
           
            _textAttack.text  = (stats & Stats.ATTACK) != 0 ? data.stats.attack.ToString() : "???";
            _textDefense.text = (stats & Stats.DEFENSE) != 0 ? data.stats.defense.ToString() : "???";
            _textSpeed.text   = (stats & Stats.SPEED) != 0 ? data.stats.speed.ToString() : "???";
            _textHP.text      = (stats & Stats.HP) != 0 ? data.stats.hp.ToString(): "???";
        }
        public void Show(){
            gameObject.Show();
        }
        public void Hide(){
            gameObject.Hide();
        }
        public void BringForward(){
            _shortingGroup.sortingOrder++;
        }
        public void BringBackward(){
            _shortingGroup.sortingOrder--;
        }
        public Vector3 GetHitPosition(Vector2 offset = default(Vector2)){
            return limbs.body.transform.position + (Vector3)offset;
        }
        public Vector3 GetDamagePosition(Vector2 offset = default(Vector2)){
            return limbs.head.transform.position + (Vector3)offset;
        }  
        
        public int GetUseIndex(BodyType type){
            switch(type){
                case BodyType.Arm:{
                    return data.limbsCollections.arms.IsNullOrEmpty()? -1 : Array.IndexOf(data.limbsCollections.arms,data.limbs.armID);
                }

                case BodyType.Body:{
                    return data.limbsCollections.bodys.IsNullOrEmpty()? -1 : Array.IndexOf(data.limbsCollections.bodys,data.limbs.bodyID);
                }

                case BodyType.Head:{
                    return data.limbsCollections.heads.IsNullOrEmpty()? -1 : Array.IndexOf(data.limbsCollections.heads,data.limbs.headID);
                }

                case BodyType.Leg:{
                    return data.limbsCollections.legs.IsNullOrEmpty()? -1 :  Array.IndexOf(data.limbsCollections.legs,data.limbs.legID);
                }

                default:{
                    return -1;
                }
            }
        }
        public int GetUseIndex(AccessoriesType type){
            switch(type){

                case AccessoriesType.Cloth:{
                    return data.accessoriesCollections.cloths.IsNullOrEmpty()? -1 : Array.IndexOf(data.accessoriesCollections.cloths,data.accessories.clothID);
                }

                case AccessoriesType.FacialHair:{
                    return data.accessoriesCollections.facialHairs.IsNullOrEmpty()? -1: Array.IndexOf(data.accessoriesCollections.facialHairs,data.accessories.facialHairID);
                }

                case AccessoriesType.Helmet:{
                    return data.accessoriesCollections.helmets.IsNullOrEmpty()? -1 : Array.IndexOf(data.accessoriesCollections.helmets,data.accessories.helmetID);
                }

                case AccessoriesType.Pants:{
                    return data.accessoriesCollections.pants.IsNullOrEmpty()? -1 : Array.IndexOf(data.accessoriesCollections.pants,data.accessories.pantsID);
                }

                case AccessoriesType.Sleeve:{
                    return data.accessoriesCollections.sleeves.IsNullOrEmpty()? -1 :  Array.IndexOf(data.accessoriesCollections.sleeves,data.accessories.sleeveID); 
                }

                case AccessoriesType.Weapon:{
                    return data.accessoriesCollections.weapons.IsNullOrEmpty()? -1 : Array.IndexOf(data.accessoriesCollections.weapons,data.accessories.weaponID);
                }

                default:{
                    return -1;
                }
            }
        }
        public string GetCollectionsID(BodyType type,int index){
            switch(type){
                case BodyType.Arm:{
                    return (index < 0 || data.limbsCollections.arms.IsNullOrEmpty()) ? null : data.limbsCollections.arms[index];
                }

                case BodyType.Body:{
                    return (index < 0 || data.limbsCollections.bodys.IsNullOrEmpty()) ? null : data.limbsCollections.bodys[index];
                }

                case BodyType.Head:{
                    return (index < 0 || data.limbsCollections.heads.IsNullOrEmpty()) ? null : data.limbsCollections.heads[index];
                }

                case BodyType.Leg:{
                    return (index < 0 || data.limbsCollections.legs.IsNullOrEmpty()) ? null : data.limbsCollections.legs[index];
                }

                default:{
                    return null;
                }
            }
        }
        public string GetCollectionsID(AccessoriesType type,int index){
            switch(type){
                case AccessoriesType.Cloth:{
                    return (index < 0 || data.accessoriesCollections.cloths.IsNullOrEmpty()) ? null : data.accessoriesCollections.cloths[index];
                }

                case AccessoriesType.FacialHair:{
                    return (index < 0 || data.accessoriesCollections.facialHairs.IsNullOrEmpty()) ? null : data.accessoriesCollections.facialHairs[index];
                }

                case AccessoriesType.Helmet:{
                    return (index < 0 || data.accessoriesCollections.helmets.IsNullOrEmpty()) ? null : data.accessoriesCollections.helmets[index];
                }

                case AccessoriesType.Pants:{
                    return (index < 0 || data.accessoriesCollections.pants.IsNullOrEmpty()) ? null : data.accessoriesCollections.pants[index];
                }

                case AccessoriesType.Sleeve:{
                    return (index < 0 || data.accessoriesCollections.sleeves.IsNullOrEmpty()) ? null : data.accessoriesCollections.sleeves[index];
                }

                case AccessoriesType.Weapon:{
                    return (index < 0 || data.accessoriesCollections.weapons.IsNullOrEmpty()) ? null : data.accessoriesCollections.weapons[index];
                }

                default:{
                    return null;
                }
            }
        }
        public int GetCollectionsCount(BodyType type){
            switch(type){
                case BodyType.Arm:{
                    return data.limbsCollections.arms.IsNullOrEmpty() ? 0 : data.limbsCollections.arms.Length;
                }

                case BodyType.Body:{
                    return data.limbsCollections.bodys.IsNullOrEmpty() ? 0 : data.limbsCollections.bodys.Length;
                }

                case BodyType.Head:{
                    return data.limbsCollections.heads.IsNullOrEmpty() ? 0 : data.limbsCollections.heads.Length;
                }

                case BodyType.Leg:{
                    return data.limbsCollections.legs.IsNullOrEmpty() ? 0 : data.limbsCollections.legs.Length;
                }

                default:{
                    return -1;
                }
            }
        }
        public int GetCollectionsCount(AccessoriesType type){
            switch(type){
                case AccessoriesType.Cloth:{
                    return data.accessoriesCollections.cloths.IsNullOrEmpty()? 0 : data.accessoriesCollections.cloths.Length;
                }

                case AccessoriesType.FacialHair:{
                    return data.accessoriesCollections.facialHairs.IsNullOrEmpty()? 0 : data.accessoriesCollections.facialHairs.Length;
                }

                case AccessoriesType.Helmet:{
                    return data.accessoriesCollections.helmets.IsNullOrEmpty()? 0 : data.accessoriesCollections.helmets.Length;
                }

                case AccessoriesType.Pants:{
                    return data.accessoriesCollections.pants.IsNullOrEmpty()? 0 : data.accessoriesCollections.pants.Length;
                }

                case AccessoriesType.Sleeve:{
                    return data.accessoriesCollections.sleeves.IsNullOrEmpty()? 0 : data.accessoriesCollections.sleeves.Length;
                }

                case AccessoriesType.Weapon:{
                    return data.accessoriesCollections.weapons.IsNullOrEmpty()? 0 : data.accessoriesCollections.weapons.Length;
                }

                default:{
                    return -1;
                }
            }
        }  
        
        public void UpdateCharacter(){
            #region Setup Limbs
            //Check body
            if(!limbs.body)
                return;

            var layerName = "Character";
            var bodyOrder = 2;
            var headOrder = 5;
            var armOrder = 9;
            var legOrder = 1;
            var clothOrder = 4;
            var pantsOrder = 3;
            var helmetOrder= 7;
            var facialHairOrder = 6;
            var weaponOrder = 8;
            var sleeveOrder = 10;
            
            var scale = transform.localScale;
                scale.x = _dir == Direction.Right ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);

            limbs.body.transform.SetParent(transform);
            limbs.body.transform.localPosition = Vector2.zero;
            limbs.body.spriteRenderer.sortingLayerName = layerName;
            limbs.body.spriteRenderer.sortingOrder = bodyOrder;
            limbs.body.transform.localScale = scale;
            
            //setup head
            if(limbs.head){
                limbs.head.transform.position = limbs.body.headPlacement.position;
                limbs.head.transform.SetParent(transform);
                limbs.head.spriteRenderer.sortingLayerName = layerName;
                limbs.head.spriteRenderer.sortingOrder = headOrder;
                limbs.head.transform.localScale = scale;
            }
            
            //Arms
            if(limbs.arm){
                limbs.arm.transform.position = limbs.body.armPlacement.position;
                limbs.arm.transform.SetParent(transform);
                limbs.arm.spriteRenderer.sortingLayerName = layerName;
                limbs.arm.spriteRenderer.sortingOrder = armOrder;
                limbs.arm.transform.localScale = scale;
            }

            //legs
            if(limbs.leg){
                limbs.leg.transform.position = limbs.body.legPlacement.position;
                limbs.leg.transform.SetParent(transform);
                limbs.leg.spriteRenderer.sortingLayerName = layerName;
                limbs.leg.spriteRenderer.sortingOrder = legOrder;
                limbs.leg.transform.localScale = scale;
            }
            #endregion

            #region Accessories
            if(accessories.cloth){
                accessories.cloth.transform.position = limbs.body.transform.position;
                accessories.cloth.transform.SetParent(transform);
                accessories.cloth.spriteRenderer.sortingLayerName = layerName;
                accessories.cloth.spriteRenderer.sortingOrder = clothOrder;
                accessories.cloth.transform.localScale = scale;
            }
            if(accessories.facialHair){
                if(limbs.head.facialHairPlacement){
                    accessories.facialHair.transform.position = limbs.head.facialHairPlacement.position;
                    accessories.facialHair.transform.SetParent(transform);
                    accessories.facialHair.spriteRenderer.sortingLayerName = layerName;
                    accessories.facialHair.spriteRenderer.sortingOrder = facialHairOrder;
                    accessories.facialHair.transform.localScale = scale;
                    accessories.facialHair.gameObject.Show();
                }else{
                    accessories.facialHair.gameObject.Hide();
                }
            }
            if(accessories.helmet){
                if(limbs.head.helmetPlacement){
                    accessories.helmet.transform.position = limbs.head.helmetPlacement.position;
                    accessories.helmet.transform.SetParent(transform);
                    accessories.helmet.spriteRenderer.sortingLayerName = layerName;
                    accessories.helmet.spriteRenderer.sortingOrder = helmetOrder;
                    accessories.helmet.transform.localScale = scale;
                    accessories.helmet.gameObject.Show();
                }else{
                    accessories.helmet.gameObject.Hide();
                }
            }
            if(accessories.pants){
                accessories.pants.transform.position = limbs.leg.transform.position;
                accessories.pants.transform.SetParent(transform);
                accessories.pants.spriteRenderer.sortingLayerName = layerName;
                accessories.pants.spriteRenderer.sortingOrder = pantsOrder;
                accessories.pants.transform.localScale = scale;
            }
            if(accessories.sleeve){
                accessories.sleeve.transform.position = limbs.arm.transform.position;
                accessories.sleeve.transform.SetParent(transform);
                accessories.sleeve.spriteRenderer.sortingLayerName = layerName;
                accessories.sleeve.spriteRenderer.sortingOrder = sleeveOrder;
                accessories.sleeve.transform.localScale = scale;
            }
            if(accessories.weapon){
                if(limbs.arm.handPlacement){
                    accessories.weapon.transform.position = limbs.arm.handPlacement.position;
                    accessories.weapon.transform.SetParent(transform);
                    accessories.weapon.spriteRenderer.sortingLayerName = layerName;
                    accessories.weapon.spriteRenderer.sortingOrder = weaponOrder;
                    accessories.weapon.transform.localScale = scale;
                    accessories.weapon.gameObject.Show();
                }else{
                    accessories.weapon.gameObject.Hide();
                }
            }
            #endregion

            #region frame
            var pos = transform.position;
                pos.x += _frameOffset[_dir].x;
                pos.y += _frameOffset[_dir].y;
                
            _frame.transform.position = pos;
            #endregion
        }    
        #endregion

        #region async
        public async Task<Exception> SetInBattleAsync(){
            
            var message = "Are you sure send this pixies to battle arena?";
            var accept  =  await _gameManager.uiManager.uiMessage.Show(message);
            if(!accept)
                return null;

            //Send pixies to smart contract 
            //var address   = _gameManager.blockChainApi.account;
            var exception = await _gameManager.blockChainApi.SetToBattleAsync(uint.Parse(data.id));
            if(!exception.IsNull())
                return exception;
            
            data.status.active = true;
            onDataUpdated?.Invoke(this,data);
            return exception;
        }
        public async Task<Exception> RetrieveFromBattle(){
            var message = "Are you sure want retrieve this pixies from battle arena?";
            var accept  =  await _gameManager.uiManager.uiMessage.Show(message);
            if(!accept)
                return null;

            //Checking available for retrieve
            (var exception,var battleId) = await _gameManager.blockChainApi.GetBattleRoomOf(uint.Parse(data.id));
            if(!exception.IsNull())
                return exception;

            if(battleId == default(uint)){
                message = "Sorry pixies already finishing a battle";
                await _gameManager.uiManager.uiMessage.Show(message,false);

                //Pull characters
                _gameManager.characterManager.PullCharactersAsync();
                return null;
            }

            //Send pixies to smart contract 
            exception = await _gameManager.blockChainApi.RetrieveFromBattle(uint.Parse(data.id));
            if(!exception.IsNull())
                return exception;
            
            data.status.active = false;
            onDataUpdated?.Invoke(this,data);
            return exception;
             
        }
        #endregion
    
        #region callbacks
        private void OnDestroy() {
            onDataUpdated = null;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _data?.Dispose();
            _data = null;
            gameObject.Destroy();
        }   
        #endregion      
    }
}