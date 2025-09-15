using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetLink);
    }
}
