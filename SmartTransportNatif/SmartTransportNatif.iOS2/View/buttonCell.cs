using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace SmartTransportNatif.iOS2.View
{
    partial class ButtonCell : UITableViewCell
    {
        public ButtonCell (IntPtr handle) : base (handle)
        {
        }

        public ButtonCell() : base()
        {
        }

        public async void UpdateCell(string station)
        {
           
            
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }




    }
}