using MishMashWebApp.ViewModels.Channels;
using System.Collections.Generic;

namespace MishMashWebApp.ViewModels.Home
{
    public class LoggedInViewModel
    {
        public ICollection<ChannelViewModel> YourChannels { get; set; }

        public ICollection<ChannelViewModel> SuggestedChannels { get; set; }

        public ICollection<ChannelViewModel> SeeOther { get; set; }
    }
}
