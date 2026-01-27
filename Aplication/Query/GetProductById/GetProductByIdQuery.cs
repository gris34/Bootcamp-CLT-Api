using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Query.GetProductById
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductResponse?>;
 
}