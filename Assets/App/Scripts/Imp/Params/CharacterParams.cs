namespace NFTGame.Params
{
    public class CharacterParams{
            #region fields
            public int setsVariant;

            public int limbsHeadVariant;
            public int limbsBodyVariant;
            public int limbsArmVariant ;
            public int limbsLegVariant ;

            public int accessoriesClothVariant;
            public int accessoriesFacialHairVariant;
            public int accessoriesFacialHelmetVariant;
            public int accessoriesFacialPantsVariant;
            public int accessoriesFacialSleeveVariant;
            public int accessoriesFacialWeaponVariant;

            public bool random ;
            #endregion

            #region constructor
            public CharacterParams(){
                SetSetsVariant(-1);
                random      = false;
            }
            public CharacterParams(bool random):this(){
                this.random = random;
            }
            public CharacterParams(int setsVariant,bool random){
                SetSetsVariant(setsVariant);
                this.random = random;
            }
            #endregion
            
            #region methods
            public void SetSetsVariant(int value){
                setsVariant = value;
                SetLimbsVariant(value);
                SetAccessoriesVariant(value);
            }
            private void SetLimbsVariant(int value){
                limbsHeadVariant = value;
                limbsBodyVariant = value;
                limbsArmVariant  = value;
                limbsLegVariant  = value;
            }
            private void SetAccessoriesVariant(int value){
                accessoriesClothVariant        = value;
                accessoriesFacialHairVariant   = value;
                accessoriesFacialHelmetVariant = value;
                accessoriesFacialPantsVariant  = value;
                accessoriesFacialSleeveVariant = value;
                accessoriesFacialWeaponVariant = value;
            }   
            #endregion
        }
}