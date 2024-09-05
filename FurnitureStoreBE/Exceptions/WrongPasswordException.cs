using System.Net;

namespace FurnitureStoreBE.Exceptions
{
    public class WrongPasswordException : BusinessException
    {
        public WrongPasswordException(string message = "Wrong password") : base(message)
        {
            this.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
