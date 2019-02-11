using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using Cirrious.FluentLayouts.Touch;

namespace SmartTransportNatif.iOS
{
    public class MainViewController : UIViewController
    {

        UIButton btn;
        JourneyViewController journey;
        Dictionary<string, string> favStations;
        UIStackView stkView;
        Worker pcl;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            View.BackgroundColor = UIColor.White;
            Title = "Accueil";

            journey = new JourneyViewController();

            var bounds = UIScreen.MainScreen.Bounds;

            pcl = new Worker();
            favStations = pcl.getFavStations();

            var img = new UIImageView();
            img.Image = UIImage.FromFile("train.png");
            img.Frame = new CGRect(0, 0, 10, 10);





            stkView = new UIStackView(bounds);
            stkView.Axis = UILayoutConstraintAxis.Vertical;
            stkView.Distribution = UIStackViewDistribution.EqualCentering;
            stkView.Spacing = 4;

            stkView.TranslatesAutoresizingMaskIntoConstraints = true;

            stkView.AddArrangedSubview(img);

            View.AddSubview(stkView);

            foreach(KeyValuePair<string, string> entry in favStations)
            {
                addButton(entry.Key, entry.Value);
            }

            //Constraints
            //View.SetNeedsUpdateConstraints();
        }

        public override void UpdateViewConstraints()
        {
            //ButtonConstraints(View);
            base.UpdateViewConstraints();
        }

        public void ButtonConstraints(UIButton btn)
        {

            View.AddConstraints(
                btn.WithSameCenterX(View)
                );
            // Center Text Field Relative to Page View

        }

        void addButton(string name, string station)
        {
            var btn = UIButton.FromType(UIButtonType.Custom);
            btn.Frame = new CGRect(0, 0, 10, 15);
            btn.SetTitle(name, UIControlState.Normal);
            btn.BackgroundColor = new UIColor(red: 0.13f, green: 0.59f, blue: 0.95f, alpha: 1.0f);
            btn.SetTitleColor(new UIColor(1,1,1,1), UIControlState.Normal);

            btn.TouchUpInside += (sender, e) =>
            {
                journey.destStation = favStations[btn.Title(UIControlState.Normal)];
                this.NavigationController.PushViewController(journey, true);
            };

            stkView.AddArrangedSubview(btn);
            //View.AddSubview(btn);
        }

    }
}