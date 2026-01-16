using MediatR;
using ProductManagement.Domain.Common;
using System;

namespace ProductManagement.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid Id { get; set; } // Hangi ürünü güncelleyeceğiz?
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}