using System;
using Sirenix.OdinInspector;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct BattleResult{
        #region fields
        public bool win;
        public Character character;
        #endregion

        #region constructor
        public BattleResult(bool win,Character character){
            this.win = win;
            this.character = character;
        }
        #endregion
    }
}