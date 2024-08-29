using System.Net;

namespace FurnitureStoreBE.Exceptions
{
    public class ObjectNotFoundException : BusinessException
    {
        public ObjectNotFoundException(string message = "Not found") : base(message) { 
           this.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
