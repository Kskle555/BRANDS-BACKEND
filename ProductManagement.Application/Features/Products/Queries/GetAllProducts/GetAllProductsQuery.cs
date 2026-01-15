using MediatR;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Common;
using System.Collections.Generic;

namespace ProductManagement.Application.Features.Products.Queries.GetAllProducts
{
    // List<ProductDto> dönüyoruz
    public class GetAllProductsQuery : IRequest<ServiceResponse<List<ProductDto>>>
    {
        public int PageNumber { get; set; } = 1; // Varsayılan 1. sayfa
        public int PageSize { get; set; } = 10;  // Varsayılan 10 kayıt
    }
}