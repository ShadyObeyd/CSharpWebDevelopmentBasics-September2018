using System.Collections.Generic;

namespace MishMashWebApp.ViewModels.Channels
{
    public class DisplayChannelViewModel
    {
        public IEnumerable<ChannelViewModel> FollowedChannels { get; set; }
    }
}
