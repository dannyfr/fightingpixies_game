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
    public class UICreatorType : SerializedMonoBehaviour,IDisposable
    {
        #region const
        const string grpConfig = "Config";
        const string grpComponent = "Component";
        const string grpRuntime = "Runtime";
        #endregion

        #region field
        [SerializeField,BoxGroup(grpConfig),ListDrawerSettings(DraggableItems = false,Expanded = true)]
        private BodyType _bodyType;

        [SerializeField,BoxGroup(grpComponent)]
        private UIButton _btnPrev,_btnNext;

        [SerializeField,BoxGroup(grpComponent)]
        private TextMeshProUGUI _textLabel;
        #endregion

        #region events
        public event Action<UICreatorType> onChangeType;
        #endregion

        #region private
        private int _index;
        private int _max;
        private Character _character;
        private GameManager _gameManager;
        #endregion

        #region methods
        public void Init(GameManager gameManager)
        { 
            _gameManager = gameManager;
            // _btnPrev.onClick.AddListener(Prev);
            // _btnNext.onClick.AddListener(Next);  

            _btnPrev.OnClick.OnTrigger.Event.AddListener(Prev);
            _btnNext.OnClick.OnTrigger.Event.AddListener(Next);  
        }
        public void SetCharacter(Character character){
            _character = character;
            _index     = 0;

            switch(_bodyType){
                case BodyType.Head:{
                    _max = _character.data.setsCollections.heads.Length;
                    break;
                }
                case BodyType.Body:{
                    _max = _character.data.setsCollections.bodys.Length;
                    break;
                }
                case BodyType.Leg:{
                    _max = _character.data.setsCollections.legs.Length;
                    break;
                }
            }

            Refresh();
        }
        private void Refresh(){
            _index = Mathf.Clamp(_index,0,_max-1);
            
            _textLabel.text = $"Type {_index + 1}";

            switch(_bodyType){
                case BodyType.Head:{
                    var sets = _gameManager.characterManager.characterBuilder.GetSetsVariantConfig(_character.data.setsCollections.heads[_index]);
                    _gameManager.characterManager.characterBuilder.SetLimbsVariant(_character,BodyType.Head,sets.head.id);
                    _gameManager.characterManager.characterBuilder.SetAccessoriesVariant(_character,AccessoriesType.FacialHair,sets.facialHair?.id);
                    _gameManager.characterManager.characterBuilder.SetAccessoriesVariant(_character,AccessoriesType.Helmet,sets.helmet?.id);
                    break;
                }
                case BodyType.Body:{
                    var sets = _gameManager.characterManager.characterBuilder.GetSetsVariantConfig(_character.data.setsCollections.bodys[_index]);
                    _gameManager.characterManager.characterBuilder.SetLimbsVariant(_character,BodyType.Body,sets.body.id);
                    _gameManager.characterManager.characterBuilder.SetLimbsVariant(_character,BodyType.Arm,sets.arm.id);
                    _gameManager.characterManager.characterBuilder.SetAccessoriesVariant(_character,AccessoriesType.Cloth,sets.cloth?.id);
                    _gameManager.characterManager.characterBuilder.SetAccessoriesVariant(_character,AccessoriesType.Sleeve,sets.sleeve?.id);
                    _gameManager.characterManager.characterBuilder.SetAccessoriesVariant(_character,AccessoriesType.Weapon,sets.weapon?.id);
                    break;
                }

                case BodyType.Leg:{
                    var sets = _gameManager.characterManager.characterBuilder.GetSetsVariantConfig(_character.data.setsCollections.legs[_index]);
                    _gameManager.characterManager.characterBuilder.SetLimbsVariant(_character,BodyType.Leg,sets.leg.id);
                    _gameManager.characterManager.characterBuilder.SetAccessoriesVariant(_character,AccessoriesType.Pants,sets.pants?.id);
                    break;
                }
            }
        
            _btnNext.gameObject.SetActive(_index < _max-1);
            _btnPrev.gameObject.SetActive(_index > 0);

            onChangeType?.Invoke(this);
        }
        public void Apply(){
            switch(_bodyType){
                case BodyType.Head:{
                    _character.data.setsCollections.heads = new string[]{_character.data.setsCollections.heads[_index]};
                    _character.data.limbsCollections.heads = new string[]{_character.data.limbs.headID};
                    _character.data.accessoriesCollections.facialHairs = new string[]{_character.data.accessories.facialHairID};
                    _character.data.accessoriesCollections.helmets = new string[]{_character.data.accessories.helmetID}; 
                    break;
                }
                case BodyType.Body:{
                    _character.data.setsCollections.bodys = new string[]{_character.data.setsCollections.bodys[_index]};
                    _character.data.limbsCollections.bodys = new string[]{_character.data.limbs.bodyID};
                    _character.data.limbsCollections.arms  = new string[]{_character.data.limbs.armID};
                    _character.data.accessoriesCollections.cloths = new string[]{_character.data.accessories.clothID};
                    _character.data.accessoriesCollections.sleeves = new string[]{_character.data.accessories.sleeveID};
                    _character.data.accessoriesCollections.weapons = new string[]{_character.data.accessories.weaponID};
                    break;
                }

                case BodyType.Leg:{
                    _character.data.setsCollections.legs = new string[]{_character.data.setsCollections.legs[_index]};
                    _character.data.limbsCollections.legs  = new string[]{_character.data.limbs.legID};
                    _character.data.accessoriesCollections.pants = new string[]{_character.data.accessories.pantsID};
                    break;
                }
            }
        }
        #endregion

        #region Buttons
        private void Prev(){
            _index--;
            
            Refresh();
        } 
        private void Next(){
            _index++;

            Refresh();
        } 
        #endregion
        
        #region Callback
        private void OnDestroy()
        { 
           onChangeType = null;
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