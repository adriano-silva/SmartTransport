using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTransportNatif
{
    public class Stop
    {
        public Station station { get; set; }
        public DateTime? arrival { get; set; }
        public int? arrivalTimestamp { get; set; }
        public DateTime? departure { get; set; }
        public int? departureTimestamp { get; set; }
        public int? delay { get; set; }
        public string platform { get; set; }
        public Prognosis prognosis { get; set; }
        public string realtimeAvailability {get; set;}
        public Station location { get; set; }
    }
}
