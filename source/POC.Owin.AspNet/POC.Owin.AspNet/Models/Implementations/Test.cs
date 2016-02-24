using POC.Owin.AspNet.Models.Abstractions;
using System.Diagnostics;

namespace POC.Owin.AspNet.Models.Implementations
{
    public class Test : ITest
    {
        public void TestIt()
        {
            Debug.WriteLine("test");
        }
    }
}
