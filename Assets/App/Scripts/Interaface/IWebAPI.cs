using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NFTGame
{
    public interface IWebAPI
    {
        void Init(GameManager gameManager);
        Task<Exception> GetTokenAsync(string address,bool hideUIProcess = false);
        Task<Exception> AddCharacterAsync(CharacterData data,bool hideUIProcess = false);
        Task<Exception> ClaimRewardAsync(string rewardId,bool hideUIProcess = false);
        Task<(Exception,HistoryData[])> GetHistoryAsync(string address,bool hideUIProcess = false);
        Task<(Exception,RewardData[])> GetRewardsAsync(string address,bool hideUIProcess = false);
        Task<(Exception,Character[])> GetCharactersAsync(IList<string> pixiesIds,bool hideUIProcess = false);
        Task<(Exception,Character)> GetCharacterAsync(string pixiesId,bool hideUIProcess = false);
        Task<(Exception,string[])> GetCharactersNameAsync(IList<string> pixiesIds,bool hideUIProcess = false);
        Task<(Exception,BattleSimulationData)> GetBattleSimulationAsync(string battleId,bool hideUIProcess = false);
    }
}