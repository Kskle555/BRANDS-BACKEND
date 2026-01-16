using MediatR;
using ProductManagement.Domain.Common;
using System;

namespace ProductManagement.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid Id { get; set; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }
}