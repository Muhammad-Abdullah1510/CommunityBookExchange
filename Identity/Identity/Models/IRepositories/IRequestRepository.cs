namespace Identity.Models.IRepositories
{
    public interface IRequestRepository
    {
        public string add(Request request);
        public List<Request> Get(string userid);

        Request GetById(int requestId); 
        void Delete(int requestId);
    }
}
