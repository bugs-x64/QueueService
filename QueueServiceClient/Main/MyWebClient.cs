using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QueueServiceClient.Main
{
    class MyWebClient:HttpClient
    {
        public MyWebClient() { }

        public static StringContent BuildJSONContent(string jsonString)
        {
            var contentHttp = new StringContent(
                content: jsonString,
                encoding: Encoding.UTF8,
                mediaType: "application/json");
            return contentHttp;
        }


        public async Task<HttpResponseMessage> Get(Uri remoteAddress)
        {
            HttpRequestMessage newRequest = new HttpRequestMessage(new HttpMethod("GET"), remoteAddress);
            var response = await SendAsync(newRequest);
            return response;
        }

        public async Task<HttpResponseMessage> Post(Uri remoteAddress, StringContent httpContent)
        {
            HttpRequestMessage newRequest = new HttpRequestMessage(new HttpMethod("POST"), remoteAddress)
            {
                Content = httpContent
            };
            var response = await SendAsync(newRequest);
            return response;
        }
    }
}
