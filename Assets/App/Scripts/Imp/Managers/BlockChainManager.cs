using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evesoft;
using NFTGame.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFTGame
{
    [HideMonoScript,HideReferenceObjectPicker]
    [AddComponentMenu(Utils.EditorMenu.Managers + nameof(BlockChainManager))]
    public class BlockChainManager : SerializedMonoBehaviour,IDisposable,IBlockChainAPI
    {
        #region const
        const string grpConfig = "Config";
        const string grpReq = "Required";
        #endregion

        #region fields
        [BoxGroup(grpReq),SerializeField]
        private Dictionary<Environment,IBlockChainAPI> _api;
        #endregion

        #region private
        private Environment _env;
        #endregion
       
        #region IBlockChainAPI
        public string account => _api[_env].account;

        public void Init(GameManager gameManager){
            _env = gameManager.environment;
            foreach (var item in _api){
                item.Value.Init(gameManager);
            }
        }

        public Task<(Exception,string)> ConnectWalletAsync(){
            return _api[_env].ConnectWalletAsync();
        }
        public Task<(Exception,uint)> SetMintAsync(string address,Character character,bool hideUIProcess = false)
        {
            return _api[_env].SetMintAsync(address,character,hideUIProcess);
        }
        public Task<Exception> SetToBattleAsync(uint tokenId,bool hideUIProcess = false)
        {
            return _api[_env].SetToBattleAsync(tokenId,hideUIProcess);
        }
        public Task<Exception> RetrieveFromBattle(uint tokenId,bool hideUIProcess = false)
        {
            return _api[_env].RetrieveFromBattle(tokenId,hideUIProcess);
        }
        public Task<Exception> SetBattleAsync(uint tokenId, uint battleId,bool hideUIProcess = false)
        {
            return _api[_env].SetBattleAsync(tokenId,battleId,hideUIProcess);
        }
        public Task<Exception> SetClaim(uint battleId,bool hideUIProcess = false)
        {
            return _api[_env].SetClaim(battleId,hideUIProcess);
        }
        public Task<(Exception, bool)> GetBattleAvailable(uint battleId,bool hideUIProcess = false)
        {
            return _api[_env].GetBattleAvailable(battleId,hideUIProcess);
        }
        public Task<(Exception, uint[])> GetBattleAsync(bool hideUIProcess = false)
        {
            return _api[_env].GetBattleAsync(hideUIProcess);
        }
        public Task<(Exception,uint[])> GetPixiesAsync(string address,bool hideUIProcess = false){
            return _api[_env].GetPixiesAsync(address,hideUIProcess);
        }
        public Task<(Exception,uint[])> GetInBattlePixiesAsync(string address,bool hideUIProcess = false){
            return _api[_env].GetInBattlePixiesAsync(address,hideUIProcess);
        }     
        public Task<(Exception, string)> OwnerOfAsync(uint tokenId,bool hideUIProcess = false)
        {
            return _api[_env].OwnerOfAsync(tokenId,hideUIProcess);
        }     
        public Task<(Exception, uint)> GetBattleRoomOf(uint tokenId,bool hideUIProcess = false)
        {
            return _api[_env].GetBattleRoomOf(tokenId,hideUIProcess);
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
