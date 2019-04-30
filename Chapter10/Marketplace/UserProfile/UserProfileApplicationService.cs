using System;
using System.Threading.Tasks;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using static Marketplace.UserProfile.Contracts;

namespace Marketplace.UserProfile
{
    public class UserProfileApplicationService
        : IApplicationService
    {
        private readonly IAggregateStore _store;
        private readonly CheckTextForProfanity _checkText;

        public UserProfileApplicationService(
            IAggregateStore store,
            CheckTextForProfanity checkText
        )
        {
            _store = store;
            _checkText = checkText;
        }

        public Task Handle(object command) =>
            command switch
            {
                V1.RegisterUser cmd =>
                    HandleCreate(cmd),
                V1.UpdateUserFullName cmd =>
                    HandleUpdate(
                        cmd.UserId,
                        profile => profile.UpdateFullName(
                            FullName.FromString(cmd.FullName)
                        )
                    ),
                V1.UpdateUserDisplayName cmd =>
                    HandleUpdate(
                        cmd.UserId,
                        profile => profile.UpdateDisplayName(
                            DisplayName.FromString(
                                cmd.DisplayName,
                                _checkText
                            )
                        )
                    ),
                V1.UpdateUserProfilePhoto cmd =>
                    HandleUpdate(
                        cmd.UserId,
                        profile => profile
                            .UpdateProfilePhoto(
                                new Uri(cmd.PhotoUrl)
                            )
                    ),
                _ => Task.CompletedTask
            };

        private async Task HandleCreate(V1.RegisterUser cmd)
        {
            if (await _store
                .Exists<Domain.UserProfile.UserProfile, UserId>(
                    new UserId(cmd.UserId)
                ))
                throw new InvalidOperationException(
                    $"Entity with id {cmd.UserId} already exists"
                );

            var userProfile = new Domain.UserProfile.UserProfile(
                new UserId(cmd.UserId),
                FullName.FromString(cmd.FullName),
                DisplayName.FromString(cmd.DisplayName, _checkText)
            );

            await _store
                .Save<Domain.UserProfile.UserProfile, UserId>(
                    userProfile
                );
        }

        private Task HandleUpdate(
            Guid id,
            Action<Domain.UserProfile.UserProfile> update
        ) =>
            this.HandleUpdate(
                _store,
                new UserId(id),
                update
            );
    }
}