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
    //TODO: написать тесты для очередей
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

        [Fact]
        public async Task GetUsers()
        {
            //array
            var name = "евсей";
            var id = 1;

            //act
            //all
            var response1 = await _client.GetAsync("/api/clients");
            var responseContent1 = await response1.Content.ReadAsStringAsync();
            //by name
            var response2 = await _client.GetAsync("/api/clients/?fio=" + name);
            var responseContent2 = await response2.Content.ReadAsStringAsync();
            //by id
            var response3 = await _client.GetAsync("/api/clients/" + id);
            var responseContent3 = await response3.Content.ReadAsStringAsync();


            //assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            response3.EnsureSuccessStatusCode();
            Assert.Contains($"{{\"id\":{id},\"fio\":\"{name}\"}}", responseContent1);
            Assert.Contains($"{{\"id\":{id},\"fio\":\"{name}\"}}", responseContent2);
            Assert.Contains($"{{\"id\":{id},\"fio\":\"{name}\"}}", responseContent3);

        }

    }
}
