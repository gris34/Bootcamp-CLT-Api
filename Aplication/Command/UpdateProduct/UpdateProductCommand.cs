using ApiBootcampCLT.Api.Request;
using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Command.UpdateProduct
{
    public record UpdateProductCommand(int Id, UpdateProductRequest ProductRequest) : IRequest<ProductResponse>;
}
