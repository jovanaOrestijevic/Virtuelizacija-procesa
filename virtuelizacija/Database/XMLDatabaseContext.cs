using Common;
using Database.FileInUseCheck;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Database
{
    public class XMLDatabaseContext
    {
        private readonly IFileInUseChecker fileInUseChecker;
        private string loadsPath;
        private string auditPath;
        public XMLDatabaseContext(IFileInUseChecker fileInUseChecker, string loadPath, string auditPath)
        {
            this.fileInUseChecker = fileInUseChecker;
            this.loadsPath = loadPath;
            this.auditPath = auditPath;
            if (!Directory.Exists(Path.GetDirectoryName(loadsPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(loadsPath));
            if (!Directory.Exists(Path.GetDirectoryName(auditPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(auditPath));
            if (!File.Exists(loadsPath))
                File.Create(loadsPath);
            if (!File.Exists(auditPath))
                File.Create(auditPath);
        }
        public ServiceResult GetLoads(DateTime date)
        {
            ServiceResult result = new ServiceResult();
            XDocument xml = null;
            try
            {
                xml = GetXDocument(loadsPath);

            }
            catch (Exception e)
            {
                result.Audits.Add(new Audit { Message = e.Message, MessageType = MessageType.Error });
                xml = null;
            }
            if (xml == null)
            {
                result.Audits.Add(new Audit { Message = "Nevalidan XML", MessageType = MessageType.Error });
            }
            else
            {

                foreach (var row in xml.Descendants("row"))
                {
                    List<Audit> audits = new List<Audit>();
                    long id = 0;
                    DateTime timestamp = DateTime.Now;
                    double forecastValue = 0, measuredValue = 0;

                    if (string.IsNullOrEmpty(row.Element("ID").Value))
                    {
                        audits.Add(new Audit { Message = "ID ne postoji u XML bazi", MessageType = MessageType.Error });
                    }
                    else if (!long.TryParse(row.Element("ID").Value, out id))
                    {
                        audits.Add(new Audit { Message = "ID nije ceo broj u XML bazi", MessageType = MessageType.Error });
                    }
                    if (string.IsNullOrEmpty(row.Element("TIME_STAMP").Value))
                    {
                        audits.Add(new Audit { Message = "TIME_STAMP ne postoji u XML bazi", MessageType = MessageType.Error });
                    }
                    else if (!DateTime.TryParse(row.Element("TIME_STAMP").Value, out timestamp))
                    {
                        audits.Add(new Audit { Message = "TIME_STAMP nije datum u XML bazi", MessageType = MessageType.Error });
                    }
                    if (string.IsNullOrEmpty(row.Element("FORECAST_VALUE").Value))
                    {
                        audits.Add(new Audit { Message = "FORECAST_VALUE ne postoji u XML bazi", MessageType = MessageType.Error });
                    }
                    else if (!double.TryParse(row.Element("FORECAST_VALUE").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out forecastValue))
                    {
                        audits.Add(new Audit { Message = "FORECAST_VALUE nije realan broj u XML bazi", MessageType = MessageType.Error });
                    }
                    if (string.IsNullOrEmpty(row.Element("MEASURED_VALUE").Value))
                    {
                        audits.Add(new Audit { Message = "MEASURED_VALUE ne postoji u XML bazi", MessageType = MessageType.Error });
                    }
                    else if (!double.TryParse(row.Element("MEASURED_VALUE").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out measuredValue))
                    {
                        audits.Add(new Audit { Message = "MEASURED_VALUE nije realan broj u XML bazi", MessageType = MessageType.Error });
                    }
                    if (audits.Count == 0)
                        result.Loads.Add(new Load(id, timestamp, forecastValue, measuredValue));
                    result.Audits.AddRange(audits);
                }
                result.Loads = result.Loads.Where(x => x.Timestamp.Date == date).ToList();
            }
            return result;
        }
        private XDocument GetXDocument(string path)
        {
            if (fileInUseChecker.IsFileInUse(path))
            {
                Console.WriteLine($"Cannot process the file {Path.GetFileName(path)}. It's being in use by another process or it has been deleted.");
                return null;
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    XDocument xdoc = XDocument.Load(fileStream);
                    return xdoc;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void AddAudits(List<Audit> audits)
        {
            XDocument xDocument = GetXDocument(auditPath);
            if (xDocument == null)
            {
                xDocument = new XDocument(new XElement("STAVKE"));
            }
            var xmlAudits = xDocument.Descendants("row").ToList();
            foreach (Audit audit in audits)
            {
                XElement row = new XElement("row");
                row.Add(new XElement("Timestamp", audit.Timestamp.ToString()));
                row.Add(new XElement("MessageType", audit.MessageType.ToString()));
                row.Add(new XElement("Message", audit.Message.ToString()));
                xmlAudits.Add(row);
            }
            xDocument.Root.Add(xmlAudits);
            try
            {

                xDocument.Save(auditPath);
            }
            catch (Exception)
            { }

        }
    }
}