using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTransportNatif
{ 
    public class Station
    {
        public string id { get; set; }
        public string name { get; set; }
        public int? score { get; set; }
        public Coordinate coordinate { get; set; }
        public double? distance { get; set; }
    }

    public class Stations
    {
        public List<Station> from { get; set; }

        public List<Station> to { get; set; }
    }

    public class RootObject
    {
        public List<Station> stations { get; set; }
    }

    public class RootObject2
    {
        public List<Connection> connections { get; set; }
        //public Station from { get; set; }
        //public Station to { get; set; }
        //public Stations stations { get; set; }
    }

 
}
