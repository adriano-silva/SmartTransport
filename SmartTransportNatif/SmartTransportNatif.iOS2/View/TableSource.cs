using Foundation;
using System;
using System.Diagnostics;
using UIKit;

namespace SmartTransportNatif.iOS2
{
    public partial class TableSource : UITableViewSource
    {

        protected string[] tableItems;
        protected string cellIdentifier = "SectionCell";
        HomeTableController home;

        public TableSource (IntPtr handle) : base (handle)
        {
        }

        
        [Export("initWithCoder:")]
        public TableSource (NSCoder coder)
        {

        }

        public TableSource (string[] items, HomeTableController home)
        {
            this.home = home;
            this.tableItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Length;
        }

        /// <summary>
        /// Called when a row is touched
        /// </summary>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //UIAlertController okAlertController = UIAlertController.Create("Row Selected", tableItems[indexPath.Row], UIAlertControllerStyle.Alert);
            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //home.PresentViewController(okAlertController, true, null);

            try
            {
                UIStoryboard board = UIStoryboard.FromName("Main", null);
                JourneyTableController journeyCtrl = board.InstantiateViewController("journey") as JourneyTableController;
                if (journeyCtrl != null)
                {
                    journeyCtrl.toFavStation = tableItems[indexPath.Row];
                    home.NavigationController.PushViewController(journeyCtrl, true);
                }
            }catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            tableView.DeselectRow(indexPath, true);
        }

        /// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);

            // if there are no cells to reuse, create a new one
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            cell.TextLabel.Text = tableItems[indexPath.Row];
            cell.TextLabel.TextAlignment = UITextAlignment.Center;
 

            return cell;
        }

    }
}