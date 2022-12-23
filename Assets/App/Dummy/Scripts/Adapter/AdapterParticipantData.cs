using System.Collections.Generic;
using Evesoft;

namespace NFTGame.Dummy.Adapter
{
    public static class AdapterParticipantData
    {
        public static Server.DatabaseDataParticipant ToDataDatabase(this ParticipantData data){
            if(data.address.IsNullOrEmpty() || data.tokenId.IsNullOrEmpty())
                return default(Server.DatabaseDataParticipant);

            var result = new Server.DatabaseDataParticipant(data.address,data.tokenId);
            return result;
        }
        public static Server.DatabaseDataParticipant[] ToDataDatabase(this IList<ParticipantData> data){
            if(data.IsNullOrEmpty())
                return default(Server.DatabaseDataParticipant[]);

            var result = new Server.DatabaseDataParticipant[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToDataDatabase();
            }
            return result;
        } 
        public static ParticipantData ToData(this Server.DatabaseDataParticipant data){
            if(data == null)
                return default(ParticipantData);

            return new ParticipantData(data.address,data.tokenId);
        }
        public static ParticipantData[] ToData(this IList<Server.DatabaseDataParticipant> data){
            if(data.IsNullOrEmpty())
                return default(ParticipantData[]);

            var result = new ParticipantData[data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = data[i].ToData();
            }
            return result;
        }
    }
}