using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Features.Products.Commands.CreateProduct;
using ProductManagement.Application.Features.Products.Commands.DeleteProduct;
using ProductManagement.Application.Features.Products.Commands.UpdateProduct;
using ProductManagement.Application.Features.Products.Queries.GetAllProducts;
using ProductManagement.Infrastructure.Services;
using System.Threading.Tasks;

namespace ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly OpenAiService _aiService;

        public ProductsController(IMediator mediator,OpenAiService aiService)
        {
            _mediator = mediator;
            _aiService = aiService;
        }


        // GET işlemleri herkese açık olsun (Authorize bilerek eklemedim)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query)
        {
            // [FromQuery] kullanalim
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
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



        //sadece adminler silebilsin
        [Authorize(Roles = "Admin")]
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
        [Authorize]

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("generate-description")]
        public async Task<IActionResult> GenerateDescription([FromBody] string productName)
        {
            var description = await _aiService.GenerateProductDescription(productName);
            return Ok(new { description });
        }

        [HttpPost("generate-price")]
        public async Task<IActionResult> GeneratePrice([FromBody] string productName)
        {
            var price = await _aiService.GeneratePricePrediction(productName);
            return Ok(new { price = int.Parse(price) }); // Sayı olarak dönüyoruz
        }
    }
}