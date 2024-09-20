using System.Net;

namespace FurnitureStoreBE.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public BusinessException(string message) : base(message) 
        {
            this.Message = message;
            this.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        public BusinessException(string message, int statusCode) : base(message)
        {
            this.Message = message;
            this.StatusCode = statusCode;
        }
    }
}
