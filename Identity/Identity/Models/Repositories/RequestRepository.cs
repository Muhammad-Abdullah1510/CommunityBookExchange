using Identity.Models.IRepositories;
using Identity.Data;

namespace Identity.Models.Repositories
{
    public class RequestRepository:IRequestRepository
    {
        private ApplicationDbContext context;

        public RequestRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public string add(Request request)
        {
            try
            {
                context.Requests.Add(request);
                context.SaveChanges();
                return "Request Sent";
            }
            catch (Exception ex) 
            {
                return "Error Occured!";
            }
        }
        public List<Request> Get(string userid)
        {
            try
            {
                return context.Requests.Where(r => r.requestedTo == userid).ToList();
            }
            catch (Exception ex)
            {
                return new List<Request> { new Request
                {
                    requestedTo = "error",
                    requestedBy= "error",
                    BookId = -1,
                    BookName = "error",
                } };



                //return new List<Book>();
            }
        }


        public Request GetById(int requestId)
        {
            return context.Requests.First(r => r.Id == requestId);
        }

        public void Delete(int requestId)
        {
            var request = context.Requests.First(r => r.Id == requestId);
            if (request != null)
            {
                context.Requests.Remove(request);
                context.SaveChanges();
            }
        }
    }
}
