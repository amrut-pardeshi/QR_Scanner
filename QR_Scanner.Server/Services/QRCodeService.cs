using QRCoder;

namespace QR_Scanner.Server.Services
{
    public interface IQRCodeService
    {
        byte[] GenerateQRCode(string content, int pixelsPerModule = 20);
        string GenerateRedirectUrl(string establishmentId, string baseUrl);
    }

    public class QRCodeService : IQRCodeService
    {
        private readonly ILogger<QRCodeService> _logger;
        private readonly IConfiguration _configuration;

        public QRCodeService(ILogger<QRCodeService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public byte[] GenerateQRCode(string content, int pixelsPerModule = 20)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);
                
                var qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);
                
                _logger.LogInformation($"Generated QR code for content: {content}");
                return qrCodeBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating QR code for content: {content}");
                throw;
            }
        }

        public string GenerateRedirectUrl(string establishmentId, string baseUrl)
        {
            var redirectUrl = $"{baseUrl}/{establishmentId}";
            _logger.LogInformation($"Generated redirect URL: {redirectUrl} for establishment: {establishmentId}");
            return redirectUrl;
        }
    }
}