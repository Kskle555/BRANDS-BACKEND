using AutoMapper;
using MediatR;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManagement.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ServiceResponse<Guid>>
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IGenericRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. AutoMapper ile Command'ı Entity'ye çevir
            var product = _mapper.Map<Product>(request);

            // 2. Repository ile veritabanına ekle
            await _productRepository.AddAsync(product);

            // 3. Başarılı sonuç dön (Oluşan ID ile birlikte)
            return ServiceResponse<Guid>.SuccessResponse(product.Id, "Ürün başarıyla eklendi.");
        }
    }
}