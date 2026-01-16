using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Features.Products.Commands.CreateProduct;
using ProductManagement.Application.Features.Products.Commands.DeleteProduct;
using ProductManagement.Application.Features.Products.Queries.GetAllProducts;
using System.Threading.Tasks;

namespace ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            // MediatR kütüphanesi, gelen bu komutu (Command) ilgili Handler'a yönlendirir.
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query)
        {
            // [FromQuery] kullanalim
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}