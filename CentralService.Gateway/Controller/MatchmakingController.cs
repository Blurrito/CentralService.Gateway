using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CentralService.Gateway.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CentralService.Gateway.Controller
{
    [ApiController]
    [Route("server")]
    public class MatchmakingController : ControllerBase
    {
        private string _Url;
        private HttpClient _Client;

        public MatchmakingController(IOptions<List<Microservice>> Options)
        {
            Microservice Service = Options.Value.FirstOrDefault(x => x.Name == "Matchmaking");
            if (Service == null)
                throw new ArgumentNullException(nameof(Service), "No microservice options could be found for the user microservice.");
            _Url = $"{ Service.Config.URL }{ Service.Config.Endpoint }";

            _Client = new HttpClient();
            _Client.DefaultRequestHeaders.Accept.Clear();
            _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpGet]
        public async Task<ActionResult> GetServerList()
        {
            try
            {
                HttpResponseMessage Response = await _Client.GetAsync(_Url);
                if (Response.IsSuccessStatusCode)
                {
                    string Content = await Response.Content.ReadAsStringAsync();
                    return Ok(Content);
                }
                else
                    return StatusCode((int)Response.StatusCode);
            }
            catch (Exception Ex)
            {
                if (Ex is ArgumentException || Ex is ArgumentNullException)
                    return BadRequest();
                return StatusCode(500);
            }
        }
    }
}
