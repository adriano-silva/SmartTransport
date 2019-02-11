using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace SmartTransportNatif.iOS2.View
{
    public partial class SectionCell : UITableViewCell
    {

        public Section section;

        public SectionCell (IntPtr handle) : base (handle)
        {
        }

        public async void UpdateCell(Section section)
        {

        }
    }
}
