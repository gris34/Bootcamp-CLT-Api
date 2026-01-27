using ApiBootcampCLT.Api.Request;
using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Aplication.Command.CreateProduct;
using ApiBootcampCLT.Aplication.Command.DeleteProduct;
using ApiBootcampCLT.Aplication.Command.UpdateProduct;
using ApiBootcampCLT.Aplication.Command.UpdateProductStatus;
using ApiBootcampCLT.Aplication.Query.GetProductById;
using ApiBootcampCLT.Aplication.Query.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class ProductController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IMediator mediator, ILogger<ProductController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la lista de productos.
    /// </summary>
    [HttpGet("v1/api/products")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductos()
    {
        _logger.LogInformation("Solicitud recibida: listar productos. Método={Metodo} Ruta={Ruta}", Request.Method, Request.Path);

        try
        {
            var products = await _mediator.Send(new GetProductsQuery());

            if (products == null || products.Count == 0)
            {
                _logger.LogWarning("No se encontraron productos. Cantidad={Cantidad}", 0);
                return NoContent();
            }

            _logger.LogInformation("Productos retornados correctamente. Cantidad={Cantidad}", products.Count);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al listar productos. Método={Metodo} Ruta={Ruta}", Request.Method, Request.Path);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
        }
    }

    /// <summary>
    /// Obtiene el detalle de un producto por su identificador.
    /// </summary>
    [HttpGet("v1/api/products/{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> GetProductById([FromRoute] int id)
    {
        _logger.LogInformation("Solicitud recibida: obtener producto por id. ProductId={ProductId}", id);

        try
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            if (product == null)
            {
                _logger.LogWarning("Producto no encontrado. ProductId={ProductId}", id);
                return NotFound();
            }

            _logger.LogInformation("Producto retornado correctamente. ProductId={ProductId}", id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener producto por id. ProductId={ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
        }
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    [HttpPost("v1/api/products")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> CreateProducto([FromBody] CreateProductRequest request)
    {
        _logger.LogInformation(
            "Solicitud recibida: crear producto. Codigo={Codigo} Nombre={Nombre} CategoriaId={CategoriaId}",
            request?.Codigo, request?.Nombre, request?.CategoriaId);

        try
        {
            var result = await _mediator.Send(new CreateProductCommand(request));

            _logger.LogInformation("Producto creado correctamente. ProductId={ProductId} Codigo={Codigo}", result.Id, result.Codigo);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear producto. Codigo={Codigo}", request?.Codigo);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza completamente un producto existente.
    /// </summary>
    [HttpPut("v1/api/products/{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> UpdateProducto([FromRoute] int id, [FromBody] UpdateProductRequest request)
    {
        _logger.LogInformation("Solicitud recibida: actualizar producto. ProductId={ProductId}", id);

        try
        {
            var result = await _mediator.Send(new UpdateProductCommand(id, request));

            _logger.LogInformation("Producto actualizado correctamente. ProductId={ProductId}", id);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Producto no encontrado para actualización. ProductId={ProductId}", id);
            return NotFound(new { Message = "No se ha encontrado el producto especificado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar producto. ProductId={ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
        }
    }

    /// <summary>
    /// Actualiza parcialmente un producto existente (estado).
    /// </summary>
    [HttpPatch("v1/api/products/{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> PatchProducto([FromRoute] int id, [FromBody] PatchProductRequest request)
    {
        _logger.LogInformation("Solicitud recibida: actualizar estado de producto. ProductId={ProductId} Activo={Activo}", id, request?.Activo);

        try
        {
            var result = await _mediator.Send(new UpdateProductStatusCommand(id, request.Activo));

            _logger.LogInformation("Estado de producto actualizado correctamente. ProductId={ProductId} Activo={Activo}", id, request.Activo);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Producto no encontrado para actualización de estado. ProductId={ProductId}", id);
            return NotFound(new { Message = "Producto no encontrado." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar estado de producto. ProductId={ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un producto existente.
    /// </summary>
    [HttpDelete("v1/api/products/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProducto([FromRoute] int id)
    {
        _logger.LogInformation("Solicitud recibida: eliminar producto. ProductId={ProductId}", id);

        try
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));

            if (!result)
            {
                _logger.LogWarning("Producto no encontrado para eliminación. ProductId={ProductId}", id);
                return NotFound(new { Message = "Producto no encontrado." });
            }

            _logger.LogInformation("Producto eliminado correctamente. ProductId={ProductId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar producto. ProductId={ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }
}
