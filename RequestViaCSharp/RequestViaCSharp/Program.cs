using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestViaCSharp
{
    class Program
    {
        static void Main(string[] args)
        {

            BaseApiService servide = new BaseApiService();

            string body= "{\"id\":2,\"name\":\"zzzz\",\"age\":3}";
            string bodyput = "{id:2,name:'yyy',age:3}";
            Console.WriteLine("Post:" + servide.Post("http://localhost:8080/users/add",body));
            Console.WriteLine("Get"+servide.Get("http://localhost:8080/users/"));
            Console.WriteLine("Get" + servide.Get("http://localhost:8080/users/2"));
            //Console.WriteLine("Put" + servide.Put("http://localhost:8080/users/0",bodyput));
            Console.WriteLine("Get" + servide.Get("http://localhost:8080/users/0"));
            Console.ReadKey();
        }
    }
}
