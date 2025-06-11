using System.Net.Http.Headers;

namespace Sample.SP0.Client.Core.WebApi
{
    public class Response<T>
    {
        public HttpResponseHeaders? Headers;
        public T? Body;
    }
}
