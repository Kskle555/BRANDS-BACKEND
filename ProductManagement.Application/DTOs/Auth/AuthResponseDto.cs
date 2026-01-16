using System;

namespace ProductManagement.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; } // Token ne zaman ölecek?
        public string Role { get; set; }         // Admin mi User mı?
        public string FullName { get; set; }
    }
}