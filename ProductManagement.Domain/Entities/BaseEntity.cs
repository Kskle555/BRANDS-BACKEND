using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Default 
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft Delete için
    }
}
