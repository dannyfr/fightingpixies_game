namespace NFTGame.Params
{
    public struct VFXBlinkingParams{
            
        #region fields
        public NFTGame.Character character;
        public int loop;
        #endregion

        #region constructors
        public VFXBlinkingParams(NFTGame.Character character,int loop){
            this.character = character;
            this.loop = loop;
        }
        #endregion
    }
}