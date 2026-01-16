using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Domain.Common;
using System.Threading.Tasks;

namespace ProductManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto request);
        Task<ServiceResponse<bool>> RegisterAsync(RegisterDto request);
    }
}