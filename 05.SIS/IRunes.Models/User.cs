namespace IRunes.Models
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
