using System;
using Evesoft;

namespace NFTGame.WebAPI
{
    [Serializable]
    public class WebAPIUniqRespone
    {
        public bool status;
    }

    [Serializable]
    public class WebAPIUniqRespone<T> : WebAPIUniqRespone where T : UniqDataResult
    {
        public UniqData<T> data;
        public uint[] tokenIds{
            get{
                if(data.IsNull())
                    return default(uint[]);

                if(data.result.IsNullOrEmpty())
                    return default(uint[]);

                return data.result[0].tokendIds;
            }
        }
    }
    
    [Serializable]
    public class UniqData<T> where T : UniqDataResult
    {
        public int totalItems;
        public T[] result;
    }

    [Serializable]
    public abstract class UniqDataResult{
        public string fullName;
        public int id;
        public string username;
        public string user_public_address;
        public string email;
        public bool is_ban;

        public abstract uint[] tokendIds{get;}
    }

    [Serializable]
    public class UniqDataCollectionResult : UniqDataResult
    {
        public Data.RawUniqCollection[] user_collectible_balance_info;


        private uint[] _tokendIds;
        public override uint[] tokendIds {
            get{
                if(user_collectible_balance_info.IsNullOrEmpty())
                    return default(uint[]);

                if(_tokendIds.IsNullOrEmpty()){
                    _tokendIds = new uint[user_collectible_balance_info.Length];
                    for (var i = 0; i < _tokendIds.Length; i++)
                    {
                        uint.TryParse(user_collectible_balance_info[i].tokenId,out _tokendIds[i]);
                    }
                }

                return _tokendIds;
            }
        }
    }

    [Serializable]
    public class UniqDataOnSaleResult : UniqDataResult
    {
        public Data.RawUniqCollection[] user_collectible_saleinfo;

        private uint[] _tokendIds;
        public override uint[] tokendIds {
            get{
                if(user_collectible_saleinfo.IsNullOrEmpty())
                    return default(uint[]);

                if(_tokendIds.IsNullOrEmpty()){
                    _tokendIds = new uint[user_collectible_saleinfo.Length];
                    for (var i = 0; i < _tokendIds.Length; i++)
                    {
                        uint.TryParse(user_collectible_saleinfo[i].tokenId,out _tokendIds[i]);
                    }
                }
                
                return _tokendIds;
            }
        }
    }
}