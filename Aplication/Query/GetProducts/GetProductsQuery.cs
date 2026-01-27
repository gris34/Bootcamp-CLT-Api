using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Query.GetProducts
{
    public record GetProductsQuery() : IRequest<List<ProductResponse>>;
}
