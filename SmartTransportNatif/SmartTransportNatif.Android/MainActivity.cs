using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using System.Collections.Generic;
using Android.Graphics;
using Android.Util;

namespace SmartTransportNatif.Droid
{
    [Activity(Label = "OnTime App", MainLauncher = true, Icon = "@drawable/ic_launcher", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {

        List<Button> buttons; //list of buttons for the favorite destinations
        LinearLayout llButtons; //Component to receive the buttons
        Worker pcl; //worker from the PCL


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.main);

            buttons = new List<Button>();
            llButtons = FindViewById<LinearLayout>(Resource.Id.lLbuttons);

            pcl = new Worker();
            SharedObjects.pcl = pcl; //saving the worker instance into the Shared objects class

            Dictionary<string, string> favStations = pcl.getFavStations();


            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Accueil";

        }


        protected override void OnResume()
        {
            base.OnResume(); // Always call the superclass first.
            UpdateUI(pcl.getFavStations()); 
        }

        //updates UI wite the favorite destinations list
        private void UpdateUI(Dictionary<string, string> stations)
        {
            if(buttons.Count> 0)
            {
                llButtons.RemoveAllViews();
            }


            foreach (KeyValuePair<string, string> station in stations)
            {

                Button btnFav = new Button(this);

                var metrics = Resources.DisplayMetrics;

                LinearLayout.LayoutParams parameters = new LinearLayout.LayoutParams(800, ViewGroup.LayoutParams.WrapContent);
                parameters.BottomMargin = 30;
                //btnFav.SetWidth(300);


                btnFav.SetBackgroundResource(new Color(Resource.Color.colorBlue));
                btnFav.SetTextColor(Color.White);
                btnFav.Text = station.Key;


                btnFav.LayoutParameters = parameters;


                buttons.Add(btnFav);
                llButtons.AddView(btnFav);

                //For a click on the button, redirect to Journey Activity
                btnFav.Click += delegate
                {
                    var activity = new Intent(this, typeof(JourneyActivity));
                    activity.PutExtra("station", station.Value);

                    StartActivity(activity);
                };

                //For a long click on the button, remove the selected station.
                btnFav.LongClick += delegate
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);

                    alert.SetTitle("Voulez-vous vraiment supprimer ce favori?");

                    alert.SetPositiveButton("Oui", (senderAlert, args) =>
                    {
                        pcl.RemoveFavStation(btnFav.Text);
                        UpdateUI(pcl.getFavStations());
                    });
                    alert.SetNegativeButton("Annuler", (senderAlert, args) =>
                    {
                        //Does nothing
                    });
                    alert.Show();
                };
            }



        }

        //Inflation of the toolbar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //Function for the "+" icon on the toolbar
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            StartActivity(typeof(AddActivity));
            return base.OnOptionsItemSelected(item);
        }


    }
}


