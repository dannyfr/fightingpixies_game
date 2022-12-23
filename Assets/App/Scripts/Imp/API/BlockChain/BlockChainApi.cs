using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Evesoft;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;
using NFTGame.Utils;
using NFTGame.WebAPI;
using NFTGame.WebAPI.Data;
using System.Net.Http;

namespace NFTGame.BlockChain
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(BlockChainApi))]
    public class BlockChainApi : SerializedMonoBehaviour,IBlockChainAPI,IDisposable
    {
        public class BattleFind{
            public string roomId;
            public string tokenId;
        }
        
        public class BattleRoom{
            public string roomId;
            public string tokenId1;
            public string owner1;
            public string tokenId2;
            public string owner2;
            public string status;
        }

        #region const
        const string grpConfig  = "Config";
        const string grpRuntime = "Runtime";
        const string grpComponenet = "Component";
        const string grpReq = "Required";
        #endregion

        #region field
        [BoxGroup(grpReq),SerializeField]
        private Dummy.BlockChain.BlockChainApiDummy _blockchainDummy;
        #endregion

        #region events
        public event Action<IBlockChainAPI> onInited;
        #endregion

        #region property
        private string chain            => _config.chain;
        private string network          => _config.network;
        private string uniqSCAddress    => _config.uniqSC.address;
        private string uniqSCAbi        => _config.uniqSC.abi;
        private string uniqSCRpc        => _config.uniqSC.rpc;
        private string battleSCAddress  => _config.battleSC.address;
        private string battleSCAbi      => _config.battleSC.abi;
        private string battleSCRpc      => _config.battleSC.rpc;

        [BoxGroup(grpConfig),ShowInInspector,HideLabel]
        public RawBlockChain config => _config;
        #endregion

        #region private
        private int _expirationTime;
        private string _account;
        private GameManager _gameManager;
        private RawBlockChain _config;
        #endregion

        #region methods
        [DllImport("__Internal")]
        private static extern void Web3Connect();

        [DllImport("__Internal")]
        private static extern string ConnectAccount();

        [DllImport("__Internal")]
        private static extern void SetConnectAccount(string value);

        private bool CheckErrorRespone(string respone,out string outRespone){
            var errors = new string[]{
                //"Returned error:",
                "Returned error: ",
                //"execution reverted:",
                "execution reverted: ",
                "Exception ERC721: ",
                "MetaMask Tx Signature: "
            };
            var error = false;

            for (var i = 0; i < errors.Length; i++)
            {
                var contain = respone.Contains(errors[i]);
                error  |= contain;

                if(contain){
                    respone = respone.Replace(errors[i],"");
                }
            }
            
            outRespone = respone;
            return error;
        }
        private void ShowUIProcess(string message = null,bool hideUIProcess = false){
            if(hideUIProcess)
                return;

            _gameManager.uiManager.uiLoading.Show(message);
        }
        private void HideUIProcess(bool hideUIProcess = false){
            if(hideUIProcess)
                return;

            _gameManager.uiManager.uiLoading.Hide();
        }        
        private async void GetConfigAsync(Action onComplete){
           
            //Request
            var url = $"{WebAPI.WebAPI.baseUrl}/blockchain/config";
            do
            {
                var respone = await url.HttpRequestGet<WebAPIRespone<RawBlockChain>>();
                _config = respone.result;
            } while (_config.IsNull());

            onComplete?.Invoke();
        }
        #endregion
  
        #region IBlockChainAPI
        public string account => _account;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            GetConfigAsync(()=>
            {   
                onInited?.Invoke(this);
            });
        }
#if UNITY_EDITOR
        public async Task<(Exception,string)> ConnectWalletAsync(){
           (var ex, var account) = await _blockchainDummy.ConnectWalletAsync();
           this._account = account;
           return (ex,this._account); 
        }
