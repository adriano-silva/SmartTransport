using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using CoreLocation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace SmartTransportNatif.iOS
{
    public class JourneyViewController : UIViewController
    {

        LoadingOverlay loadPop;
        public string destStation { get; set; }
        List<Connection> connections;
        UILabel depArrLbl;
        UIStackView journeyDetails;

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;
            Title = "Trajet";

            CLLocationManager manager = new CLLocationManager();
            manager.RequestAlwaysAuthorization();
            manager.RequestWhenInUseAuthorization();

            Worker pcl = new Worker();



            var stationBtn = new UIButton(new CGRect(0, 0, 280, 44));
            
            stationBtn.BackgroundColor = new UIColor(red: 0.13f, green: 0.59f, blue: 0.95f, alpha: 1.0f);
            stationBtn.SetTitleColor(new UIColor(1, 1, 1, 1), UIControlState.Normal);
            //stationBtn.Font = UIFont.FromName("Helvetica-Bold", 20f);
            stationBtn.SetTitle("", UIControlState.Normal);
            stationBtn.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            stationBtn.TitleLabel.TextAlignment = UITextAlignment.Center;

            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds); // using field from step 2

            var stkView = new UIStackView(bounds);
            stkView.Axis = UILayoutConstraintAxis.Vertical;
            stkView.Distribution = UIStackViewDistribution.FillEqually;
            stkView.Spacing = 5;

            stkView.AddArrangedSubview(stationBtn);

            journeyDetails = new UIStackView(new CGRect(0, 0, 300, 300));
            journeyDetails.Axis = UILayoutConstraintAxis.Vertical;
            journeyDetails.Distribution = UIStackViewDistribution.FillEqually;
            journeyDetails.Spacing = 20;


            //View.AddSubview(customLabel);
            //View.AddConstraints(customLabel.WithSameCenterX(View));
            View.Add(loadPop);
            

            

            Station station = await pcl.GetClosestStation();
            stationBtn.SetTitle(station.name+"\n"+station.distance+"m",UIControlState.Normal);
            if (destStation != null && station != null)
            {
                connections = await pcl.GetJourney(station.name, destStation);
                addJourneyDetails(connections[0]);
            }
            View.AddSubview(stkView);
            View.AddSubview(journeyDetails);
            loadPop.Hide();

        }


        public void addJourneyDetails(Connection connection)
        {
            depArrLbl = new UILabel(new CGRect(0, 0, 280, 44));
            depArrLbl.LineBreakMode = UILineBreakMode.CharacterWrap;
            depArrLbl.Lines = 2;
            depArrLbl.TextAlignment = UITextAlignment.Center;
            depArrLbl.TextColor = new UIColor(red: 0.50f, green: 0.50f, blue: 0.50f, alpha: 1.0f);

            DateTime now = DateTime.Now;
            TimeSpan remainingTime = connections[0].from.departure.Value.Subtract(now);
            String depArr = "Départ dans " + remainingTime.Minutes + " minutes\nArrivée à " + connections[0].to.arrival.Value.ToString("HH:mm");
            if (remainingTime.Hours > 0)
            {
                depArr = "Départ dans " + remainingTime.Hours + "heures et " + remainingTime.Minutes + " minutes\nArrivée à " + connections[0].to.arrival.Value.ToString("HH:mm");
            }
            depArrLbl.Text = depArr;
            journeyDetails.AddArrangedSubview(depArrLbl);

            foreach (Section section in connection.sections)
            {
                var sectionDetails = new UIStackView();
                sectionDetails.Axis = UILayoutConstraintAxis.Horizontal;
                sectionDetails.Distribution = UIStackViewDistribution.Fill;
                sectionDetails.Spacing = 5;

                var info = new UILabel(new CGRect(0, 0, 280, 44));
                info.LineBreakMode = UILineBreakMode.CharacterWrap;
                info.Lines = 3;
                info.TextAlignment = UITextAlignment.Center;
                info.TextColor = new UIColor(red: 0.50f, green: 0.50f, blue: 0.50f, alpha: 1.0f);

                sectionDetails.AddArrangedSubview(info);


                
                if (section.journey != null)
                {
                    info.Text = section.journey.name;
                    info.Text = info.Text + "\n"+section.departure.station.name + " -> " + section.arrival.station.name;
                    DateTime? departure = section.departure.departure;
                    if (departure != null)
                    {
                        info.Text = info.Text+ "\nDépart : " + departure.Value.ToString("HH:mm");
                    }
                    else
                    {
                        info.Text = "Départ : ...";
                    }
                    //if (image != null)
                    //{
                    //    if (section.journey.category == "BUS" || section.journey.category == "NFO" || section.journey.category == "NFB")
                    //    {
                    //        image.SetImageResource(Resource.Drawable.bus);
                    //    }
                    //    else
                    //    {
                    //        image.SetImageResource(Resource.Drawable.train);
                    //    }
                    //}
                }
                else
                {
                    info.Text = "Marche";
                    info.Text = info.Text + "\n" + section.departure.station.name + " -> " + section.arrival.station.name;
                    //if (image != null)
                    //{
                    //    image.SetImageResource(Resource.Drawable.walk);
                    //}
                }
                journeyDetails.AddArrangedSubview(sectionDetails);
            }
        }


    }
}
