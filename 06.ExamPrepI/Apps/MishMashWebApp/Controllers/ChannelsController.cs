using MishMashWebApp.Models;
using MishMashWebApp.Models.Enums;
using MishMashWebApp.ViewModels.Channels;
using SIS.HTTP.Responses;
using SIS.MvcFramework;
using System;
using System.Linq;

namespace MishMashWebApp.Controllers
{
    public class ChannelsController : BaseController
    {
        public IHttpResponse Followed()
        {
            var followedChannels = this.Db.Channels
                .Where(c => c.Followers.Any(u => u.User.Username == this.User.Username))
                .Select(c => new ChannelViewModel
                {
                    Id = c.Id,
                    Type = c.Type,
                    Name = c.Name,
                    FollowersCount = c.Followers.Count
                }).ToList();

            var viewModel = new DisplayChannelViewModel
            {
                FollowedChannels = followedChannels
            };

            return this.View(viewModel);
        }

        public IHttpResponse Create()
        {
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Create(ChannelInputModel model)
        {
            if (!Enum.TryParse(model.Type, true, out ChannelType type))
            {
                return this.BadRequestErrorWithView("Invalid channel type!");
            }

            var channel = new Channel
            {
                Name = model.Name,
                Description = model.Description,
                Type = type
            };

            if (!string.IsNullOrWhiteSpace(model.Tags))
            {
                string[] channelTags = model.Tags.Split(',', ';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var tagName in channelTags)
                {
                    var tag = this.Db.Tags.FirstOrDefault(t => t.Name == tagName);

                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Name = tagName
                        };

                        this.Db.Tags.Add(tag);

                        this.Db.SaveChanges();
                    }

                    var channelTag = new ChannelTag
                    {
                        TagId = tag.Id,
                        ChannelId = channel.Id
                    };
                }
            }

            this.Db.Channels.Add(channel);
            this.Db.SaveChanges();

            return this.Redirect($"/Channels/Details?id={channel.Id}");
        }

        public IHttpResponse Details(int id)
        {
            var channelViewMode = this.Db.Channels
                .Where(c => c.Id == id)
                .Select(c => new ChannelDetailViewModel
                {
                    Id = c.Id,
                    Description = c.Description,
                    Tags = c.Tags.Select(t => t.Tag.Name),
                    Type = c.Type.ToString(),
                    Name = c.Name,
                    FollowersCount = c.Followers.Count
                }).FirstOrDefault();

            if (channelViewMode == null)
            {
                return this.BadRequestErrorWithView("Requested channel does not exist!");
            }

            return this.View(channelViewMode);
        }

        public IHttpResponse Follow(int id)
        {
            var user = this.Db.Users.FirstOrDefault(u => u.Username == this.User.Username);

            if (!this.Db.UsersChannels.Any(uc => uc.UserId == user.Id && uc.ChannelId == id))
            {
                var userChannel = new UserChannel
                {
                    UserId = user.Id,
                    ChannelId = id
                };

                this.Db.UsersChannels.Add(userChannel);
                this.Db.SaveChanges();
            }

            return this.Redirect("/Channels/Followed");
        }

        public IHttpResponse Unfollow(int id)
        {
            var user = this.Db.Users.FirstOrDefault(u => u.Username == this.User.Username);

            var userChannel = this.Db.UsersChannels.FirstOrDefault(uc => uc.UserId == user.Id && uc.ChannelId == id);

            if (userChannel != null)
            {
                this.Db.UsersChannels.Remove(userChannel);
                this.Db.SaveChanges();
            }

            return this.Redirect("/Channels/Followed");
        }
    }
}
