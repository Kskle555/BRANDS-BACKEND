using AutoMapper;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Features.Products.Commands.CreateProduct;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Mappings
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<CreateProductCommand, Product>(); // Birazdan yazacağımız Command için
        }
    }
}