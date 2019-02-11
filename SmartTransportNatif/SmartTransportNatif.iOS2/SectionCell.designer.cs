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
    [Register ("SectionCell")]
    partial class SectionCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SmartTransportNatif.iOS2.SectionCell buttonCel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (buttonCel != null) {
                buttonCel.Dispose ();
                buttonCel = null;
            }
        }
    }
}