using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QR_Scanner.Server.Models;
using QR_Scanner.Server.Services;
using System.Buffers.Text;

namespace QR_Scanner.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;
        private readonly IQRCodeService _qrCodeService;
        private readonly ILogger<QRController> _logger;
        private const string CollectionName = "establishments";
        private readonly string baseUrl;
        

        public QRController(IFirebaseService firebaseService, IQRCodeService qrCodeService, ILogger<QRController> logger, IConfiguration configuration)
        {
            _firebaseService = firebaseService;
            _qrCodeService = qrCodeService;
            _logger = logger;
            baseUrl = configuration["BaseUiUrl"] ?? string.Empty;
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


        [HttpPost("generate/{establishmentId}")]
        public async Task<IActionResult> GenerateQRCodeImage(string establishmentId)
        {
            try
            {
                var results = new List<object>();

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
                    var qrCodeBytes = _qrCodeService.GenerateQRCode(redirectUrl, 20);
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
                _logger.LogError(ex, "Error in QR code generation");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}