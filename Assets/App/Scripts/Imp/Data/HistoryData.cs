using System;
using Sirenix.OdinInspector;
using Evesoft;

namespace NFTGame
{
    [Serializable,HideReferenceObjectPicker]
    public struct HistoryData
    {
        #region fields
        [DisplayAsString,TableColumnWidth(50,false)] 
        public string id;

        [DisplayAsString,TableColumnWidth(50,false)]
        public DateTime date;

        [DisplayAsString,TableColumnWidth(50,false)] 
        public string room;

        [DisplayAsString,TableColumnWidth(50,false)] 
        public string winner;

        [DisplayAsString,TableColumnWidth(60,false)] 
        public bool claimed;
        
        [ListDrawerSettings(IsReadOnly = true,Expanded = true),FoldoutGroup("$participantGrp")] 
        public ParticipantData[] participant;
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
        #endregion

        #region private
        private string _str;
        #endregion

        #region constructor
        public HistoryData(string id,DateTime date,string room,string winner,ParticipantData[] participant,bool claimed = false){
            this.id = id;
            this.date = date;
            this.room = room;
            this.winner = winner;
            this.participant = participant;
            this.claimed = claimed;
            this._str = $"{id}-{room}-{winner}-{participant.Join()}";
        }
        #endregion

        #region methods
        public bool IsWon(string address){
            if(participant?.Length != 0){
                for (var i = 0; i < participant.Length; i++)
                {
                    if(winner == participant[i].tokenId && participant[i].address == address)
                        return true;
                }
            }

            return false;  
        }
        public string GetReward(string address){
            var tokenId = default(string);
            var opponent= default(string);

            if(participant?.Length != 0){
                if(participant[0].address == address){
                    tokenId = participant[0].tokenId;
                    opponent= participant[1].tokenId;

                    if(tokenId == winner){
                        return opponent;
                    }
                    
                }
                
                if(participant[1].address == address){
                    tokenId = participant[1].tokenId;
                    opponent= participant[0].tokenId;

                    if(tokenId == winner){
                        return opponent;
                    }
                }
            }

            

            return null;
        }
        public void GetTokenAndOpponent(string address,out string tokenId,out string opponent){
            opponent = default(string);
            tokenId  = default(string);

            if(participant?.Length != 0){
                if(participant[0].address == address){
                    tokenId = participant[0].tokenId;
                    opponent= participant[1].tokenId;
                }else if(participant[1].address == address){
                    tokenId = participant[1].tokenId;
                    opponent= participant[0].tokenId;
                }
            }
        }
        public override string ToString()
        {
            return _str;
        }
        #endregion
    }
}