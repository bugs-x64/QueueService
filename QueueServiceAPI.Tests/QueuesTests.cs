using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace QueueServiceAPI.Tests
{
    public class QueuesTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public QueuesTests()
        {
            _server = new TestServer(new WebHostBuilder()
               .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        //получение информации об очередях
        [Fact]
        public async Task GetQueues()
        {
            //array
            var fio = "evsey";//check your DB

            //act
            var response1 = await _client.GetAsync("/api/queues");
            var responseContent1 = await response1.Content.ReadAsStringAsync();

            //assert
            response1.EnsureSuccessStatusCode();
            Assert.Contains($"\"fio\":\"{fio}\",\"competing\":true}}", responseContent1);
        }

        [Fact]
        public async Task Enqueue()
        {
            //array
            var fio1 = "evsey";
            var fio2 = "другойevsey";
            var competing = true;
            var notcompeting = false;

            //act
            var response1 = await _client.PostAsync(
               requestUri: $"/api/queues/?c={competing}",
               content: new StringContent(
                   content: $"\"{fio1}\"",
                   encoding: Encoding.UTF8,
                   mediaType: "application/json"
               ));
            var response2 = await _client.PostAsync(
               requestUri: $"/api/queues/?c={notcompeting}",
               content: new StringContent(
                   content: $"\"{fio2}\"",
                   encoding: Encoding.UTF8,
                   mediaType: "application/json"
               ));


            //assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PickFromQueue()
        {
            //array
            var empoyeeid = 1;//check your DB
            var queueid = 2;//check your DB

            //act
            var response1 = await _client.PutAsync(
               requestUri: $"/api/queues/{queueid}",
               content: new StringContent(
                   content: $"{empoyeeid}",
                   encoding: Encoding.UTF8,
                   mediaType: "application/json"
               ));

            //assert
            response1.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PickNextFromQueue()
        {
            //array
            var empoyeeid = 1;

            //act
            var response1 = await _client.PostAsync(
               requestUri: $"/api/queues/next",
               content: new StringContent(
                   content: $"{empoyeeid}",
                   encoding: Encoding.UTF8,
                   mediaType: "application/json"
               ));

            //assert
            response1.EnsureSuccessStatusCode();
        }


    }
}
