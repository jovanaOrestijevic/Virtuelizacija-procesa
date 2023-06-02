using Server.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class BackgroundDelete
    {
        public delegate void DeleteDelegate();
        internal static void Start()
        {
            InMemoryDatabaseContext inMemoryDatabaseContext = new InMemoryDatabaseContext();
            DeleteDelegate Delete = new DeleteDelegate(inMemoryDatabaseContext.DeleteOldLoads);
            while (true)
            {
                Task.Delay(1000).Wait();
                Delete();

            }

        }
    }
}
