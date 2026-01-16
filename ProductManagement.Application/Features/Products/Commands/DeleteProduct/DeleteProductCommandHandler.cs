using MediatR;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManagement.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ServiceResponse<bool>>
    {
        private readonly IGenericRepository<Product> _productRepository;

        public DeleteProductCommandHandler(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);

            if (product == null)
            {
                return ServiceResponse<bool>.ErrorResponse("Ürün bulunamadı.");
            }

            // Repository'deki DeleteAsync metodumuz zaten Soft Delete (IsDeleted = true) yapıyor.
            await _productRepository.DeleteAsync(product);

            return ServiceResponse<bool>.SuccessResponse(true, "Ürün başarıyla silindi.");
        }
    }
}