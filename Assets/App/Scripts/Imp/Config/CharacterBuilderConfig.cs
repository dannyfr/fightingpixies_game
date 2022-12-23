using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(CharacterBuilderConfig),fileName = nameof(CharacterBuilderConfig))]
    public class CharacterBuilderConfig : SerializedScriptableObject
    {
        #region const
        const string grpLimbs = "Limbs";
        const string grpAccessories = "Accessories";
        const string grpCharacterSet = "Characters Set";
        #endregion

        #region fields
        
        [BoxGroup("Pixel Per Unit"),HideLabel]
        public int pixelPerUnit = 150;

        [BoxGroup(grpCharacterSet),InlineEditor,ListDrawerSettings(DraggableItems = false),HideLabel]
        public List<Config.CharacterSetConfig> characters;

        [BoxGroup(grpLimbs),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.HeadConfig> headsCfg;

        [BoxGroup(grpLimbs),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.BodyConfig> bodysCfg;

        [BoxGroup(grpLimbs),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.ArmConfig> armsCfg;

        [BoxGroup(grpLimbs),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.LegConfig> legsCfg;

        [BoxGroup(grpAccessories),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.ClothConfig> clothsCfg;

        [BoxGroup(grpAccessories),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.FacialHairConfig> facialHairCfg;

        [BoxGroup(grpAccessories),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.HelmetConfig> helmetCfg;

        [BoxGroup(grpAccessories),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.SleeveConfig> sleevesCfg;

        [BoxGroup(grpAccessories),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.PantsConfig> pantsCfg;

        [BoxGroup(grpAccessories),InlineEditor,ListDrawerSettings(DraggableItems = false)]
        public List<Config.WeaponConfig> weaponsCfg;
        #endregion
    }
}