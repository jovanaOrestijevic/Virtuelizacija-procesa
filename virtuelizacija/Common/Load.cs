using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Load
    {
        public Load()
        {
        }

        public Load(long id, DateTime timestamp, double forecastValue, double measuredValue)
        {
            Id = id;
            Timestamp = timestamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
        }

        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double ForecastValue { get; set; }
        public double MeasuredValue { get; set; }
    }
}
