using System;
using Marketplace.EventSourcing;

namespace Marketplace.Users.Domain.UserProfiles
{
    public class UserId : AggregateId<UserProfile>
    {
        public UserId(Guid value) : base(value) { }
    }
}