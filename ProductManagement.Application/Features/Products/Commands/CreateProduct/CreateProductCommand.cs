using MediatR;
using ProductManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Application.Features.Products.Commands.CreateProduct
{
    // IRequest<DönüşTipi> şeklinde tanımlanır.

    // gönderilen dokumanda Result Pattern istendiği için ServiceResponse dönüyoruz.
    public class CreateProductCommand : IRequest<ServiceResponse<Guid>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
