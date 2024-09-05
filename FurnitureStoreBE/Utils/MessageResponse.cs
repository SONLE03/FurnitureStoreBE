using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;
using System.Text.Json;
namespace FurnitureStoreBE.Utils
{
    public class SuccessfulResponse<T>
    {
        public bool Success {  get; set; }
        public T? Data { get; set; }
        public int _StatusCode { get; set; }
        public string? Message { get; set; }

        public SuccessfulResponse(T? data, int statusCode, string? message)
        {
            this.Success = true;
            this.Data = data;
            this._StatusCode = statusCode;
            this.Message = message ?? "Success";
        }
        public IActionResult GetResponse()
        {
            var jsonResponse = JsonSerializer.Serialize(new
            {
                success = this.Success,
                status = (int)this._StatusCode,
                detail = this.Message,
                data = this.Data
            });
            return new ContentResult
            {
                StatusCode = (int)this._StatusCode,
                Content = jsonResponse,
                ContentType = "application/json"
            };
        }
    }
}
