using System.Collections;
using System.Collections.Generic;
using Evesoft;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Test
{
    [HideMonoScript]
    public class TestInitedCharacter : SerializedMonoBehaviour
    {
        #region const
        const string grpReq = "Required";
        const string grpAction = "Actions";
        #endregion

        #region fields
        [SerializeField,BoxGroup(grpReq)]
        private IBlockChainAPI _blockChainAPI;

        [SerializeField,BoxGroup(grpReq)]
        private IWebAPI webAPI;

        [SerializeField,BoxGroup(grpReq)]
        private Dummy.Server.ServerDummy server;
        #endregion
        
        [Button,BoxGroup(grpAction)]
        public async void CreateCharacter(){
            (var ex,var token) = await _blockChainAPI.GetPixiesAsync(_blockChainAPI.account);
            if(!ex.IsNull()){
                ex.LogError();
                return;
            }
            
            if(token.IsNullOrEmpty())
                return;
            
            var character = default(Character);
            for (var i = 0; i < token.Length; i++)
            {
                (ex,character) = await webAPI.GetCharacterAsync(token[i].ToString());
                if(!character.IsNull()){
                    character.Dispose();
                }
                //Create new chatacters
                else{
                    $"Creating Character {token[i]}".Log();
                    var data = server.GetRandomCharacter();
                        data.id = token[i].ToString();

                    ex = await webAPI.AddCharacterAsync(data);
                    if(!ex.IsNull()){
                        ex.LogError();
                    }else{
                        $"Create Character {token[i]} completed".Log();
                    }
                    
                }
            }
        }
    }
}


