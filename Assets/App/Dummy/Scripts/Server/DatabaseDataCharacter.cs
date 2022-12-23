using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Evesoft;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataCharacter
    {
        #region const
        private const string grp= "$GetID";
        private const string grpMain = grp + "/Main";
        private const string grpmain = grp + "/Main";
        private const string grpstats = grp + "/Stats";
        private const string grpLimbs = grp + "/Limbs";
        private const string grpAccessories = grp + "/Accessories";
        private const string grpStatus = grp + "/Status";
        private const string grpStory = grp + "/Story";
        #endregion

        private string GetID{
            get{
                if(owner.IsNullOrEmpty()){
                    return id;
                }else{
                    var status = active? "Active" : "Disable";
                    return $"{id} - {status} - {owner}";
                }
            }
        }

        [FoldoutGroup(grp),BoxGroup(grpmain),DisplayAsString] public string id;
        [BoxGroup(grpmain),DisplayAsString] public string owner;
        [BoxGroup(grpmain),DisplayAsString] public string name;
        [BoxGroup(grpstats),DisplayAsString] public int hp;
        [BoxGroup(grpstats),DisplayAsString] public int attack;
        [BoxGroup(grpstats),DisplayAsString] public int defense;
        [BoxGroup(grpstats),DisplayAsString] public int speed;
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
        [BoxGroup(grpStatus),DisplayAsString] public bool active;
        [BoxGroup(grpStatus),DisplayAsString] public int energy;
        [BoxGroup(grpStatus),DisplayAsString] public int maxEnergy;
        [BoxGroup(grpStatus),DisplayAsString] public float winRate;
        [BoxGroup(grpStatus),DisplayAsString] public int win;
        [BoxGroup(grpStatus),DisplayAsString] public int lose;
        [BoxGroup(grpStatus),DisplayAsString] public int battleCount;
        [BoxGroup(grpStory),HideLabel,Multiline(10),ReadOnly] public string story;
    }
}