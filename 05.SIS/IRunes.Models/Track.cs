namespace IRunes.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Track
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string AlbumId { get; set; }

        [Required]
        [ForeignKey("AlbumId")]
        public virtual Album Album { get; set; }
    }
}
