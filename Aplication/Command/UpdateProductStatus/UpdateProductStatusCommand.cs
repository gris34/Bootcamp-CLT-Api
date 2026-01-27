using ApiBootcampCLT.Api.Response;
using MediatR;

namespace ApiBootcampCLT.Aplication.Command.UpdateProductStatus
{
    public record UpdateProductStatusCommand(int Id, bool? Activo) : IRequest<ProductResponse>;
}
