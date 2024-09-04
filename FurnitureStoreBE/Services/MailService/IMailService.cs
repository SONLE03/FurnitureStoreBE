using FurnitureStoreBE.DTOs.Request.MailRequest;

namespace FurnitureStoreBE.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
