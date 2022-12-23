using System;

namespace NFTGame.WebAPI.Data
{
    [Serializable]
    public class RawHistory
    {
        public string id;
        public string room;
        public string winner;
        public bool claimed;
        public string updated_at;
        public RawParticipant[] participant;
    }
}