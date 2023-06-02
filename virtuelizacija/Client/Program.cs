
using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ILoadService> factory = new ChannelFactory<ILoadService>("LoadService");
            string folderPath = ConfigurationManager.AppSettings["folderPath"];
            if (string.IsNullOrEmpty(folderPath))
            {
                Console.WriteLine("Putanja za čuvanje fajlova nije dodata u konfiguraciju");
                return;
            }
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            CSV csv = new CSV(folderPath);
            while (true)
            {
                Console.WriteLine("Unesite datum: ");
                string input = Console.ReadLine();
                if (!DateTime.TryParse(input, out DateTime date))
                {
                    Console.WriteLine("Nevalidan datum");
                    continue;
                }
                ILoadService loadService = factory.CreateChannel();
                ServiceResult serviceResult = loadService.GetLoads(date);
                foreach (var item in serviceResult.Audits)
                {
                    Console.WriteLine($"[{item.MessageType}] {item.Message}");
                }
                if (serviceResult.Loads != null && serviceResult.Loads.Count != 0)
                {
                    string fileName = $"result_{date.ToString("yyyy_MM_dd")}.csv";
                    csv.Save(serviceResult.Loads, fileName);
                    Console.WriteLine("Rezultati su u file-u na putanji " + Path.Combine(folderPath, fileName));
                }
            }
        }
    }
}
