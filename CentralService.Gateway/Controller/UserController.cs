using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Net.Http;
using System.Threading.Tasks;
using CentralService.Gateway.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using CentralService.Gateway.Dto.User;
using System.Net.Http.Json;

namespace CentralService.Gateway.Controller
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private string _Url;
        private HttpClient _Client;

        public UserController(IOptions<List<Microservice>> Options)
        {
            Microservice Service = Options.Value.FirstOrDefault(x => x.Name == "User");
            if (Service == null)
                throw new ArgumentNullException(nameof(Service), "No microservice options could be found for the user microservice.");
            _Url = $"{ Service.Config.URL }{ Service.Config.Endpoint }";

            _Client = new HttpClient();
            _Client.DefaultRequestHeaders.Accept.Clear();
            _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUser(string Nickname, string Email, bool IsVerified)
        {
            try
            {
                HttpResponseMessage Response = await _Client.GetAsync($"{ _Url }?Nickname={ Nickname }&Email={ Email }&IsVerified={ IsVerified }");
                if (Response.IsSuccessStatusCode)
                {
                    string ResponseMessage = await Response.Content.ReadAsStringAsync();
                    return Ok(ResponseMessage);
                }
                return StatusCode((int)Response.StatusCode);
            }
            catch (Exception Ex)
            {
                if (Ex is ArgumentException || Ex is ArgumentNullException)
                    return BadRequest();
                return StatusCode(500);
            }
        }

        [HttpPost("link")]
        [Authorize]
        public async Task<ActionResult> LinkDeviceToUser(DeviceLinkRequest Request)
        {
            try
            {
                JsonContent Content = JsonContent.Create(Request);
                HttpResponseMessage Response = await _Client.PostAsync($"{ _Url }/link", Content);
                if (Response.IsSuccessStatusCode)
                    return Ok();
                string ResponseMessage = await Response.Content.ReadAsStringAsync();
                return StatusCode((int)Response.StatusCode, ResponseMessage);
            }
            catch (Exception Ex)
            {
                if (Ex is ArgumentException || Ex is ArgumentNullException)
                    return BadRequest();
                return StatusCode(500);
            }
        }

        [HttpDelete("unlink")]
        [Authorize]
        public async Task<ActionResult> UnlinkDeviceFromUser(string Email, string DeviceId, string MacAddress)
        {
            try
            {
                JsonContent Content = JsonContent.Create(Request);
                HttpResponseMessage Response = await _Client.DeleteAsync($"{ _Url }/unlink?Email={ Email }&DeviceId={ DeviceId }&MacAddress={ MacAddress }");
                if (Response.IsSuccessStatusCode)
                    return Ok();
                string ResponseMessage = await Response.Content.ReadAsStringAsync();
                return StatusCode((int)Response.StatusCode, ResponseMessage);
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
