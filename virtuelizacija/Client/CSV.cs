using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class CSV
    {
        private string folderPath;

        public CSV(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public void Save(List<Load> loads, string name)
        {
            string csv = "TIME_STAMP,,FORECAST_VALUE,MEASURED_VALUE";
            csv += Environment.NewLine;
            foreach (var load in loads)
            {
                csv += $"{load.Timestamp.ToString("yyyy-MM-dd")},{load.Timestamp.ToString("hh:mm")},{load.ForecastValue.ToString(CultureInfo.InvariantCulture)},{load.MeasuredValue.ToString(CultureInfo.InvariantCulture)}";
                csv += Environment.NewLine;
            }
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(folderPath, name)))
            {
                streamWriter.Write(csv);
            }
        }
    }
}
