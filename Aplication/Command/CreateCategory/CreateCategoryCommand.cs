using ApiBootcampCLT.Api.Request;
using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Command.CreateCategory
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryResponse>;
}
