using MediatR;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManagement.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ServiceResponse<bool>>
    {
        private readonly IGenericRepository<Product> _productRepository;

        public UpdateProductCommandHandler(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            //  Ürünü bul
            var product = await _productRepository.GetByIdAsync(request.Id);

            // 2. Ürün yoksa hata dön
            if (product == null)
            {
                return ServiceResponse<bool>.ErrorResponse("Güncellenecek ürün bulunamadı.");
            }

            // Verileri güncelle
            // AutoMapper da kullanılabilir ama burada manuel atama daha güvenli 
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;

            // Veritabanına kaydet 
            await _productRepository.UpdateAsync(product);

            return ServiceResponse<bool>.SuccessResponse(true, "Ürün başarıyla güncellendi.");
        }
    }
}