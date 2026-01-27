using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Command.DeleteCategory
{
    public record DeleteCategoryCommand(int Id) : IRequest<bool>;
}
