// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SmartTransportNatif.iOS2
{
    [Register ("JourneyTableController")]
    partial class JourneyTableController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel depArrLbl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton stationBtn { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (depArrLbl != null) {
                depArrLbl.Dispose ();
                depArrLbl = null;
            }

            if (stationBtn != null) {
                stationBtn.Dispose ();
                stationBtn = null;
            }
        }
    }
}