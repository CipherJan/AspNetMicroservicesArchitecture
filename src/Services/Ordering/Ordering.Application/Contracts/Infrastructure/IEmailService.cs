using Ordering.Application.Models;
using System.Threading.Tasks;

namespace Ordering.Application.Contracts.Infrastructure
{
    internal interface IEmailService
    {
        Task<string> SendEmailAsync(Email email);
    }
}
