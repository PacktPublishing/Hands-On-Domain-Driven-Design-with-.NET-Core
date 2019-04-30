using System;
using Marketplace.EventSourcing;
using Marketplace.Users.Domain.Shared;
using Marketplace.Users.Domain.UserProfiles;
using static Marketplace.Users.Messages.UserProfile.Commands;

namespace Marketplace.Users.UserProfiles
{
    public class UserProfileCommandService
        : ApplicationService<UserProfile>
    {
        public UserProfileCommandService(
            IAggregateStore store,
            CheckTextForProfanity checkText
        ) : base(store)
        {
            CreateWhen<V1.RegisterUser>(
                cmd => new UserId(cmd.UserId),
                (cmd, id) => UserProfile.Create(
                    new UserId(id), FullName.FromString(cmd.FullName),
                    DisplayName.FromString(cmd.DisplayName, checkText)
                )
            );

            UpdateWhen<V1.UpdateUserFullName>(
                cmd => new UserId(cmd.UserId),
                (user, cmd)
                    => user.UpdateFullName(FullName.FromString(cmd.FullName))
            );

            UpdateWhen<V1.UpdateUserDisplayName>(
                cmd => new UserId(cmd.UserId),
                (user, cmd) => user.UpdateDisplayName(
                    DisplayName.FromString(cmd.DisplayName, checkText)
                )
            );

            UpdateWhen<V1.UpdateUserProfilePhoto>(
                cmd => new UserId(cmd.UserId),
                (user, cmd) => user.UpdateProfilePhoto(new Uri(cmd.PhotoUrl))
            );
        }
    }
}