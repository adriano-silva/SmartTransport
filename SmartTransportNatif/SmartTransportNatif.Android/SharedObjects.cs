using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SmartTransportNatif.Droid
{
    class SharedObjects
    {
        public static Station closestStation;

        public static List<Connection> connections;

        public static List<Route> routes;

        public static Worker pcl;

        public static String destStation;

        public static Connection bestConnection;

    }
}