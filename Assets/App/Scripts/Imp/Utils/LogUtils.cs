using System;
using Evesoft;


namespace NFTGame.Utils
{
    public static class LogUtils
    {
        public static T LogException<T>(this T target,string scope) where T: Exception
        {
            $"<Fail> {scope} - {typeof(T).Name} {target.Message}".LogError();
            return target;
        }
        public static T LogException<T>(this T target,UnityEngine.Object obj,string scope) where T: Exception
        {
            $"<Fail> {obj.name} - {scope} - {typeof(T).Name} {target.Message}".LogError();
            return target;
        }
        public static void LogRun(this string message){
            $"<Run> {message}".Log();
        }
        public static void LogRun(this UnityEngine.Object target,string scope){
            LogRun($"{target.name} - {scope}");
        }
        public static void LogCompleted(this UnityEngine.Object target,string scope){
            LogCompleted($"{target.name} - {scope}");
        }
        public static void LogCompleted(this string message){
            $"<Complete> {message}".Log();
        }
    }
}