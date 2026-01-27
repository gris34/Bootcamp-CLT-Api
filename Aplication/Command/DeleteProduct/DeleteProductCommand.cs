using MediatR;

namespace ApiBootcampCLT.Aplication.Command.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;
}
