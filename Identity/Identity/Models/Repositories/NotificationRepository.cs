using Identity.Data;
using Identity.Models.IRepositories;

namespace Identity.Models.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext context;

        public NotificationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(Notification notification)
        {
            context.Notifications.Add(notification);
            context.SaveChanges();
        }

        public List<Notification> GetAll(string userId)
        {
            return context.Notifications.Where(n => n.UserId == userId).ToList();
        }

        public void Delete(int id)
        {
            Notification? notification = context.Notifications.Find(id);
            if (notification != null)
            {
                context.Notifications.Remove(notification);
                context.SaveChanges();
            }
        }
    }

}
