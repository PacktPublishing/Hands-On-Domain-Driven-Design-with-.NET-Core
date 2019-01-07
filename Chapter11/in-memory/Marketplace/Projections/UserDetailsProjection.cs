using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;

namespace Marketplace.Projections
{
    public class UserDetailsProjection : IProjection
    {
        List<ReadModels.UserDetails> _items;
        
        public UserDetailsProjection(List<ReadModels.UserDetails> items)
        {
            _items = items;
        }
        
        public Task Project(object @event)
        {
            switch (@event)
            {
                case Events.UserRegistered e:
                    _items.Add(new ReadModels.UserDetails
                    {
                        UserId = e.UserId,
                        DisplayName = e.DisplayName
                    });
                    break;
                case Events.UserDisplayNameUpdated e:
                    UpdateItem(e.UserId, x => x.DisplayName = e.DisplayName);
                    break;
                case Events.ProfilePhotoUploaded e:
                    UpdateItem(e.UserId, x => x.PhotoUrl = e.PhotoUrl);
                    break;
            }
            
            return Task.CompletedTask;
        }
        
        private void UpdateItem(Guid id, Action<ReadModels.UserDetails> update)
        {
            var item = _items.FirstOrDefault(x => x.UserId == id);
            if (item == null) return;

            update(item);
        }
    }
}