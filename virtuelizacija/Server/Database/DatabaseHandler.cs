using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database
{
    public class DatabaseHandler
    {
        private readonly InMemoryDatabaseContext inMemoryDatabase;
        private readonly XMLDatabaseContext xMLDatabaseContext;

        public DatabaseHandler(InMemoryDatabaseContext inMemoryDatabase, XMLDatabaseContext xMLDatabaseContext)
        {
            this.inMemoryDatabase = inMemoryDatabase;
            this.xMLDatabaseContext = xMLDatabaseContext;
        }
        public ServiceResult GetLoads(DateTime date)
        {
            ServiceResult result = new ServiceResult();
            result.Loads = inMemoryDatabase.GetLoads(date);
            if (result.Loads == null)
            {
                result = xMLDatabaseContext.GetLoads(date);
                if (result.Loads != null && result.Loads.Count > 0)
                {
                    inMemoryDatabase.AddLoads(date, result.Loads);
                    result.Audits.Add(new Audit { Message = $"Load objekti za datum {date.ToShortDateString()} su preuzeti iz XML baze", MessageType = MessageType.Info });
                }
                else
                {
                    result.Audits.Add(new Audit { Message = $"Load objekti za datum {date.ToShortDateString()} ne postoje", MessageType = MessageType.Warning });
                }

            }
            else
            {
                result.Audits.Add(new Audit { Message = $"Load objekti za datum {date.ToShortDateString()} su preuzeti iz in-memory baze", MessageType = MessageType.Info });
            }
            inMemoryDatabase.AddAudits(result.Audits);
            xMLDatabaseContext.AddAudits(result.Audits);

            return result;
        }
    }
}
