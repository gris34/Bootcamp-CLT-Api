using ApiBootcampCLT.Api.Request;
using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Command.CreateProduct
{
    public record CreateProductCommand(CreateProductRequest ProductRequest) : IRequest<ProductResponse>;
}
