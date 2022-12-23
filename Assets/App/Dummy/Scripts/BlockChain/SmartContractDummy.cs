using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Evesoft;
using Random = UnityEngine.Random;
using System.Linq;
using NFTGame.Dummy.Server;

namespace NFTGame.Dummy.BlockChain
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Dummy + nameof(SmartContractDummy))]
    public class SmartContractDummy : SerializedMonoBehaviour ,IDisposable
    {
        public enum BattleStatus{
            OPEN,
            IN_BATTLE,
            FINISH,
            CANCELED
        }

        public delegate void OnBattleStartHandler(uint battleId,string address1,string address2,uint tokenId1,uint tokenId2);
        public delegate void OnClaimHandler(uint battleId);

        #region const
        const string grpConfig = "Config";
        const string grpRuntime = "Runtime";
        const string grpRequired = "Required";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpConfig),InlineEditor]
        private Address _address;

        [SerializeField,BoxGroup(grpConfig),Range(0,100),LabelText("Init Battle %")]
        private int _battleCount = 25;

        [SerializeField,HideLabel,HideInInspector]
        private uint tokenCounter;

        [SerializeField,BoxGroup(grpRequired),Required,ListDrawerSettings(Expanded = true,DraggableItems = false)]
        private Address[] _userAddress;

        [NonSerialized,ShowInInspector,BoxGroup(grpRuntime),DictionaryDrawerSettings(IsReadOnly = true)] 
        private Dictionary<uint,string> tokenId = new Dictionary<uint, string>();

        [NonSerialized,ShowInInspector,BoxGroup(grpRuntime),DictionaryDrawerSettings(IsReadOnly = true)] 
        private Dictionary<string,List<uint>> nft = new Dictionary<string, List<uint>>(); // address <tokenID>

        [NonSerialized,ShowInInspector,BoxGroup(grpRuntime),DictionaryDrawerSettings(IsReadOnly = true)] 
        private Dictionary<string,List<uint>> nftactive = new Dictionary<string, List<uint>>();

        [NonSerialized,ShowInInspector,BoxGroup(grpRuntime),DictionaryDrawerSettings(IsReadOnly = true)] 
        private Dictionary<uint,Dictionary<uint,string>> rooms = new Dictionary<uint, Dictionary<uint, string>>(); // battleId <adress,tokenID>

        [NonSerialized,ShowInInspector,BoxGroup(grpRuntime),DictionaryDrawerSettings(IsReadOnly = true)]
        private Dictionary<uint,BattleStatus> roomsStatus = new Dictionary<uint, BattleStatus>();

        [NonSerialized,ShowInInspector,BoxGroup(grpRuntime),DictionaryDrawerSettings(IsReadOnly = true)] 
        private Dictionary<uint,uint> result = new Dictionary<uint, uint>(); //battleId <winner tokend id>
        #endregion

        #region events
        public event OnBattleStartHandler onBattleStart;
        public event OnClaimHandler onClaim;
        #endregion

        #region property
        public string address => _address.address;
        #endregion

        #region methods
        public void Init(DatabaseDummy database){
            tokenId?.Clear();
            nft?.Clear();
            nftactive?.Clear();
            rooms?.Clear();
            result?.Clear();

            //minting pixies to smart contract
            if(!database.pixies.IsNullOrEmpty()){
                foreach (var item in database.pixies)
                {
                    var index = Random.Range(0,_userAddress.Length);
                    var owner = _userAddress[index].address;
                    var tokenid = SetMint(owner);
                    item.id = tokenid.ToString();
                }
            }

            //sent to battle
            var battleCount = Mathf.RoundToInt(_battleCount/100f * database.pixies.Count);
            var pixiesLeft  = new List<DatabaseDataCharacter>(database.pixies);
            for (var i = 0; i < battleCount; i++)
            {
                var index   = Random.Range(0,pixiesLeft.Count);
                var tokenId = uint.Parse(pixiesLeft[index].id);
                SetToBattle(tokenId);
                pixiesLeft.RemoveAt(index);
            } 
        }
        public uint SetMint(string address){
            if(!nft.ContainsKey(address))
                nft[address] = new List<uint>();

            tokenCounter++;
            var tokenID = (uint)tokenCounter + 1;
            nft[address].Add(tokenID);

            this.tokenId[tokenID] = address;
            return tokenID;
        }
        public void SetMint(string address,uint tokenId){
            if(!nft.ContainsKey(address))
                nft[address] = new List<uint>();

            if(!nft[address].Contains(tokenId))
                nft[address].Add(tokenId);

            this.tokenId[tokenId] = address;
        }    
        public uint SetToBattle(uint tokenId){
            var address = OwnerOf(tokenId);
            
            if(!nft.ContainsKey(address))
                return default(uint);

            //transfer owner to smartcontract
            Transfer(address,this._address.address,tokenId);

            //set to battle room
            var battleId             = (uint)rooms.Count + 1;
            rooms[battleId]          = new Dictionary<uint, string>();
            rooms[battleId][tokenId] = address;
            roomsStatus[battleId] = BattleStatus.OPEN;
            
            //set to active nft
            if(!nftactive.ContainsKey(address)){
                nftactive[address] = new List<uint>();
            }       
            nftactive[address].Add(tokenId);

            return battleId;
        }
        public void RetrieveFromBattle(uint tokenId)
        {
            //Get RoomBattleId
            if(!rooms.IsNullOrEmpty()){
                foreach (var room in rooms)
                {
                    if(room.Value.ContainsKey(tokenId) && roomsStatus[room.Key] == BattleStatus.OPEN){
                        var battleId = room.Key;
                        var prevAddress  = room.Value[tokenId];

                        //Transfer back to previous owner
                        Transfer(this._address.address,prevAddress,tokenId);

                        //remove from active nft
                        nftactive[prevAddress].Remove(tokenId);

                        //Set room status to canceled
                        roomsStatus[battleId] = BattleStatus.CANCELED;
                        break;
                    }
                }
            }
        }
        public void SetBattle(uint battleId,uint tokenId){
            var address  = OwnerOf(tokenId);
            var opponentToken   = GetTokenOfBattle(battleId)[0];
            var opponentAddress = default(string);

            //Get opponent address
            foreach (var item in nftactive)
            {
                if(item.Value.IsNullOrEmpty())
                    continue;

                if(!item.Value.Contains(opponentToken))
                    continue;

                opponentAddress = item.Key;
                break;
            }
            
            //transfer owner to smartcontract
            Transfer(address,this._address.address,tokenId);

            //set to battle room
            rooms[battleId][tokenId] = address;
            roomsStatus[battleId]= BattleStatus.IN_BATTLE;
            //set to active nft
            if(!nftactive.ContainsKey(address))
                nftactive[address] = new List<uint>();

            nftactive[address].Add(tokenId);

            onBattleStart?.Invoke(battleId,address,opponentAddress,tokenId,opponentToken);
        }
        public void SetWinner(uint battleId,uint tokenId){
            //set result
            result[battleId]     = tokenId;
            roomsStatus[battleId]= BattleStatus.FINISH;

            if(rooms.ContainsKey(battleId)){
                foreach (var item in rooms[battleId])
                {
                    var token = item.Key;
                    var owner = item.Value;
                    nftactive[owner].Remove(token);
                }
            }
        }       
        public void SetClaim(uint battleId){
            //Get winner token
            var tokenId = result[battleId];

            //Transfer to new owner
            var winAddress = rooms[battleId][tokenId];
            foreach (var item in rooms[battleId])
            {
                //Transfer all pixies to winner
                Transfer(this._address.address,winAddress,item.Key);

                //remove from active
                nftactive[item.Value]?.Remove(item.Key);
                if(nftactive[item.Value].IsNullOrEmpty())
                    nftactive.Remove(item.Value);
            }

            onClaim?.Invoke(battleId);
        }
        public bool GetAvailableRoom(uint battleId){
            if(!roomsStatus.ContainsKey(battleId))
                return false;

            if(roomsStatus[battleId] == BattleStatus.OPEN)
                return true;

            return false;
        }
        public uint GetBattleRoomOf(uint tokenId)
        {
            if(!rooms.IsNullOrEmpty()){
                foreach (var room in rooms)
                {
                    if(room.Value.ContainsKey(tokenId) && (roomsStatus[room.Key] == BattleStatus.OPEN || roomsStatus[room.Key] == BattleStatus.IN_BATTLE))
                        return room.Key;
                }
            }

            return default(uint);
        }
        public uint[] GetBattle()
        {
            if(rooms.IsNullOrEmpty())
                return default(uint[]);

            #region old
            // var attempt = 0;
            // var count   = rooms.Count;
            // var index   = Random.Range(0,count-1);
            // var result = new uint[2];
    
            // do
            // {
            //     var kpair    = rooms.ElementAt(index);
            //     var battleId = kpair.Key;
               
            //     if(kpair.Value.Count >=2){
            //         attempt++;
            //         index++;

            //         if(index > count-1)
            //             index = 0;

            //         continue;
            //     }else{
            //         result[0] = battleId;
            //     }
            // } while (attempt < count && result[0] == default(uint));


            // if(result[0] != default(uint)){
            //     result[1] = GetTokenOfBattle(result[0])[0];
            // }
            //return result;
            #endregion

            foreach (var item in roomsStatus)
            {
                if(item.Value == BattleStatus.OPEN)
                    return new uint[]{item.Key,rooms[item.Key].ElementAt(0).Key};
            }

            return default(uint[]);
        }
        public uint[] GetPixies(string address){
            if(!nft.ContainsKey(address))
                return default(uint[]);

            return nft[address].ToArray();
        }
        public uint[] GetInBattlePixies(string address){
            if(!nftactive.ContainsKey(address))
                return default(uint[]);

            return nftactive[address].ToArray();
        }
        public string OwnerOf(uint tokenId){
            if(!this.tokenId.ContainsKey(tokenId))
                return null;

            return this.tokenId[tokenId];
        }
        
        private uint[] GetTokenOfBattle(uint battleId){
            if(!rooms.ContainsKey(battleId))
                return default(uint[]);

            var tokenIds = new uint[rooms[battleId].Count];
            var index    = 0;
            foreach (var item in rooms[battleId])
            {
                tokenIds[index] = item.Key;
                index++;
            }
            return tokenIds;
        }      
        private void Transfer(string from,string to,uint tokenId){
            //remove from old owner
            nft[from].Remove(tokenId);

            //add to new owner
            if(!nft.ContainsKey(to))
                nft[to] = new List<uint>();

            //set tokenId
            nft[to].Add(tokenId);
            this.tokenId[tokenId] = to;
        }
        private void RemoveByToken(uint tokenId){
            if(!this.tokenId.ContainsKey(tokenId))
                return;
            
            var address = this.tokenId[tokenId];
            this.tokenId.Remove(tokenId);

            if(!nft.ContainsKey(address))
                return;

            nft[address]?.Remove(tokenId);
            
            if(nft[address].IsNullOrEmpty())
                nft.Remove(address);

        }
        #endregion

        #region callbacks
        private void OnDestroy(){
            onBattleStart = null;
            onClaim = null;
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