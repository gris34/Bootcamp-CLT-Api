using ApiBootcampCLT.Api.Request;
using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Command.UpdateCategory
{
    public record UpdateCategoryCommand(UpdateCategoryRequest Request, int Id) : IRequest<CategoryResponse?>;
}
