using System.Net;

namespace FurnitureStoreBE.Exceptions
{
    public class AppConfigException : BusinessException
    {
        public AppConfigException(string message = "AppConfig exception") : base(message)
        {
            this.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
