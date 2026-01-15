using AutoMapper;
using MediatR;
using ProductManagement.Application.DTOs;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManagement.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ServiceResponse<List<ProductDto>>>
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IGenericRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            // Repository'e eklediğimiz sayfalama metodunu çağırıyoruz
            var products = await _productRepository.GetPagedReponseAsync(request.PageNumber, request.PageSize);

            // Entity -> DTO dönüşümü
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // Cevabı dönüyoruz
            return ServiceResponse<List<ProductDto>>.SuccessResponse(productDtos);
        }
    }
}