using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Evesoft;
using UnityEngine.Networking;
using System.Net;
using System.Net.Http;
using NFTGame.Utils;

namespace NFTGame.WebAPI
{
    public static class WebRequestExtend
    {
        public static async Task<T> HttpRequestGet<T>(this string url,IDictionary<string,string> headers = null) {     
            try
            {
                var request = UnityWebRequest.Get(url);
            
                //SetCustom Headers
                if(!headers.IsNullOrEmpty()){
                    foreach (var item in headers){   
                        request.SetRequestHeader(item.Key,item.Value);
                    }
                }

                await request.SendWebRequest();
                            
                if (request.result == UnityWebRequest.Result.ConnectionError){
                    return default(T);
                }else{
                    var jsonRespone = request.downloadHandler.text;
                    (var value,var ex) = jsonRespone.FromJsonUnity<T>();
                    if(!ex.IsNull()){
                        ex.Message.LogError();
                        return default(T);
                    }else{
                        return value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ex.Message.LogError();
                return default(T);
            }
        }
        public static async Task<T> HttpRequestPost<T>(this string url,string json = null,IDictionary<string,string> headers = null){     
            try
            {
                var request = new UnityWebRequest(url, "POST");
                var jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                //SetCustom Headers
                if(!headers.IsNullOrEmpty()){
                    foreach (var item in headers){   
                        request.SetRequestHeader(item.Key,item.Value);
                    }
                }

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError){
                    return default(T);
                }else{
                    var jsonRespone = request.downloadHandler.text;
                    (var value,var ex) = jsonRespone.FromJsonUnity<T>();
                    if(!ex.IsNull()){
                        ex.Message.LogError();
                        return default(T);
                    }else{
                        return value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ex.Message.LogError();
                return default(T);
            }
        }
        public static async Task<T> HttpRequestPostFile<T>(this string url,string fileName,byte[] data,IDictionary<string,string> headers = null){     
            try
            {
                var formData = new List<IMultipartFormSection>();
                    formData.Add(new MultipartFormFileSection(fileName,data));

                var request = UnityWebRequest.Post(url,formData);
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError){
                    return default(T);
                }else{
                    var jsonRespone = request.downloadHandler.text;
                    (var value,var ex) = jsonRespone.FromJsonUnity<T>();
                    if(!ex.IsNull()){
                        ex.Message.LogError();
                        return default(T);
                    }else{
                        return value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ex.Message.LogError();
                return default(T);
            }
        }
        public static async Task<T> HttpRequestPut<T>(this string url,string bodyData,IDictionary<string,string> headers = null) {     
            try
            {
                var request = UnityWebRequest.Put(url,bodyData);
                
                //SetCustom Headers
                if(!headers.IsNullOrEmpty()){
                    foreach (var item in headers){   
                        request.SetRequestHeader(item.Key,item.Value);
                    }
                }

                await request.SendWebRequest();
                            
                if (request.result == UnityWebRequest.Result.ConnectionError){
                    return default(T);
                }else{
                    var jsonRespone = request.downloadHandler.text;
                    (var value,var ex) = jsonRespone.FromJsonUnity<T>();
                    if(!ex.IsNull()){
                        ex.Message.LogError();
                        return default(T);
                    }else{
                        return value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                 ex.Message.LogError();
                 return default(T);
            }
            
        }   
    }
}