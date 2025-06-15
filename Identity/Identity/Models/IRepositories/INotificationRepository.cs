namespace Identity.Models.IRepositories
{
    public interface INotificationRepository
    {
        void Add(Notification notification);
        List<Notification> GetAll(string userId);
        void Delete(int id);
    }
}
