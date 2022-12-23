using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Evesoft;
using System.IO;
using System.Net.Http;
using System.Net;
using NFTGame.Utils;

namespace NFTGame.Editor
{
    [HideReferenceObjectPicker]
    public class UploadBundleTool
    {
        #region fields
        [SerializeField]
        private string url;

        [ShowIf(nameof(ShowUpload)),SerializeField,DisplayAsString,ReadOnly,ListDrawerSettings(Expanded = true)]
        private string[] files;
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
        private void SelectFolder(){
            var path =  EditorUtility.OpenFolderPanel("Select Folder",Application.dataPath,"");
            if(path.IsNullOrEmpty())
                return;

            files = Directory.GetFiles(path);
        }   

        private bool ShowUpload(){
            return !files.IsNullOrEmpty();
        }

        [ShowIf(nameof(ShowUpload)),Button(ButtonSizes.Medium)]
        private async void Upload(){
            if(files.IsNullOrEmpty())
                return;
                
            var progress = 0;
            var total    = files.Length;
            var title    = "Uploading Assets";

            for (var i = 0; i < files.Length; i++)
            {
                progress++;
                if(!files[i].FileExist())
                    continue;
                    
                var fileName = Path.GetFileName(files[i]);
                var fStream  = File.Open(files[i], FileMode.Open, FileAccess.Read);

                EditorUtility.DisplayProgressBar(title, $"{fileName} ({progress}/{total})", progress/(float)total);

                var content = new MultipartFormDataContent();      
                    content.Add(new StreamContent(fStream, (int)fStream.Length), "file", fileName);
                
                var request = await client.PostAsync(url,content);
                var respone = await request.Content.ReadAsStringAsync();
                ProcessRespone(fileName,respone);
            }

            EditorUtility.ClearProgressBar();
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


