using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace QR_Scanner.Server.Models
{
    [FirestoreData]
    public class Establishment
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        [Required]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        public string Address { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? City { get; set; }

        [FirestoreProperty]
        public string? Country { get; set; }

        [FirestoreProperty]
        public string? PostalCode { get; set; }

        [FirestoreProperty]
        public string? Phone { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        [Required]
        public EstablishmentType Type { get; set; }

        [FirestoreProperty]
        public Owner? Owner { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string QRCodeDataUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a new establishment - excludes Id and QRCodeDataUrl
    /// </summary>
    public class CreateEstablishmentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PostalCode { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        [Required]
        public EstablishmentType Type { get; set; }

        public Owner? Owner { get; set; }
    }

    /// <summary>
    /// DTO for updating an establishment - excludes Id and QRCodeDataUrl
    /// </summary>
    public class UpdateEstablishmentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PostalCode { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        [Required]
        public EstablishmentType Type { get; set; }

        public Owner? Owner { get; set; }
    }

    [FirestoreData]
    public class Owner
    {
        [FirestoreProperty]
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        [Phone]
        [StringLength(20, MinimumLength = 10)]
        public string Mobile { get; set; } = string.Empty;
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

    /// <summary>
    /// Extension methods for mapping between DTOs and Establishment entity
    /// </summary>
    public static class EstablishmentExtensions
    {
        /// <summary>
        /// Maps CreateEstablishmentDto to Establishment entity
        /// </summary>
        public static Establishment ToEstablishment(this CreateEstablishmentDto dto)
        {
            return new Establishment
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                Email = dto.Email,
                Type = dto.Type,
                Owner = dto.Owner,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                QRCodeDataUrl = string.Empty
            };
        }

        /// <summary>
        /// Maps UpdateEstablishmentDto to existing Establishment entity, preserving Id and QRCodeDataUrl
        /// </summary>
        public static Establishment ToEstablishment(this UpdateEstablishmentDto dto, Establishment existing)
        {
            return new Establishment
            {
                Id = existing.Id,
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                Email = dto.Email,
                Type = dto.Type,
                Owner = dto.Owner,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                QRCodeDataUrl = existing.QRCodeDataUrl
            };
        }
    }
}