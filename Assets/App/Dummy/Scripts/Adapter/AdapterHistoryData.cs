using System.Collections.Generic;
using Evesoft;

namespace NFTGame.Dummy.Adapter
{
    public static class AdapterHistoryData
    {
        public static HistoryData ToData(this Server.DatabaseDataHistory data){
            if(data.IsNull())
                return default(HistoryData);

            //var simulation  =  data.simulation?.ToData();
            var participant =  data.participant?.ToData();
            return new HistoryData(data.id,data.date,data.room,data.winner,participant,data.claimed);
        }
        public static HistoryData[] ToData(this IList<Server.DatabaseDataHistory> data){
            if(data.IsNullOrEmpty())
                return default(HistoryData[] );

            var result = new HistoryData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData();
            }
            return result;
        }
    }
}