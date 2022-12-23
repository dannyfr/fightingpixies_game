using System;
namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public struct RawBattleSimulation
    {
        public string pixies_id_1;
        public string pixies_id_2;
        public string win_id;
        public string los_id;        
        public RawBattleStep[] steps;
    }
}