using Identity.Data;
using Identity.Models.IRepositories;

namespace Identity.Models.Repositories
{
    public class ProfilePictureRepository :IProfilePictureRepository
    {
        private readonly ApplicationDbContext context;

        public ProfilePictureRepository(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ProfilePicture? GetByUserId(string userId)
        {
            return context.ProfilePictures.FirstOrDefault(p => p.UserId == userId);
        }

        public void Add(ProfilePicture picture)
        {
            context.ProfilePictures.Add(picture);
            context.SaveChanges();
        }

        public void Update(ProfilePicture picture)
        {
            context.ProfilePictures.Update(picture);
            context.SaveChanges();
        }

        
    }
}
