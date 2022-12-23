using System;
using System.Threading.Tasks;

namespace NFTGame
{
    public interface IBlockChainAPI
    {
        void Init(GameManager gameManager);
        string account{get;}
        Task<(Exception,string)> ConnectWalletAsync();
        Task<(Exception,uint)> SetMintAsync(string address,Character character,bool hideUIProcess = false);
        Task<Exception> SetToBattleAsync(uint tokenId,bool hideUIProcess = false);
        Task<Exception> RetrieveFromBattle(uint tokenId,bool hideUIProcess = false);
        Task<Exception> SetBattleAsync(uint tokenId,uint battleId,bool hideUIProcess = false);
        Task<Exception> SetClaim(uint battleId,bool hideUIProcess = false);
        Task<(Exception,bool)> GetBattleAvailable(uint battleId,bool hideUIProcess = false);
        Task<(Exception,uint[])> GetBattleAsync(bool hideUIProcess = false);
        Task<(Exception,uint[])> GetPixiesAsync(string address,bool hideUIProcess = false);
        Task<(Exception,uint[])> GetInBattlePixiesAsync(string address,bool hideUIProcess = false);
        Task<(Exception,uint)> GetBattleRoomOf(uint tokenId,bool hideUIProcess = false);
        Task<(Exception,string)> OwnerOfAsync(uint tokenId,bool hideUIProcess = false);
    }
}