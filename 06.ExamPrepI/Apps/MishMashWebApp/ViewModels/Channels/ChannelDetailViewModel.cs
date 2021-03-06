﻿using System.Collections.Generic;

namespace MishMashWebApp.ViewModels.Channels
{
    public class ChannelDetailViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int FollowersCount { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string TagsAsString => string.Join(", ", this.Tags);
    }
}
