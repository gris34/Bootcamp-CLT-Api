using ApiBootcampCLT.Api.Request;
using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Aplication.Command.CreateCategory;
using ApiBootcampCLT.Aplication.Command.DeleteCategory;
using ApiBootcampCLT.Aplication.Command.UpdateCategory;
using ApiBootcampCLT.Aplication.Query.GetCategories;
using ApiBootcampCLT.Aplication.Query.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiBootcampCLT.Api
{
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IMediator mediator, ILogger<CategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la lista de categorías.
        /// </summary>
        /// <returns>Listado de categorías.</returns>
        [HttpGet("v1/api/categories")]
        [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoriesAsync()
        {
            _logger.LogInformation("Solicitud recibida: listar categorías. Método={Metodo} Ruta={Ruta}", Request.Method, Request.Path);

            try
            {
                var query = new GetCategoriesQuery();
                var categories = await _mediator.Send(query);

                if (categories == null || categories.Count == 0)
                {
                    _logger.LogWarning("No se encontraron categorías activas. Cantidad={Cantidad}", 0);
                    return NoContent();
                }

                _logger.LogInformation("Categorías retornadas correctamente. Cantidad={Cantidad}", categories.Count);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al listar categorías.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
            }
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="request">Datos de la categoría a crear.</param>
        /// <returns>Categoría creada.</returns>
        [HttpPost("v1/api/categories")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            _logger.LogInformation("Solicitud recibida: crear categoría. Nombre={Nombre}", request?.Nombre);

            try
            {
                var result = await _mediator.Send(new CreateCategoryCommand(request));

                _logger.LogInformation("Categoría creada correctamente. CategoryId={CategoryId} Nombre={Nombre}", result.Id, result.Nombre);
                return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear categoría. Nombre={Nombre}", request?.Nombre);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el detalle de una categoría por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la categoría.</param>
        /// <returns>Categoría encontrada.</returns>
        [HttpGet("v1/api/categories/{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryResponse>> GetCategoryById([FromRoute] int id)
        {
            _logger.LogInformation("Solicitud recibida: obtener categoría por id. CategoryId={CategoryId}", id);

            try
            {
                var result = await _mediator.Send(new GetCategoryByIdQuery(id));

                if (result == null)
                {
                    _logger.LogWarning("Categoría no encontrada. CategoryId={CategoryId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Categoría retornada correctamente. CategoryId={CategoryId}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener categoría por id. CategoryId={CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Error al procesar la solicitud, inténtelo nuevamente más tarde." });
            }
        }

        /// <summary>
        /// Actualiza completamente una categoría existente.
        /// </summary>
        /// <param name="id">Identificador de la categoría.</param>
        /// <param name="request">Datos de la categoría a actualizar.</param>
        /// <returns>Categoría actualizada.</returns>
        [HttpPut("v1/api/categories/{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryResponse>> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryRequest request)
        {
            _logger.LogInformation("Solicitud recibida: actualizar categoría. CategoryId={CategoryId}", id);

            try
            {
                var result = await _mediator.Send(new UpdateCategoryCommand(request, id));

                if (result == null)
                {
                    _logger.LogWarning("Categoría no encontrada para actualización. CategoryId={CategoryId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Categoría actualizada correctamente. CategoryId={CategoryId}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar categoría. CategoryId={CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina (lógicamente) una categoría existente.
        /// </summary>
        /// <param name="id">Identificador de la categoría.</param>
        [HttpDelete("v1/api/categories/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            _logger.LogInformation("Solicitud recibida: eliminar (lógico) categoría. CategoryId={CategoryId}", id);

            try
            {
                var result = await _mediator.Send(new DeleteCategoryCommand(id));

                if (!result)
                {
                    _logger.LogWarning("Categoría no encontrada para eliminación lógica. CategoryId={CategoryId}", id);
                    return NotFound(new { Message = "Categoría no encontrada." });
                }

                _logger.LogInformation("Categoría eliminada lógicamente correctamente. CategoryId={CategoryId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar (lógico) categoría. CategoryId={CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
    }
}
