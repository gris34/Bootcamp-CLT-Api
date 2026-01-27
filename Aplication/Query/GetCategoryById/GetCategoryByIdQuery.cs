using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Query.GetCategoryById
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryResponse>;
}
