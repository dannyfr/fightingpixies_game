using System;
using Sirenix.OdinInspector;

namespace NFTGame.Params
{
    [Serializable]
    public class VFXVariantParams{
        
        #region fields
        [HorizontalGroup(width:0.6f),LabelWidth(50)]
        public VFXType type;

        [HorizontalGroup,MinValue(1),LabelWidth(50)]
        public int variant;
        #endregion

        #region constructor
        public VFXVariantParams(){}
        public VFXVariantParams(VFXType type){
            this.type = type;
            this.variant = 1;
        }
        public VFXVariantParams(VFXType type,int variant):this(type){
            this.variant = variant;
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return $"{type}-{variant}";
        }
        #endregion
    }
}