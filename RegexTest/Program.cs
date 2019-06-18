using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var regex = new Regex(".com/api/callback/bokecc/(getcurr){0,1}devicestatus");

            Console.WriteLine(regex.IsMatch("http://v4.api.platform.lks.com/api/callback/bokecc/getcurrdevicestatus?roomId=1B7DEE728D9994939C33DC5901307461"));

            Console.WriteLine(regex.IsMatch("http://v4.api.platform.lks.com/api/callback/bokecc/devicestatus PostData:roomId=18514479100793F59C33DC5901307461&userId=RfyOLkQOfNx7jhOM&operateType=1&device[mc]=1&device[camera]=1&device[chat]=true&device[draw]=false& 请求耗时：20.335ms"));

            Console.ReadKey();
        }
    }
}