#else    
        public async Task<(Exception,string)> ConnectWalletAsync(){
            var scope = $"{nameof(ConnectWalletAsync)}()";
            this.LogRun(scope);

            if(_config.IsNull())
                return (new NullReferenceException().LogException(this,scope),default(string));

            Web3Connect();
            
            try
            {
                #region TryConnectWallet
                //Connect to wallet
                do
                {
                    _account = ConnectAccount();
                    await new WaitForSeconds(1);
                } while (_account.IsNullOrEmpty());
                #endregion

                #region SignLoginMessage
                // create expiration time
                DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                int currentTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
                _expirationTime = (currentTime + 30);

                // create message to sign
                string message = _account + "-" + _expirationTime.ToString();
                string signature = await Web3GL.Sign(message);
                #endregion

                #region VerifySignature
                // get current time
                epochStart  = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                currentTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

                // return if date expired
                if (currentTime > _expirationTime) 
                    return (new TimeoutException("Date expired"),null);

                // get owner of signature
                message = _account + '-' + _expirationTime.ToString();
                string owner = await EVM.Verify(message, signature);

                // return if not owner
                if (owner != _account) 
                    return (new Exception("You are not owner"),null);

                // reset login message
                SetConnectAccount("");
                this.LogCompleted(scope);
                return (null,_account);
                #endregion
            }
            catch(Exception ex)
            {
                return (ex.LogException(this,scope),null);
            }
        }
