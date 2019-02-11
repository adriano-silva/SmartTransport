using Foundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace SmartTransportNatif.iOS2
{
    public partial class TableJourney : UITableViewSource
    {

        protected List<Section> tableItems;
        protected JourneyTableController owner;
        protected string cellIdentifier = "SectionCell";

        //Constructor 1
        public TableJourney(IntPtr handle) : base(handle)
        {
        }

        //Constructor 2
        public TableJourney(List<Section> items, JourneyTableController owner)
        {
            this.owner = owner;
            this.tableItems = items;
        }

        //Function to get the number of rows in a section
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count;
        }

        //Function to get the cells from the tableView and to update their informations
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            
            // request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);

            // if there are no cells to reuse, create a new one
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);

            Section section = tableItems[indexPath.Row];
            cell.TextLabel.Lines = 3;
            string info = "";

            UIImageView image = cell.ImageView;
            image.ContentMode = UIViewContentMode.ScaleAspectFit;
            image.ClipsToBounds = true;


            if (section.journey != null)
            {
                info = section.journey.name;
                info = info + "\n" + section.departure.station.name + " -> " + section.arrival.station.name;
                DateTime? departure = section.departure.departure;
                if (departure != null)
                {
                    info = info+ "\nDépart : " + departure.Value.ToString("HH:mm");
                }
                else
                {
                    info = "Départ : ...";
                }

                if (image != null)
                {
                    if (section.journey.category == "BUS" || section.journey.category == "NFO" || section.journey.category == "NFB")
                    {
                        image.Image = MaxResizeImage(UIImage.FromBundle("bus"),35,35);
                    }
                    else
                    {
                        image.Image = MaxResizeImage(UIImage.FromBundle("train"), 35, 35);
                    }
                }
            }
            else
            {
                info = "Marche";
                if (image != null)
                {
                    image.Image = MaxResizeImage(UIImage.FromBundle("walk"), 35, 35);
                }
                info = info + "\n" + section.departure.station.name + " -> " + section.arrival.station.name;
            }

            cell.TextLabel.Text = info;
            cell.TextLabel.TextAlignment = UITextAlignment.Center;
            cell.TextLabel.AdjustsFontSizeToFitWidth = true;

            return cell;
        }

        //Function to resize images
        public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
            sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

    }
}