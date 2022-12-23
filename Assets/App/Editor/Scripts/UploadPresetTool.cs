using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Evesoft;
using System.Net;
using System.Net.Http;
using NFTGame.WebAPI;
using NFTGame.Utils;

namespace NFTGame.Editor
{
    [HideReferenceObjectPicker]
    public class UploadPresetTool
    {
        #region fields
        [SerializeField]
        private string urlSetPreset;

        [SerializeField]
        private string urlSetParts;

        [SerializeField]
        private string urlSetSkills;

        [SerializeField]
        private string urlClearData;

        [SerializeField,InlineEditor]
        private Config.CharacterBuilderConfig config;

        [SerializeField,InlineEditor]
        private ISkill[] skills;
        #endregion

        #region private
        private HttpClient client {
            get{
                if(_client.IsNull())
                    _client = new HttpClient();

                return _client;
            }
        }
        private HttpClient _client;
        #endregion

        #region methods
        [Button(ButtonSizes.Medium)]
        private async void Upload(){
            var progress = 0;
            var total    = config.IsNull() || urlSetPreset.IsNullOrEmpty() || config.characters.IsNullOrEmpty()? 0 : config.characters.Count;
                total   += config.IsNull() || urlSetParts.IsNullOrEmpty()  || config.characters.IsNullOrEmpty()? 0 : (config.characters.Count * 10);
                total   += urlSetSkills.IsNullOrEmpty() || skills.IsNullOrEmpty() ? 0 : skills.Length;
            
            var title    = "Uploading";
            var current  = default(string);
            var content  = default(HttpContent);
            var request  = default(HttpResponseMessage);
            var respone  = default(string);

            var GetPresetString = default(Func<string,string>);
                GetPresetString = (id)=>{
                    return $"Preset - {id}"; 
                };

            var GetPartString = default(Func<string,string>);
                GetPartString = (id)=>{
                    return $"Part - {id}"; 
                };

            var GetSkillString = default(Func<string,string>);
                GetSkillString = (id)=>{
                    return $"Skill - {id}"; 
                };

            #region preset
            if(!urlSetPreset.IsNullOrEmpty() || !config.IsNull() || !config.characters.IsNullOrEmpty()){
                for (var i = 0; i < config.characters.Count; i++)
                {   
                    progress++; 
                    current = GetPresetString(config.characters[i].id);
                    EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);

                    content = config.characters[i].ToHttpContent();
                    request = await client.PostAsync(urlSetPreset,content);
                    respone = await request.Content.ReadAsStringAsync();
                    ProcessRespone(current,respone);

                    if(urlSetParts.IsNullOrEmpty())
                        continue;

                    #region Limbs
                    if(config.characters[i].arm){
                        progress++;
                        current = GetPartString(config.characters[i].arm.id);
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].arm.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    if(config.characters[i].body){
                        progress++;
                        current = GetPartString(config.characters[i].body.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].body.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    if(config.characters[i].head){
                        progress++;
                        current = GetPartString(config.characters[i].head.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].head.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    if(config.characters[i].leg){
                        progress++;
                        current = GetPartString(config.characters[i].leg.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].leg.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }
                    #endregion

                    #region accsessories
                    progress++;
                    if(config.characters[i].cloth){
                        current = GetPartString(config.characters[i].cloth.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].cloth.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    progress++;
                    if(config.characters[i].facialHair){     
                        current = GetPartString(config.characters[i].facialHair.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].facialHair.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    progress++;
                    if(config.characters[i].helmet){
                        current = GetPartString(config.characters[i].helmet.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].helmet.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    progress++;
                    if(config.characters[i].pants){
                        current = GetPartString(config.characters[i].pants.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].pants.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    progress++;
                    if(config.characters[i].sleeve){
                        current = GetPartString(config.characters[i].sleeve.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].sleeve.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }

                    progress++;
                    if(config.characters[i].weapon){
                        current = GetPartString(config.characters[i].weapon.id); 
                        EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);
                        content = config.characters[i].weapon.ToHttpContent();
                        request = await client.PostAsync(urlSetParts,content);
                        respone = await request.Content.ReadAsStringAsync();
                        ProcessRespone(current,respone);
                    }
                    #endregion
                }
            }
            #endregion

            #region skills
            if(!urlSetSkills.IsNullOrEmpty() && !skills.IsNullOrEmpty()){
                for (var i = 0; i < skills.Length; i++)
                {
                    progress++; 
                    current = GetSkillString(skills[i].id); 
                    EditorUtility.DisplayProgressBar(title, $"{current} ({progress}/{total})", progress/(float)total);

                    content = skills[i].ToHttpContent();
                    request = await client.PostAsync(urlSetSkills,content);
                    respone = await request.Content.ReadAsStringAsync();
                    ProcessRespone(current,respone);
                }
            }
            #endregion

            EditorUtility.ClearProgressBar();
        }
        
        [Button(ButtonSizes.Medium)]
        private async void ClearAll(){
            if(UnityEditor.EditorUtility.DisplayDialog("Delete All Database","Are You Sure want to delete all database? this is irreversible","Yes","No")){
                var request = await client.DeleteAsync(urlClearData);
                var respone = await request.Content.ReadAsStringAsync();
                ProcessRespone("ClearData",respone);
            }
        }
        private void ProcessRespone(string id,string respone){
            (var dic,var ex) = respone.FromJson<Dictionary<string,object>>();
            if(!ex.IsNull())
                return;

            var key = "status";
            if(!dic.ContainsKey(key))
                return;

            var statusCode = (HttpStatusCode)dic[key].ToInt32();
            if(statusCode == HttpStatusCode.OK){
                $"{statusCode} - {id}".LogCompleted();
            }else{
                var message = dic["message"];
                $"{statusCode} - {id} - {message}".LogError();
            }
        }
        #endregion
    }
}


