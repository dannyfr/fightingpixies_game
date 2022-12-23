using System;
using Sirenix.OdinInspector;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataCharacterPreset
    {
        const string grp = "$id";
        const string grpLimbs = grp + "/Limbs";
        const string grpAccessories = grp + "/Accessories";

        [FoldoutGroup(grp),BoxGroup(grp +"/ID"),HideLabel,DisplayAsString] public string id;
        [BoxGroup(grpLimbs),DisplayAsString] public string headid;
        [BoxGroup(grpLimbs),DisplayAsString] public string bodyid;
        [BoxGroup(grpLimbs),DisplayAsString] public string armid;
        [BoxGroup(grpLimbs),DisplayAsString] public string legid;
        [BoxGroup(grpAccessories),DisplayAsString] public string clothid;
        [BoxGroup(grpAccessories),DisplayAsString] public string facialHairid;
        [BoxGroup(grpAccessories),DisplayAsString] public string helmetid;
        [BoxGroup(grpAccessories),DisplayAsString] public string pantsid;
        [BoxGroup(grpAccessories),DisplayAsString] public string sleeveid;
        [BoxGroup(grpAccessories),DisplayAsString] public string weaponid;
    }
}