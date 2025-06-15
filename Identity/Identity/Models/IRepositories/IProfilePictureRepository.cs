namespace Identity.Models.IRepositories
{
    public interface IProfilePictureRepository
    {
        ProfilePicture GetByUserId(string userId);
        void Add(ProfilePicture picture);
        void Update(ProfilePicture picture);
    }
}
