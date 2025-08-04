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