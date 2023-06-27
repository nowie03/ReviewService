using System.ComponentModel.DataAnnotations;

namespace ReviewService.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public int rating { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
