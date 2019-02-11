using CoreGraphics;
using CoreLocation;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace SmartTransportNatif.iOS2
{
    public partial class JourneyTableController : UIViewController
    {

        UITableView table;
        Worker pcl;
        LoadingOverlay loadPop;

        public string toFavStation { get; set; }

        public JourneyTableController (IntPtr handle) : base (handle)
        {
        }


        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            CLLocationManager manager = new CLLocationManager();
            //manager.RequestAlwaysAuthorization();
            manager.RequestWhenInUseAuthorization();
            pcl = new Worker();



            table = new UITableView(new CGRect(0, 270, View.Bounds.Width, View.Bounds.Width - 20)); // defaults to Plain style
            table.AutoresizingMask = UIViewAutoresizing.All;



            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds);
            View.Add(loadPop);

            //Sends GET Request to OpenData Transport API to get the closest station according to the current position
            Station station = await pcl.GetClosestStation();
            stationBtn.SetTitle(station.name + "\n" + station.distance, UIControlState.Normal);

            string dest = pcl.getFavStations()[toFavStation];

            //Sends GET Request to OpenData Transport API to get the journey between the closest and selected stations
            List<Connection> connections = await pcl.GetJourney(station.name,dest );

            loadPop.Hide();

            //Time parsing and displaying
            TimeSpan miniDuration = new TimeSpan(100,24,60,60);
            int indexBestConnection = 0;
            for (int i =0; i<4; i++)
            {
                TimeSpan duration = new TimeSpan(100, 24,60, 60);
                string durationStr = connections[i].duration;
                if (durationStr != null)
                {
                    String[] substring = durationStr.Split('d');
                    int days =  Int32.Parse(substring[0]);
                    String[] hourMinSec = substring[1].Split(':');
                    duration = new TimeSpan(days, Int32.Parse(hourMinSec[0]), Int32.Parse(hourMinSec[1]), Int32.Parse(hourMinSec[2]));
                }
               
                if(duration < miniDuration)
                {
                    miniDuration = duration;
                    indexBestConnection = i;
                }
            }

            table.Source = new TableJourney(connections[indexBestConnection].sections, this);
            table.ScrollEnabled = true;
            table.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            table.EstimatedRowHeight = 40f;
            Add(table);
        }
    }
}