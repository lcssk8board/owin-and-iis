using System.Web.Http;

namespace POC.Owin.AspNet.Controllers
{
    public class TestController : ApiController
    {
        public int Get()
        {
            return 1;
        }
    }
}
