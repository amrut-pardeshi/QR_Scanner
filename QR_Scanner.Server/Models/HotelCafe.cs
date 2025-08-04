using System.ComponentModel.DataAnnotations;

namespace QR_Scanner.Server.Models
{
    public class Establishment
    {
        public string? Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        public string? City { get; set; }
        
        public string? Country { get; set; }
        
        public string? PostalCode { get; set; }
        
        public string? Phone { get; set; }
        
        public string? Email { get; set; }
        
        public string? Website { get; set; }
        
        [Required]
        public EstablishmentType Type { get; set; }
        
        // Hotel specific properties
        public int? StarRating { get; set; }
        
        public decimal? PricePerNight { get; set; }
        
        public List<string> Amenities { get; set; } = new();
        
        // Cafe specific properties
        public string? OpeningHours { get; set; }
        
        public bool? HasWifi { get; set; }
        
        public bool? HasParking { get; set; }
        
        public List<string> Specialties { get; set; } = new();
        
        public decimal? AveragePrice { get; set; }
        
        // Common properties
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Dictionary<string, object>? AdditionalProperties { get; set; }
    }
    
    public enum EstablishmentType
    {
        Hotel,
        Cafe,
        Restaurant,
        FastFood,
        Bar,
        Bakery,
        IceCream,
        TeaHouse
    }
}