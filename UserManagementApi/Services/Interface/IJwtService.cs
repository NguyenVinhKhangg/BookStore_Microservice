using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementApi.Models;

namespace UserManagementApi.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(Users user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
