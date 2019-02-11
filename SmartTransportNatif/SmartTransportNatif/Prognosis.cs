using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTransportNatif
{
    public class Prognosis
    {
        public string platform { get; set; }
        public string arrival { get; set; }
        public string departure { get; set; }
        public int? capacity1st { get; set; }
        public int? capacity2nd { get; set; }
    }
}
