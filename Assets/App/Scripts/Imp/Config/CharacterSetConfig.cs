using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Config
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Config + nameof(CharacterSetConfig),fileName = nameof(CharacterSetConfig))]
    public class CharacterSetConfig : SerializedScriptableObject
    {
        #region const
        const string grpLimbs = "Limbs";
        const string grpAccessories = "Accessories";
        #endregion

        #region fields
        [ShowInInspector,BoxGroup(nameof(id)),DisplayAsString,PropertyOrder(-1),HideLabel]
        public string id => name;

        [BoxGroup(grpLimbs),InlineEditor]
        public HeadConfig head;

        [BoxGroup(grpLimbs),InlineEditor]
        public BodyConfig body;

        [BoxGroup(grpLimbs),InlineEditor]
        public ArmConfig arm;

        [BoxGroup(grpLimbs),InlineEditor]
        public LegConfig leg;

        [BoxGroup(grpAccessories),InlineEditor]
        public ClothConfig cloth;

        [BoxGroup(grpAccessories),InlineEditor]
        public FacialHairConfig facialHair;

        [BoxGroup(grpAccessories),InlineEditor]
        public HelmetConfig helmet;

        [BoxGroup(grpAccessories),InlineEditor]
        public PantsConfig pants;

        [BoxGroup(grpAccessories),InlineEditor]
        public SleeveConfig sleeve;

        [BoxGroup(grpAccessories),InlineEditor]
        public WeaponConfig weapon;
        #endregion
    }
}