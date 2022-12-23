using System.Net;
using System;

namespace NFTGame.WebAPI
{
    [Serializable]
    public class WebAPIRespone
    {
        public HttpStatusCode status = HttpStatusCode.OK;
        public string message;
    }

    [Serializable]
    public class WebAPIRespone<T> : WebAPIRespone
    {
        public T result;
    }
}