using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(LoadService)))
            {
                serviceHost.Open();
                Task.Factory.StartNew(BackgroundDelete.Start);
                Console.WriteLine("Pritisnite [Enter] za zaustavljanje servisa.");
                Console.ReadLine();
            }
        }
    }
}
