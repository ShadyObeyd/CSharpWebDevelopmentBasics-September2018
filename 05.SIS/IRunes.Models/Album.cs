namespace IRunes.Models
{
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Album
    {
        private const decimal AlbumDiscount = 1.13m;

        public Album()
        {
            this.Tracks = new List<Track>();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string CoverUrl { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
