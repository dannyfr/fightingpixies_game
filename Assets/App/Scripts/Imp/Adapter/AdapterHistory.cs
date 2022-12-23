using System;
using NFTGame.WebAPI.Data;
using Evesoft;
using System.Collections.Generic;

namespace NFTGame
{
    public static class AdapterHistory
    {
        public static HistoryData ToData(this RawHistory data){
            if(data.IsNull())
                return default(HistoryData);

            var participant = default(ParticipantData[]);
            if(!data.participant.IsNullOrEmpty()){
                participant = new ParticipantData[data.participant.Length];
                for (var i = 0; i < participant.Length; i++)
                {
                    participant[i] = new ParticipantData(data.participant[i].address,data.participant[i].pixies_id);
                }
            }
            
            var date = default(DateTime);
            DateTime.TryParse(data.updated_at,out date);
            var result = new HistoryData(data.id,date,data.room,data.winner,participant,data.claimed);
            return result;
        }
        public static HistoryData[] ToData(this IList<RawHistory> data){
            if(data.IsNullOrEmpty())
                return default(HistoryData[]);

            var result = new HistoryData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData();
            }
            return result;
        }
    }
}