using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

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
}