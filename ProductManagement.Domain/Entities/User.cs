using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; } // Şifreyi hashleyip saklayacağız
        public string? Role { get; set; } 
    }
}
