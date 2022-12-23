using System;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame.Dummy.Server
{
    [Serializable]
    public class DatabaseDataHistory
    {
        #region fields
        [DisplayAsString,TableColumnWidth(50,false)] 
        public string id;

        [DisplayAsString,TableColumnWidth(50,false)] 
        public DateTime date;

        [DisplayAsString,TableColumnWidth(50,false)] 
        public string room;

        [DisplayAsString,TableColumnWidth(50,false)]
        public BattleStatus status;

        [DisplayAsString,TableColumnWidth(50,false)] 
        public string winner;

        [DisplayAsString,TableColumnWidth(60,false)] 
        public bool claimed;
        
        [ListDrawerSettings(IsReadOnly = true,Expanded = true),FoldoutGroup("$participantGrp")] 
        public DatabaseDataParticipant[] participant;

        [FoldoutGroup("$simulationGrp"),HideLabel]
        public DatabaseDataBattleSimulation simulation;
        #endregion

        #region odin
        private string participantGrp {
            get{
                var count = 0;
                if(!participant.IsNullOrEmpty())
                    count = participant.Length;
                    
                return $"{count} Participant";
            }
        }
        private string simulationGrp {
            get{
                var count = 0;
                if(!simulation.IsNull() && !simulation.steps.IsNullOrEmpty())
                    count = simulation.steps.Length;
                    
                return $"{count} Simulation";
            }
        }
        #endregion

        private string _str;

        #region constructor
        public DatabaseDataHistory(){}
        public DatabaseDataHistory(string id,DateTime date,string room,BattleStatus status,string winner,DatabaseDataParticipant[] participant,DatabaseDataBattleSimulation simulation,bool claimed = false){
            this.id = id;
            this.date = date;
            this.room = room;
            this.status = status;
            this.winner = winner;
            this.participant = participant;
            this.claimed = claimed;
            this.simulation = simulation;
            this._str = $"{id}-{date}-{room}-{winner}-{participant.Join()}";
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return _str;
        }
        #endregion
    }
}