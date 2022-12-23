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

namespace NFTGame.Dummy.BlockChain
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Dummy + nameof(BlockChainApiDummy))]
    public class BlockChainApiDummy : SerializedMonoBehaviour,IBlockChainAPI,IDisposable
    {
        #region const
        const string grpConfig  = "Config";
        const string grpReq = "Required";
        #endregion

        #region field
        [BoxGroup(grpConfig),SerializeField,InlineEditor]
        private BlockChain.Address _playerAccount;

        [BoxGroup(grpConfig),SerializeField,Range(0,100f)]
        private float _requestFailPercentage = 5;

        [BoxGroup(grpConfig),SerializeField,MinMaxSlider(0.001f,5f,true)]
        private Vector2 _requestTimeRange = new Vector2(1,2);

        [BoxGroup(grpReq),SerializeField]
        private BlockChain.SmartContractDummy _smartContract;

        [BoxGroup(grpReq),SerializeField]
        private Dummy.Server.DatabaseDummy _database;

        [BoxGroup(grpReq),SerializeField]
        private Wallet.Wallet _wallet;
        #endregion

        #region private
        private int _expirationTime;
        private string _account;
        private GameManager _gameManager;
        #endregion

        #region methods
        [DllImport("__Internal")]
        private static extern void Web3Connect();

        [DllImport("__Internal")]
        private static extern string ConnectAccount();

        [DllImport("__Internal")]
        private static extern void SetConnectAccount(string value);

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
        #endregion

        #region IBlockChainAPI
        public string account => _account;
    
        public void Init(GameManager gameManager){
            _gameManager = gameManager;
            _smartContract.Init(_database);
        }

#if UNITY_EDITOR
        public async Task<(Exception,string)> ConnectWalletAsync(){
            var scope = $"{nameof(ConnectWalletAsync)}()";
            this.LogRun(scope);

            _gameManager.uiManager.uiLoading.Show("WAITING CONFIRMATION");

            var confirm = await _wallet.Show("Connect To Wallet");
            if(!confirm){
                _gameManager.uiManager.uiLoading.Hide();
                return (new OperationCanceledException().LogException(this,scope),null);
            }
                
            _gameManager.uiManager.uiLoading.Hide();

            this.LogCompleted(scope);
            _account = _playerAccount.address;
            return (null,_account);
        }
#else    
        public async Task<(Exception,string)> ConnectWalletAsync(){
            var scope = $"{nameof(ConnectWalletAsync)}()";
            this.LogRun(scope);

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
        public async Task<(Exception,uint)> SetMintAsync(string address,Character character,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetMintAsync)}({address},{character})";
            this.LogRun(scope);

            #region check params
            if(address.IsNullOrEmpty())
                return (new NullReferenceException(nameof(address)).LogException(this,scope),default(uint));
            #endregion

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            var confirm = await _wallet.Show("Minting message");
            if(!confirm){
                HideUIProcess(hideUIProcess);
                return(new OperationCanceledException().LogException(this,scope),default(uint));
            }
                
            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(uint));
            
            var tokenId = _smartContract.SetMint(address);
            if(tokenId == default(uint))
                return (new Exception(nameof(SetMintAsync)).LogException(this,scope),tokenId);
            
            this.LogCompleted(scope);
            return (null,tokenId);
        }
        public async Task<Exception> SetToBattleAsync(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetToBattleAsync)}({tokenId})";
            this.LogRun(scope);

            #region check params
            if(tokenId <= 0 )
                return new ArgumentException(nameof(tokenId)).LogException(this,scope);
            #endregion

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            var confirm = await _wallet.Show("Sent To Battle Message");
            if(!confirm){
                HideUIProcess(hideUIProcess);
                return new OperationCanceledException().LogException(this,scope);
            }
                
            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope);
            
            var battleId = _smartContract.SetToBattle(tokenId);
            if(battleId == default(uint))
                return new Exception(nameof(battleId)).LogException(this,scope);
        
            this.LogCompleted(scope);
            return null;
        }
        public async Task<Exception> RetrieveFromBattle(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(RetrieveFromBattle)}({tokenId})";
            this.LogRun(scope);

            #region check params
            if(tokenId <= 0 )
                return new ArgumentException(nameof(tokenId)).LogException(this,scope);
            #endregion

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            var confirm = await _wallet.Show("Retrieve From Battle");
            if(!confirm){
                HideUIProcess(hideUIProcess);
                return new OperationCanceledException().LogException(this,scope);
            }
                
            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope);
            
            _smartContract.RetrieveFromBattle(tokenId);
            this.LogCompleted(scope);
            return null;
        }
        public async Task<Exception> SetBattleAsync(uint tokenId, uint battleId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetBattleAsync)}({tokenId},{battleId})";
            this.LogRun(scope);

            #region check params
            if(battleId <= 0 )
                return new ArgumentNullException(nameof(battleId)).LogException(this,scope);

            if(tokenId <= 0 )
                return new ArgumentException(nameof(tokenId)).LogException(this,scope);
            #endregion

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            var confirm = await _wallet.Show("Sent To Battle Message");
            if(!confirm){
                HideUIProcess(hideUIProcess);
                return new OperationCanceledException().LogException(this,scope);
            }
            
            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);

            if(fail){
                return new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope);
            }else{
                _smartContract.SetBattle(battleId,tokenId);
                this.LogCompleted(scope);
                return null;
            }
        }
        public async Task<Exception> SetClaim(uint battleId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(SetClaim)}({battleId})";
            this.LogRun(scope);

            #region check params
            if(battleId <= 0 )
                return new ArgumentNullException(nameof(battleId)).LogException(this,scope);
            #endregion

            ShowUIProcess("WAITING CONFIRMATION",hideUIProcess);

            var confirm = await _wallet.Show("Claim");
            if(!confirm){
                HideUIProcess(hideUIProcess);
                return new OperationCanceledException().LogException(this,scope);
            }
            
            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);

            if(fail){
                return new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope);
            }else{
                _smartContract.SetClaim(battleId);
                this.LogCompleted(scope);
                return null;
            }
        }
        public async Task<(Exception, uint)> GetBattleRoomOf(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetBattleRoomOf)}({tokenId})";
            this.LogRun(scope);

            ShowUIProcess("CHECKING PIXIES",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));   
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(uint));
                       
            this.LogCompleted(scope);
            return (null,_smartContract.GetBattleRoomOf(tokenId));
        }      
        public async Task<(Exception, bool)> GetBattleAvailable(uint battleId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetBattleAvailable)}({battleId})";
            this.LogRun(scope);

            ShowUIProcess("CHECKING",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(bool));
                
            var data = _smartContract.GetAvailableRoom(battleId);
            this.LogCompleted(scope);
            return (null,data);
        }
        public async Task<(Exception,uint[])> GetBattleAsync(bool hideUIProcess = false)
        {
            var scope = $"{nameof(GetBattleAsync)}()";
            this.LogRun(scope);

            ShowUIProcess("GET BATTLE",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(uint[]));
                
            var data = _smartContract.GetBattle();
            if(data == default(uint[]) || (data.Length == 2 && data[0] == default(uint) && data[1] == default(uint)))
                return (new KeyNotFoundException("Battle Not Exist").LogException(this,scope),data);
            
            this.LogCompleted(scope);
            return (null,data);
        }
        public async Task<(Exception,uint[])> GetPixiesAsync(string address,bool hideUIProcess = false){
            var scope = $"{nameof(GetPixiesAsync)}({address})";
            this.LogRun(scope);

            #region check params
            if(address.IsNullOrEmpty())
                return (new ArgumentException(nameof(address)).LogException(this,scope),default(uint[]));
            #endregion

            ShowUIProcess("GETTING CHARACTERS",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));   
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(uint[]));
                       
            this.LogCompleted(scope);
            return (null,_smartContract.GetPixies(address));
        }
        public async Task<(Exception,uint[])> GetInBattlePixiesAsync(string address,bool hideUIProcess = false){
            var scope = $"{nameof(GetInBattlePixiesAsync)}({address})";
            this.LogRun(scope);

            #region check params
            if(address.IsNullOrEmpty())
                return (new ArgumentException(nameof(address)).LogException(this,scope),default(uint[]));
            #endregion

            ShowUIProcess("GETTING CHARACTERS",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));   
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),default(uint[]));
                       
            this.LogCompleted(scope);
            return (null,_smartContract.GetInBattlePixies(address));
        }     
        public async Task<(Exception,string)> OwnerOfAsync(uint tokenId,bool hideUIProcess = false)
        {
            var scope = $"{nameof(OwnerOfAsync)}({tokenId})";
            this.LogRun(scope);
            
            #region check params
            if(tokenId == default(uint))
                return (new NullReferenceException(nameof(tokenId)).LogException(this,scope),null);
            #endregion

            ShowUIProcess("GET OWNER",hideUIProcess);

            var fail = Random.Range(0,100f) < _requestFailPercentage;
            await new WaitForSeconds(Random.Range(_requestTimeRange.x,_requestTimeRange.y));
            
            HideUIProcess(hideUIProcess);
            
            if(fail)
                return (new WebException(WebExceptionStatus.Timeout.ToString(),WebExceptionStatus.Timeout).LogException(this,scope),null);
            
            this.LogCompleted(scope);
            return (null,_smartContract.OwnerOf(tokenId));
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