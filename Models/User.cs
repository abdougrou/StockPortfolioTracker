using System.ComponentModel.DataAnnotations;

namespace StockPortfolioTracker.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        public string UserPassword { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // FK
        public ICollection<Portfolio> Porfolios { get; set; }
    }
}
