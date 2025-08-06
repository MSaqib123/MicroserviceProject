using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Xml.Linq;
using static Mango.Web.Models.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            //📦 Dependencies
            //This factory creates an HttpClient instance(named "MangoAPI") for making requests.
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                //🧱 Step 1: Create HttpClient
                //Uses a named client (MangoAPI) that must be configured in Program.cs or Startup.cs.
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");

                //🧾 Step 2: Prepare the HTTP Request
                //Sets the request to accept JSON responses
                //Sets the request URL from the RequestDto
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(requestDto.Url);

                //📤 Step 3: Add Body (for POST/PUT)
                //If data is passed, it serializes it to JSON and adds it to the request body.
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                //🧭 Step 4: Set HTTP Method
                //Dynamically sets HTTP method (GET, POST, PUT, DELETE) based on ApiType enum in RequestDto.
                HttpResponseMessage? apiResponse = null;
                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                //📡 Step 5: Send the Request
                //Sends the HTTP request asynchronously and stores the response.
                apiResponse = await client.SendAsync(message);

                //✅ Step 6: Handle the Response Return
                //If it's an error: returns a standard ResponseDto with failure message.
                //Otherwise: reads the content, deserializes into ResponseDto, and returns it.
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
