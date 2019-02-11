using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;
using Android.Runtime;
using Android.Views;
using Android.Util;
using SmartTransportNatif;
using Android.Content;
using SmartTransportNatif.Droid.Services;

namespace SmartTransportNatif.Droid
{
    [Activity(Label = "JourneyActivity", ParentActivity=typeof(MainActivity), ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class JourneyActivity : Activity
    {
        ProgressDialog progress;
        Button stationBtn;

        TextView arrDepTxtV;

        Intent startServiceIntent;
        Intent stopServiceIntent;

        String destStation;

        public const string ACTION_START_SERVICE = "SmartTransport.action.START_SERVICE";


        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

            //Intent for service starting
            startServiceIntent = new Intent(this, typeof(WalkingService));
            startServiceIntent.SetAction(ACTION_START_SERVICE);

            //Intent for service stopping
            stopServiceIntent = new Intent(this, typeof(WalkingService));

            SetContentView(Resource.Layout.journey);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            SetActionBar(toolbar);

            //Getting the selected button station in order to display the correct journey
            destStation = Intent.GetStringExtra("station") ?? "Data not available"; ;
            if (SharedObjects.destStation == null)
            {
                if(destStation == "Data not available")
                {
                    destStation = SharedObjects.destStation;
                }
                else
                {
                    SharedObjects.destStation = destStation;
                }

            }
            else if (SharedObjects.destStation != destStation)
            {
                SharedObjects.destStation = destStation;

                
            }
            else
            {
                destStation = SharedObjects.destStation;
            }
            

            ActionBar.Title = "Trajet vers "+destStation;
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);

            stationBtn = FindViewById<Button>(Resource.Id.stationBtn);
            arrDepTxtV = FindViewById<TextView>(Resource.Id.depArrTxtV);

            //Getting reference from SharedObjects pcl Worker
            Worker pclClass = SharedObjects.pcl;
            

            progress = new ProgressDialog(this);

            //Web requests
            ShowLoading();
            SharedObjects.closestStation = await pclClass.GetClosestStation();
            SharedObjects.connections = await pclClass.GetJourney(SharedObjects.closestStation.name, destStation);
            SharedObjects.routes = await pclClass.GetDirection(SharedObjects.closestStation.coordinate.x, SharedObjects.closestStation.coordinate.y);
            DismissLoading();

            stationBtn.Text = SharedObjects.closestStation.name+"   "+SharedObjects.closestStation.distance+"m";

            stationBtn.Click += delegate
            {

                StartActivity(typeof(MapsActivity));
            };

            //Time calculation and displaying
            DateTime now = DateTime.Now;
            TimeSpan remainingTime = SharedObjects.connections[0].from.departure.Value.Subtract(now);
            String depArr= "Départ dans "+remainingTime.Minutes+" minutes\nArrivée à " + SharedObjects.connections[0].to.arrival.Value.ToString("HH:mm");
            if (remainingTime.Hours > 0)
            {
                depArr = "Départ dans " + remainingTime.Hours+" heures et "+ remainingTime.Minutes + " minutes\nArrivée à " + SharedObjects.connections[0].to.arrival.Value.ToString("HH:mm");
            }
            
            arrDepTxtV.Text = depArr;

            AddSectionDetailsUI(SharedObjects.connections[0]);
            StartService(startServiceIntent);

        }

        //This function updates the section part of the journey on the UI. It's done dynamically
        private void AddSectionDetailsUI(Connection connection)
        {
            LinearLayout detailsLY = FindViewById<LinearLayout>(Resource.Id.detailsLY);
            LinearLayout.LayoutParams parameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            LayoutInflater vi = (LayoutInflater)Application.Context.GetSystemService(LayoutInflaterService);

            SharedObjects.bestConnection = connection;

            //For each section inflate a sectionDetails drawable
            foreach (Section section in connection.sections){
                View custom = vi.Inflate(Resource.Drawable.sectionDetails, null);

                TextView product = (TextView)custom.FindViewById<TextView>(Resource.Id.transport1TxtV);
                TextView locations = (TextView)custom.FindViewById<TextView>(Resource.Id.transport2TxtV);
                TextView departureTime = (TextView)custom.FindViewById<TextView>(Resource.Id.transport3TxtV);
                ImageView image = (ImageView)custom.FindViewById<ImageView>(Resource.Id.transport1ImgV);


                locations.Text = section.departure.station.name + " -> " + section.arrival.station.name;
                if (section.journey != null) {
                    product.Text = section.journey.name;
                    DateTime? departure = section.departure.departure;
                    if (departure != null)
                    {
                        departureTime.Text = "Départ : " + departure.Value.ToString("HH:mm");
                    }
                    else
                    {
                        departureTime.Text = "Départ : ...";
                    }
                    if (image != null)
                    {
                        if (section.journey.category == "BUS" || section.journey.category == "NFO" || section.journey.category == "NFB")
                        {
                            image.SetImageResource(Resource.Drawable.bus);
                        }
                        else
                        {
                            image.SetImageResource(Resource.Drawable.train);
                        }
                    }
                }
                else
                {
                    product.Text = "Marche";
                    departureTime.Text = section.walk.duration;
                    if (image != null)
                    {
                        image.SetImageResource(Resource.Drawable.walk);
                    }
                }
                ((ViewGroup)detailsLY).AddView(custom, parameters);
            }
        }


        //Function to show the loading popup
        private void ShowLoading()
        {

            progress.SetTitle("Chargement");
            progress.SetMessage("Veuillez patienter...");
            progress.SetCancelable(false); // disable dismiss by tapping outside of the dialog
            progress.Show();
        }

        //Function to dismiss the loading popup
        private void DismissLoading()
        {
            progress.Dismiss();
            stationBtn.Enabled = true;
        }

        //Function to handle the Android exception
        void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            e.Handled = true;
            Android.Util.Log.Debug("TEST", e.ToString());
        }




    }
}