namespace MishMashWebApp.Models
{
    using Enums;

    using System.Collections.Generic;

    public class User
    {
        public User()
        {
            this.FollowedChannels = new HashSet<UserChannel>();
        }

        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public Role Role { get; set; }

        public virtual ICollection<UserChannel> FollowedChannels { get; set; }
    }
}
