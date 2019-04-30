using System;
using System.Threading.Tasks;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using static Marketplace.UserProfile.Contracts;

namespace Marketplace.UserProfile
{
    public class UserProfileApplicationService : IApplicationService
    {
        private readonly IUserProfileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CheckTextForProfanity _checkText;

        public UserProfileApplicationService(
            IUserProfileRepository repository, IUnitOfWork unitOfWork,
            CheckTextForProfanity checkText
        )
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
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
                            DisplayName.FromString(cmd.DisplayName, _checkText)
                        )
                    ),
                V1.UpdateUserProfilePhoto cmd =>
                    HandleUpdate(
                        cmd.UserId,
                        profile => profile.UpdateProfilePhoto(
                            new Uri(cmd.PhotoUrl)
                        )
                    ),
                _ => Task.CompletedTask
            };

        private async Task HandleCreate(V1.RegisterUser cmd)
        {
            if (await _repository.Exists(new UserId(cmd.UserId)))
                throw new InvalidOperationException(
                    $"Entity with id {cmd.UserId} already exists"
                );

            var userProfile = new Domain.UserProfile.UserProfile(
                new UserId(cmd.UserId),
                FullName.FromString(cmd.FullName),
                DisplayName.FromString(cmd.DisplayName, _checkText)
            );

            await _repository.Add(userProfile);
            await _unitOfWork.Commit();
        }

        private async Task HandleUpdate(
            Guid userProfileId,
            Action<Domain.UserProfile.UserProfile> operation
        )
        {
            var userProfile = await _repository
                .Load(new UserId(userProfileId));
            if (userProfile == null)
                throw new InvalidOperationException(
                    $"Entity with id {userProfileId} cannot be found"
                );

            operation(userProfile);

            await _unitOfWork.Commit();
        }
    }
}