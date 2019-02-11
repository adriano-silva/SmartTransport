using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace SmartTransportNatif.iOS2
{
    public partial class HomeTableController : UIViewController
    {

        UITableView table;

        public HomeTableController (IntPtr handle) : base (handle)
        {
            
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Worker pcl = new Worker();

            table = new UITableView(new CGRect(0,250, View.Bounds.Width, View.Bounds.Width - 20)); // defaults to Plain style
            table.AutoresizingMask = UIViewAutoresizing.All;

            //get the favorite stations list from the PCL
            Dictionary<string,string> favStations = pcl.getFavStations();
            string[]tableItems = new string[favStations.Keys.Count];
            favStations.Keys.CopyTo(tableItems, 0);

            table.Source = new TableSource(tableItems, this);
            table.ScrollEnabled = false;
            table.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            Add(table);

        }



        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}