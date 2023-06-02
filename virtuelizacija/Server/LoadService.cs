using Common;
using Server.Database;
using Server.FileInUseCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class LoadService : ILoadService
    {
        DatabaseHandler databaseHandler = new DatabaseHandler(new InMemoryDatabaseContext(), new XMLDatabaseContext(new FileInUseCommonChecker()));
        public ServiceResult GetLoads(DateTime date)
        {
            return databaseHandler.GetLoads(date);
        }
    }
}