#endif
        public async Task<(Exception, uint)> SetMintAsync(string address,Character character,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetMintAsync)}({address}.{character.data.name})";
            this.LogRun(scope);

            #region Get VRS
            ShowUIProcess("GET VRS",hideUIProcess);
            var url = $"https://apidev.unique.one/generateVRS/token/erc721/0x6656e037F3281b95EC0E2fBBc68F809431eeb1dA/{address}";
            var responeVRS = await url.HttpRequestGet<WebAPIRespone<RawVRS>>();
            HideUIProcess(hideUIProcess);

            if(responeVRS.IsNull() || responeVRS.status != HttpStatusCode.OK)
                return (new HttpRequestException($"{responeVRS.status} - {responeVRS.message}").LogException(this,scope),default(uint));
            
            if(responeVRS.result.IsNull())
                return (new Exception("Get VRS failed"),default(uint));
            
            //Change respone v
            if(responeVRS.result.v == "0x1b"){
                responeVRS.result.v = "27";
            }else if(responeVRS.result.v == "0x1c"){
                responeVRS.result.v = "28";
            }
            #endregion

            #region Upload Image to IPFS
            ShowUIProcess("UPLOADING IMAGE",hideUIProcess);
            url = "https://ipfs.infura.io:5001/api/v0/add?stream-channels=true";
            var fileName = $"{character.name}.png";
            var bytes    = character.data.thumbnail.texture.EncodeToPNG();
            var responeIPFS = await url.HttpRequestPostFile<RawIPFSData>(fileName,bytes);
            HideUIProcess(hideUIProcess);
            if(responeIPFS.IsNull())
                return (new Exception("Upload pixies image failed"),default(uint));
            #endregion

            #region Create Mint Data
            var tokenId  = responeVRS.result.tokenId;
            var attribute = new List<RawMintAttribute>();
            
            //Attack
            if((character.data.showstat & Stats.ATTACK) != 0)
                attribute.Add(new RawMintAttribute("Attack","Attack",character.data.stats.attack.ToString()));
            
            if((character.data.showstat & Stats.DEFENSE) != 0)
                attribute.Add(new RawMintAttribute("Defense","Defense",character.data.stats.defense.ToString()));

            if((character.data.showstat & Stats.SPEED) != 0)
                attribute.Add(new RawMintAttribute("Speed","Speed",character.data.stats.speed.ToString()));

            if((character.data.showstat & Stats.HP) != 0)
                attribute.Add(new RawMintAttribute("Hp","Hp",character.data.stats.hp.ToString()));

            (var rawMintData,var ex) = new RawMintData(){
                name        = character.data.name,
                description = character.data.story,
                image       = $"https://ipfs.infura.io/ipfs/{responeIPFS.Hash}",
                external_url= $"https://appdev.unique.one/token2/0x6656e037F3281b95EC0E2fBBc68F809431eeb1dA:{tokenId}",
                attributes  = attribute.ToArray()
            }.ToJson();
            
            if(!ex.IsNull())
                return (ex.LogException(this,scope),default(uint));
            #endregion

            #region MintData to IPFS
            ShowUIProcess("UPLOADING METADATA",hideUIProcess);
            fileName = $"pixies-{character.name}";
            bytes    = System.Text.Encoding.UTF8.GetBytes(rawMintData);
            responeIPFS  = await url.HttpRequestPostFile<RawIPFSData>(fileName,bytes);
            HideUIProcess(hideUIProcess);
            if(responeIPFS.IsNull())
                return (new Exception("Upload pixies data failed"),default(uint));
            #endregion
            
            #region Minting To BlockChain
            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);
            var royalty   = 1000;
            var response  = default(string);
            var args = $"[\"{tokenId}\",\"{responeVRS.result.v}\",\"{responeVRS.result.r}\",\"{responeVRS.result.s}\",[{{\"recipient\":\"{address}\", \"value\": {royalty}}}],\"{responeIPFS.Hash}\"]";
            (ex,response) = await WriteContract(uniqSCAddress,uniqSCAbi,"mint",args);
            HideUIProcess(hideUIProcess);
            
            if(!ex.IsNull()){
                return (ex.LogException(this,scope),default(uint));
            }
            #endregion
 
            this.LogCompleted(scope);
            return (null,tokenId);
        }
        public async Task<Exception> SetToBattleAsync(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetToBattleAsync)}({tokenId})";
            this.LogRun(scope);
            
            #region Check Owner
            var ex      = default(Exception);
            var owner   = default(string);
            (ex,owner)  = await OwnerOfAsync(tokenId);
            if(!ex.IsNull()){
                return ex;
            }else if(owner != account){
                return new Exception("Only owner of pixie");
            }
            #endregion
       
            #region Check Approve
            //Check approve
            var approve     = default(bool);
            (ex, approve)   = await GetApproved(tokenId);
            if(!ex.IsNull()){
                return ex;
            }else if(!approve){
                //Approve
                ex = await SetApprove(tokenId);
                if(!ex.IsNull()){
                    return ex;
                }

                //Check until approve
                do
                {
                    ShowUIProcess("CHECKING APPROVE",hideUIProcess);
                    await new WaitForSeconds(5);
                    (ex, approve) = await GetApproved(tokenId);
                    if(!ex.IsNull()){
                        return ex;
                    }
                } while (!approve);
            }
            #endregion

            #region ReadyToBattle

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);
            
            var response  = default(string);
            (ex,response) = await WriteContract(battleSCAddress,battleSCAbi,"readyToBattle",$"[\"{tokenId}\"]");
            
            HideUIProcess(hideUIProcess);
            
            if(!ex.IsNull()){
                return ex.LogException(this,scope);
            }
            #endregion

            this.LogCompleted(scope);
            return null;
        }
        public async Task<Exception> RetrieveFromBattle(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(RetrieveFromBattle)}({tokenId})";
            this.LogRun(scope);

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            (var ex,var response) = await WriteContract(battleSCAddress,battleSCAbi,"inactivePixie",$"[\"{tokenId}\"]");
            
            HideUIProcess(hideUIProcess);

            if(!ex.IsNull()){
                return ex.LogException(this,scope);
            }

            this.LogCompleted(scope);
            return null;
        }
        public async Task<Exception> SetBattleAsync(uint tokenId, uint battleId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetBattleAsync)}({tokenId},{battleId})";
            this.LogRun(scope);
            
            #region Check Owner
            var ex      = default(Exception);
            var owner   = default(string);
            (ex,owner)  = await OwnerOfAsync(tokenId);
            if(!ex.IsNull()){
                return ex;
            }else if(owner != account){
                return new Exception("Only owner of pixie");
            }
            #endregion
       
            #region Check Approve
            //Check approve
            var approve     = default(bool);
            (ex, approve)   = await GetApproved(tokenId);
            if(!ex.IsNull()){
                return ex;
            }else if(!approve){
                //Approve
                ex = await SetApprove(tokenId);
                if(!ex.IsNull()){
                    return ex;
                }

                //Check until approve
                do
                {
                    ShowUIProcess("CHECKING APPROVE",hideUIProcess);
                    await new WaitForSeconds(2);
                    (ex, approve) = await GetApproved(tokenId);
                    if(!ex.IsNull()){
                        return ex;
                    }
                } while (!approve);
            }
            #endregion

            #region Battle
            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            var response  = default(string);
            (ex,response) = await WriteContract(battleSCAddress,battleSCAbi,"battle",$"[\"{tokenId}\",\"{battleId}\"]");
            
            HideUIProcess(hideUIProcess);
            
            if(!ex.IsNull()){
                return ex.LogException(this,scope);
            }
            #endregion

            this.LogCompleted(scope);
            return null;
        }
        public async Task<Exception> SetClaim(uint battleId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetClaim)}({battleId})";
            this.LogRun(scope);

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            (var ex,var response) = await WriteContract(battleSCAddress, battleSCAbi,"claim",$"[\"{battleId}\"]");
            
            HideUIProcess(hideUIProcess);

            if(!ex.IsNull()){
                return ex.LogException(this,scope);
            }

            var claimed = false;
            do
            {   
                ShowUIProcess("CHECK CLAIM",hideUIProcess);
                await new WaitForSeconds(5);
                (ex,claimed) = await GetClaim(battleId);
            } while (!claimed);


            HideUIProcess(hideUIProcess);
            this.LogCompleted(scope);
            return null;
        }
        public async Task<(Exception, uint)> GetBattleRoomOf(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetBattleRoomOf)}({tokenId})";
            this.LogRun(scope);

            ShowUIProcess("CHECKING PIXIES",hideUIProcess);
            
            (var ex,var response) = await ReadContract(battleSCAddress, battleSCAbi,"getBattleRoomOf",$"[\"{tokenId}\"]",battleSCRpc);
            
            HideUIProcess(hideUIProcess);
            
            if(!ex.IsNull()){
                return (ex.LogException(this,scope),default(uint));
            }

            //parsing
            uint roomId;
            if(!uint.TryParse(response,out roomId)){
                return (new Exception(response).LogException(this,scope),default(uint));
            }

            this.LogCompleted(scope);
            return (null,roomId);
        }
        public async Task<(Exception, bool)> GetBattleAvailable(uint battleId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetBattleAvailable)}({battleId})";
            this.LogRun(scope);

            ShowUIProcess("CHECKING",hideUIProcess);

            (var ex,var response) = await ReadContract(battleSCAddress, battleSCAbi,"IsRoomAvailable",$"[\"{battleId}\"]",battleSCRpc);
            
            HideUIProcess(hideUIProcess);

            if(!ex.IsNull()){
                return (ex.LogException(this,scope),default(bool));
            }

            //parsing
            bool available = false;
            if(!bool.TryParse(response,out available)){
                return (new Exception(response).LogException(this,scope),default(bool));
            }

            this.LogCompleted(scope);
            return (null,available);
        }
        public async Task<(Exception, uint[])> GetBattleAsync(bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetBattleAsync)}()";
            this.LogRun(scope);

            ShowUIProcess("GET BATTLE",hideUIProcess);

            (var ex,var response) = await ReadContract(battleSCAddress, battleSCAbi,"GetOpponent","[]",battleSCRpc);
            
            HideUIProcess(hideUIProcess);
            
            if(!ex.IsNull()){
                return (ex.LogException(this,scope),default(uint[]));
            }

            //parsing
            var battle = default(BattleFind);
            (battle,ex) = response.FromJsonUnity<BattleFind>();
            if(!ex.IsNull()){
                return (ex.LogException(this,scope),default(uint[]));
            }

            var result = new uint[2];
            result[0] = uint.Parse(battle.roomId);
            result[1] = uint.Parse(battle.tokenId);

            this.LogCompleted(scope);
            return (null,result);
        }
        public async Task<(Exception, uint[])> GetPixiesAsync(string address,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetPixiesAsync)}({address})";
            this.LogRun(scope);

            #region Get Balance
            ShowUIProcess("GETTING BALANCE",hideUIProcess);
            var balance = await ERC721.BalanceOf(chain,network,uniqSCAddress,address);
            if(balance <= 0){
                HideUIProcess(hideUIProcess);
                this.LogCompleted(scope);
                return (null,default(uint[]));
            }
            #endregion
                
            #region Get All Tokens
            var tokens = new uint[balance];
            if(balance > 0){
                var completed       = 0;
                var onComplete      = default(Action<string,int>);
                var onFailed        = default(Action<Exception,int>);
                var getTokenAction  = default(Action<int>);

                onComplete      = (response,index)=>{
                    completed++;
                    ShowUIProcess($"GETTING TOKEN {completed}/{balance}",hideUIProcess);
                    uint.TryParse(response,out tokens[index]);
                };
                onFailed        = (ex,index)=>
                {
                    getTokenAction.Invoke(index);
                };
                getTokenAction  = (index)=>{
                    ReadContract(uniqSCAddress,uniqSCAbi,"tokenOfOwnerByIndex",$"[\"{address}\",\"{index}\"]",uniqSCRpc,(response)=>{
                        onComplete.Invoke(response,index);
                    },(ex)=>
                    {
                        onFailed.Invoke(ex,index);
                    });
                };

                ShowUIProcess($"GETTING TOKEN {completed}/{balance}",hideUIProcess);
                for (var i = 0; i < balance; i++)
                {
                    //GetToken
                    var index = i;
                    getTokenAction.Invoke(index);
                }
                await new WaitUntil(()=> completed == balance);
            }
            #endregion

            #region Get from Uniq API
            //TODO:Change this when prod ready
            var prod  = false;

            // //GetCollection
            // var prodUrl = $"https://apixdai2.unique.one/getUserCollectedCollectibleList?userPublicAddress={address}&type=collections"; 
            // var devUrl  = $"https://apidev.unique.one/getUserCollectedCollectibleList?userPublicAddress={address}&type=collections";
            // var url     = prod ? prodUrl : devUrl;
            // var collectionResult = await url.HttpRequestGet<WebAPIUniqRespone<UniqDataCollectionResult>>();
            // if(collectionResult.IsNull()){
            //     return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(uint[]));
            // }
                
            //Get Collection OnSale
            var prodUrl = $"https://apixdai2.unique.one/getUserOnsaleCollectibleList?userPublicAddress={address}&type=onsale";
            var devUrl  = $"https://apidev.unique.one/getUserOnsaleCollectibleList?userPublicAddress={address}&type=onsale";
            var url     = prod ? prodUrl : devUrl;
            
            ShowUIProcess("FILTER ON SALE PIXIES",hideUIProcess);
            var onSaleResult = await url.HttpRequestGet<WebAPIUniqRespone<UniqDataOnSaleResult>>();
            HideUIProcess(hideUIProcess);
            
            if(onSaleResult.IsNull()){            
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(uint[]));
            }

            if(!onSaleResult.tokenIds.IsNullOrEmpty()){
                var onSale    = onSaleResult.tokenIds;
                var available = new List<uint>(tokens);
                available.RemoveAll(x=> Array.IndexOf(onSale,x) >= 0);
                tokens = available.ToArray();
            }
            #endregion 

            this.LogCompleted(scope);
            return (null,tokens);
        }
        public async Task<(Exception, uint[])> GetInBattlePixiesAsync(string address,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetInBattlePixiesAsync)}({address})";
            this.LogRun(scope);

            #region GetBalance
            ShowUIProcess("GETTING IN BATTLE BALANCE",hideUIProcess);
            var balance = await ERC721.BalanceOf(chain,network,battleSCAddress,address);
            if(balance <= 0){
                HideUIProcess(hideUIProcess);
                this.LogCompleted(scope);
                return (null,default(uint[]));
            }
            #endregion
            
            #region Get all tokens
            var tokens = new uint[balance];
            if(balance > 0){
                var completed       = 0;
                var onComplete      = default(Action<string,int>);
                var onFailed        = default(Action<Exception,int>);
                var getTokenAction  = default(Action<int>);

                onComplete      = (response,index)=>{
                    completed++;
                    ShowUIProcess($"GETTING IN BATTLE TOKEN {completed}/{balance}",hideUIProcess);
                    uint.TryParse(response,out tokens[index]);
                };
                onFailed        = (ex,index)=>
                {
                    getTokenAction.Invoke(index);
                };
                getTokenAction  = (index)=>{
                    ReadContract(battleSCAddress,battleSCAbi,"tokenOfOwnerByIndex",$"[\"{address}\",\"{index}\"]",battleSCRpc,(response)=>{
                        onComplete.Invoke(response,index);
                    },(ex)=>
                    {
                        onFailed.Invoke(ex,index);
                    });
                };

                ShowUIProcess($"GETTING IN BATTLE TOKEN {completed}/{balance}",hideUIProcess);

                for (var i = 0; i < balance; i++)
                {
                    //GetToken
                    var index = i;
                    getTokenAction.Invoke(index);
                }
                await new WaitUntil(()=> completed == balance);
            }
            #endregion

            this.LogCompleted(scope);
            return (null,tokens);
        }
        public async Task<(Exception,bool)> GetClaim(uint roomId,bool hideUIProcess = false){
            var scope = $"{nameof(GetClaim)}({roomId})";
            this.LogRun(scope);

            //ShowUIProcess("CHECK CLAIM",hideUIProcess);

            (var ex,var response) = await ReadContract(battleSCAddress, battleSCAbi,"battleRoom",$"[\"{roomId}\"]",battleSCRpc);
            
            //HideUIProcess(hideUIProcess);
            
            if(!ex.IsNull()){
                return (ex.LogException(this,scope),true);
            }

            //parsing
            var battleRoom = default(BattleRoom);
            (battleRoom,ex) = response.FromJsonUnity<BattleRoom>();
            if(!ex.IsNull()){
                return (ex.LogException(this,scope),true);
            }

            var result = true;
            var index  = default(uint);
            uint.TryParse(battleRoom.status,out index);           
            switch(index){
                case 2:{
                    result = false;
                    break;
                }
            }
            
            this.LogCompleted(scope);
            return (null,result);
        }
        public async Task<(Exception, string)> OwnerOfAsync(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(OwnerOfAsync)}({tokenId})";
            this.LogRun(scope);

            ShowUIProcess("GETTING OWNER",hideUIProcess);
            var response = await ERC721.OwnerOf(chain,network,uniqSCAddress,tokenId.ToString());
            HideUIProcess(hideUIProcess);
            if(CheckErrorRespone(response,out response)){
                return (new Exception(response),default(string));
            }

            this.LogCompleted(scope);
            return (null,response);
        }
        
        private async Task<(Exception,bool)> GetApproved(uint tokenId,bool hideUIProcess = false){
            var scope = $"{nameof(GetApproved)}({tokenId})";
            this.LogRun(scope);
            
            ShowUIProcess("CHECKING APPROVE",hideUIProcess);
            (var ex,var response) = await ReadContract(uniqSCAddress,uniqSCAbi,"getApproved",$"[\"{tokenId}\"]",uniqSCRpc);
            HideUIProcess(hideUIProcess);
            if(!ex.IsNull()){
                return (ex.LogException(this,scope),default(bool));
            }

            this.LogCompleted(scope);
            return (null,response == battleSCAddress); 
        }
        private async Task<Exception> SetApprove(uint tokenId,bool hideUIProcess = false){
            var scope = $"{nameof(SetApprove)}({tokenId})";
            this.LogRun(scope);

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);
            (var ex,var response) = await WriteContract(uniqSCAddress,uniqSCAbi,"approve",$"[\"{battleSCAddress}\",\"{tokenId}\"]");
            HideUIProcess(hideUIProcess);
            if(!ex.IsNull()){
                return ex.LogException(this,scope);
            }

            this.LogCompleted(scope);
            return null;
        }
        
        public async Task<(Exception,string)> WriteContract(string contract,string abi,string method,string args){
            try {
                var value = "0";
                var response = await Web3GL.Send(method, abi, contract, args, value);
                if(CheckErrorRespone(response,out response)){
                    return (new Exception(response),default(string));
                }else{
                    return(null,response);
                }
            } catch (Exception ex) {
                return(ex,default(string));
            };
        }
        public async Task<(Exception,string)> ReadContract(string contract,string abi,string method,string args,string rpc){
            try {
                var response = await EVM.Call(chain,network,contract,abi,method,args,rpc);
                if(CheckErrorRespone(response,out response)){
                    return (new Exception(response),default(string));
                }else{
                    return(null,response);
                }
            } catch (Exception ex) {
                return(ex,default(string));
            };
        }
        public async void ReadContract(string contract,string abi,string method,string args,string rpc,Action<string> onCompleted,Action<Exception> onFailed = null){
            (var ex,var respone) = await ReadContract(contract,abi,method,args,rpc);
            if(!ex.IsNull()){
                onFailed?.Invoke(ex);
            }else{
                onCompleted?.Invoke(respone);
            }
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