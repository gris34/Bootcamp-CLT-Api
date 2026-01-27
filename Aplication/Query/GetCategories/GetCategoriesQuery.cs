using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Query.GetCategories
{
    public record GetCategoriesQuery : IRequest<List<CategoryResponse>>;
}
