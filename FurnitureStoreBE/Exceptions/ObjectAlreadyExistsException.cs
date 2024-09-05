using System.Net;

namespace FurnitureStoreBE.Exceptions
{
    public class ObjectAlreadyExistsException : BusinessException
    {
        public ObjectAlreadyExistsException(string message = "Object already exists") : base(message)
        {
            this.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
