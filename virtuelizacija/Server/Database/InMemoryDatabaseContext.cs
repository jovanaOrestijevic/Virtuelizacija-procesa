using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database
{
    public class InMemoryDatabaseContext
    {
        private static Dictionary<DateTime, List<Load>> loadsDatabase = new Dictionary<DateTime, List<Load>>();
        private static Dictionary<DateTime, DateTime> expireTimes = new Dictionary<DateTime, DateTime>();
        private static Dictionary<long, Audit> auditDatabase = new Dictionary<long, Audit>();
        private static object lockObject = new object();
        int dataTimeout = 15;
        public InMemoryDatabaseContext()
        {
            if (ConfigurationManager.AppSettings["DateTimeout"] != null)
                if (int.TryParse(ConfigurationManager.AppSettings["DateTimeout"], out int timeout))
                    if (timeout > 0)
                        dataTimeout = timeout;
        }

        public List<Load> GetLoads(DateTime date)
        {
            lock (lockObject)
            {
                if (loadsDatabase.TryGetValue(date.Date, out List<Load> loads))
                {
                    return loads;
                }
                return null;
            }
        }
        public void AddLoads(DateTime date, List<Load> loads)
        {
            lock (lockObject)
            {
                loadsDatabase.Add(date.Date, loads);
                expireTimes.Add(DateTime.Now.AddMinutes(dataTimeout), date.Date);
            }
        }
        public void DeleteOldLoads()
        {
            lock (lockObject)
            {
                DateTime now = DateTime.Now;
                foreach (KeyValuePair<DateTime, DateTime> item in expireTimes)
                {
                    if (item.Key <= now)
                    {
                        loadsDatabase.Remove(item.Value);
                        expireTimes.Remove(item.Key);
                    }
                }
                expireTimes = expireTimes.Where(item => item.Key > now).ToDictionary(x => x.Key, x => x.Value);

            }
        }

        public void AddAudits(List<Audit> audits)
        {
            lock (lockObject)
            {
                foreach (var audit in audits)
                {
                    long id = 1;
                    if (auditDatabase.Count != 0)
                        id = auditDatabase.Values.Max(x => x.Id) + 1;

                    audit.Id = id;
                    auditDatabase.Add(id, audit);

                }
            }
        }
    }
}
