using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTransportNatif
{
    public class Journey
    {
        public string name { get; set; }
        public string category { get; set; }
        public string subcategory { get; set; }
        public int? categoryCode { get; set; }
        public string number { get; set; }
        public string @operator { get; set; }
        public string to { get; set; }
        public List<Stop> passList { get; set; }
        public int? capacity1st { get; set; }
        public int? capacity2nd { get; set; }
    }
}
