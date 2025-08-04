using Microsoft.AspNetCore.Mvc;
using QR_Scanner.Server.Models;
using QR_Scanner.Server.Services;

namespace QR_Scanner.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstablishmentsController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;
        private readonly ILogger<EstablishmentsController> _logger;
        private const string CollectionName = "establishments";

        public EstablishmentsController(IFirebaseService firebaseService, ILogger<EstablishmentsController> logger)
        {
            _firebaseService = firebaseService;
            _logger = logger;
        }

        /// <summary>
        /// Get all establishments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Establishment>>> GetAll()
        {
            try
            {
                var establishments = await _firebaseService.GetAllAsync<Establishment>(CollectionName);
                return Ok(establishments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all establishments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get establishment by ID
        /// </summary>
        [HttpGet("{establishmentId}")]
        public async Task<ActionResult<Establishment>> GetById(string establishmentId)
        {
            try
            {
                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }
                return Ok(establishment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving establishment with ID {Id}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get active establishments only
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<Establishment>>> GetActive()
        {
            try
            {
                var establishments = await _firebaseService.GetByFieldAsync<Establishment>(CollectionName, "IsActive", true);
                return Ok(establishments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active establishments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new establishment
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Establishment>> Create([FromBody] CreateEstablishmentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Map DTO to Establishment entity using extension method
                var establishment = createDto.ToEstablishment();

                var id = await _firebaseService.CreateAsync(CollectionName, establishment);
                
                return CreatedAtAction(nameof(GetById), new { id }, establishment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating establishment");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing establishment
        /// </summary>
        [HttpPut("{establishmentId}")]
        public async Task<ActionResult<Establishment>> Update(string establishmentId, [FromBody] UpdateEstablishmentDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get existing establishment to preserve Id and QRCodeDataUrl
                var existingEstablishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (existingEstablishment == null)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                // Map DTO to Establishment entity using extension method
                var establishment = updateDto.ToEstablishment(existingEstablishment);

                var success = await _firebaseService.UpdateAsync(CollectionName, establishmentId, establishment);
                
                if (!success)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                return Ok(establishment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating establishment with ID {Id}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Search establishments by name
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<Establishment>>> Search([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Search name cannot be empty");
                }

                // Note: Firestore doesn't support case-insensitive or partial text search natively
                // For production, consider using Algolia or implement a full-text search solution
                var allEstablishments = await _firebaseService.GetAllAsync<Establishment>(CollectionName);
                var filteredEstablishments = allEstablishments
                    .Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return Ok(filteredEstablishments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching establishments with name {Name}", name);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Soft delete (deactivate) an establishment
        /// </summary>
        [HttpPatch("{establishmentId}/deactivate")]
        public async Task<ActionResult> Deactivate(string establishmentId)
        {
            try
            {
                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                establishment.IsActive = false;
                establishment.UpdatedAt = DateTime.UtcNow;
                
                var success = await _firebaseService.UpdateAsync(CollectionName, establishmentId, establishment);
                if (!success)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                return Ok(new { message = "Establishment deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating establishment with ID {Id}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Reactivate an establishment
        /// </summary>
        [HttpPatch("{establishmentId}/activate")]
        public async Task<ActionResult> Activate(string establishmentId)
        {
            try
            {
                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                establishment.IsActive = true;
                establishment.UpdatedAt = DateTime.UtcNow;
                
                var success = await _firebaseService.UpdateAsync(CollectionName, establishmentId, establishment);
                if (!success)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                return Ok(new { message = "Establishment activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating establishment with ID {Id}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete an establishment
        /// </summary>
        [HttpDelete("{establishmentId}")]
        public async Task<ActionResult> Delete(string establishmentId)
        {
            try
            {
                var success = await _firebaseService.DeleteAsync(CollectionName, establishmentId);

                if (!success)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting establishment with ID {Id}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}