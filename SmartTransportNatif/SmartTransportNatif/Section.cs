using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTransportNatif
{
    public class Section
    {
        public Journey journey { get; set; }
        public Walk walk { get; set; }
        public Stop departure { get; set; }
        public Stop arrival { get; set; }
    }

    public class Walk
    {
        public string duration { get; set; }
    }

}
