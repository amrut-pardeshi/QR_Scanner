using Microsoft.AspNetCore.Mvc;
using QR_Scanner.Server.Models;
using QR_Scanner.Server.Services;

namespace QR_Scanner.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;
        private readonly IQRCodeService _qrCodeService;
        private readonly ILogger<QRController> _logger;
        private readonly IConfiguration _configuration;
        private const string CollectionName = "establishments";

        public QRController(IFirebaseService firebaseService, IQRCodeService qrCodeService, ILogger<QRController> logger, IConfiguration configuration)
        {
            _firebaseService = firebaseService;
            _qrCodeService = qrCodeService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Generate QR code image for an establishment
        /// </summary>
        [HttpGet("generate/{establishmentId}")]
        public async Task<IActionResult> GenerateQRCode(string establishmentId, [FromQuery] int size = 20)
        {
            try
            {
                // Get the establishment to verify it exists
                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                // Generate redirect URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var redirectUrl = _qrCodeService.GenerateRedirectUrl(establishmentId, baseUrl);

                // Generate QR code image
                var qrCodeBytes = _qrCodeService.GenerateQRCode(redirectUrl, size);

                _logger.LogInformation("Generated QR code for establishment {EstablishmentName} ({EstablishmentId})", 
                    establishment.Name, establishmentId);

                return File(qrCodeBytes, "image/png", $"qr-{establishmentId}.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code for establishment {EstablishmentId}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get QR code data URL for an establishment (base64 encoded)
        /// </summary>
        [HttpGet("generate/{establishmentId}/data")]
        public async Task<IActionResult> GenerateQRCodeData(string establishmentId, [FromQuery] int size = 20)
        {
            try
            {
                // Get the establishment to verify it exists
                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                // Generate redirect URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var redirectUrl = _qrCodeService.GenerateRedirectUrl(establishmentId, baseUrl);

                // Generate QR code image
                var qrCodeBytes = _qrCodeService.GenerateQRCode(redirectUrl, size);
                var base64String = Convert.ToBase64String(qrCodeBytes);
                var dataUrl = $"data:image/png;base64,{base64String}";

                return Ok(new
                {
                    EstablishmentId = establishmentId,
                    EstablishmentName = establishment.Name,
                    RedirectUrl = redirectUrl,
                    QRCodeDataUrl = dataUrl,
                    QRCodeSize = size
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code data for establishment {EstablishmentId}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Handle QR code redirect - when someone scans the QR code
        /// </summary>
        [HttpGet("redirect/{establishmentId}")]
        public async Task<IActionResult> RedirectToEstablishment(string establishmentId)
        {
            var baseUIUrl = _configuration["BaseUiUrl"];
            try
            {
                // Get the establishment
                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    return NotFound("Establishment not found");
                }

                // Check if establishment is active
                if (!establishment.IsActive)
                {
                    return BadRequest("This establishment is currently inactive");
                }

                // Check if establishment has a website
                if (!string.IsNullOrEmpty(_configuration[""]))
                {

                     var websiteUrl = "https://" + baseUIUrl + establishmentId;
                    
                    
                    _logger.LogInformation("Redirecting to establishment website: {Website} for {EstablishmentName}", 
                        websiteUrl, establishment.Name);
                    
                    return Redirect(websiteUrl);
                }
                else
                {
                    // If no website, return establishment information
                    _logger.LogInformation("No website found for establishment {EstablishmentName}, returning establishment info", 
                        establishment.Name);
                    
                    return Ok(new
                    {
                        Message = $"Welcome to {establishment.Name}!",
                        Establishment = new
                        {
                            establishment.Name,
                            establishment.Address,
                            establishment.City,
                            establishment.Phone,
                            establishment.Email,
                            establishment.Type
                        },
                        Note = "This establishment doesn't have a website configured. Please contact them directly using the information above."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redirecting for establishment {EstablishmentId}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get QR code information for an establishment
        /// </summary>
        [HttpGet("info/{establishmentId}")]
        public async Task<IActionResult> GetQRInfo(string establishmentId)
        {
            try
            {
                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    return NotFound($"Establishment with ID {establishmentId} not found");
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var redirectUrl = _qrCodeService.GenerateRedirectUrl(establishmentId, baseUrl);
                var qrGenerateUrl = $"{baseUrl}/api/qr/generate/{establishmentId}";
                var qrDataUrl = $"{baseUrl}/api/qr/generate/{establishmentId}/data";

                return Ok(new
                {
                    EstablishmentId = establishmentId,
                    EstablishmentName = establishment.Name,
                    EstablishmentType = establishment.Type.ToString(),
                    RedirectUrl = redirectUrl,
                    QRGenerateUrl = qrGenerateUrl,
                    QRDataUrl = qrDataUrl,
                    IsActive = establishment.IsActive,
                    Instructions = new
                    {
                        ImageDownload = "Use /api/qr/generate/{id} to download QR code as PNG image",
                        DataUrl = "Use /api/qr/generate/{id}/data to get base64 encoded QR code",
                        CustomSize = "Add ?size=X parameter to control QR code size (default: 20)"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QR info for establishment {EstablishmentId}", establishmentId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateBulkQRCodes([FromBody] string establishmentId, [FromQuery] int size = 20)
        {
            try
            {
                var results = new List<object>();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                if (establishment == null)
                {
                    results.Add(new
                    {
                        EstablishmentId = establishmentId,
                        Success = false,
                        Error = "Establishment not found"
                    });

                    return Ok(new
                    {
                        Successful = 0,
                        Results = results
                    });
                }

                try
                {
                    var redirectUrl = _qrCodeService.GenerateRedirectUrl(establishmentId, baseUrl);
                    var qrCodeBytes = _qrCodeService.GenerateQRCode(redirectUrl, size);
                    var base64String = Convert.ToBase64String(qrCodeBytes);

                    results.Add(new
                    {
                        EstablishmentId = establishmentId,
                        EstablishmentName = establishment.Name,
                        Success = true,
                        RedirectUrl = redirectUrl,
                        QRCodeDataUrl = $"data:image/png;base64,{base64String}"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating QR for establishment {EstablishmentId}", establishmentId);
                    results.Add(new
                    {
                        EstablishmentId = establishmentId,
                        Success = false,
                        Error = "Failed to generate QR code"
                    });
                }

                return Ok(new
                {
                    Successful = results.Count(r => (bool)r.GetType().GetProperty("Success")!.GetValue(r)!),
                    Results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk QR code generation");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Generate QR codes for multiple establishments
        /// </summary>
        [HttpPost("generate/bulk")]
        public async Task<IActionResult> GenerateBulkQRCodes([FromBody] string[] establishmentIds, [FromQuery] int size = 20)
        {
            try
            {
                var results = new List<object>();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                foreach (var establishmentId in establishmentIds)
                {
                    var establishment = await _firebaseService.GetByIdAsync<Establishment>(CollectionName, establishmentId);
                    if (establishment == null)
                    {
                        results.Add(new
                        {
                            EstablishmentId = establishmentId,
                            Success = false,
                            Error = "Establishment not found"
                        });
                        continue;
                    }

                    try
                    {
                        var redirectUrl = _qrCodeService.GenerateRedirectUrl(establishmentId, baseUrl);
                        var qrCodeBytes = _qrCodeService.GenerateQRCode(redirectUrl, size);
                        var base64String = Convert.ToBase64String(qrCodeBytes);

                        results.Add(new
                        {
                            EstablishmentId = establishmentId,
                            EstablishmentName = establishment.Name,
                            Success = true,
                            RedirectUrl = redirectUrl,
                            QRCodeDataUrl = $"data:image/png;base64,{base64String}"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating QR for establishment {EstablishmentId}", establishmentId);
                        results.Add(new
                        {
                            EstablishmentId = establishmentId,
                            Success = false,
                            Error = "Failed to generate QR code"
                        });
                    }
                }

                return Ok(new
                {
                    TotalRequested = establishmentIds.Length,
                    Successful = results.Count(r => (bool)r.GetType().GetProperty("Success")!.GetValue(r)!),
                    Results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk QR code generation");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}