using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace QR_Scanner.Server.Models
{
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
}