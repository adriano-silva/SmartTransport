using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTransportNatif
{
    public class Connection
    {
        public Stop from { get; set; }
        public Stop to { get; set; }
        public string duration { get; set; }
        public int? transfers { get; set; }
        public Service service { get; set; }
        public List<string> products { get; set; }
        public int? capacity1st { get; set; }
        public int? capacity2nd { get; set; }
        public List<Section> sections { get; set; }
    }
}
