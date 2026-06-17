using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoTrackerAPI.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CryptoCode { get; set; } = string.Empty; 

        [Required]
        public string Action { get; set; } = string.Empty; // "purchase" o "sale"

        [Required]
        [Column(TypeName = "decimal(18, 8)")]
        public decimal CryptoAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Money { get; set; }

        [Required]
        public DateTime Datetime { get; set; }
    }
}