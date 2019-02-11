using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SmartTransportNatif;

namespace SmartTransportNatif.Droid
{
    [Activity(Label = "AddActivity", ParentActivity = typeof(MainActivity), ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AddActivity : Activity
    {
        Button addBtn;
        Button cancelBtn;
        EditText name;
        EditText station;
        Worker pcl;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //display settings
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.add);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            SetActionBar(toolbar);

            ActionBar.Title = "Ajouter un favori";
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);

            addBtn = FindViewById<Button>(Resource.Id.addBtn);
            cancelBtn = FindViewById<Button>(Resource.Id.cancelBtn);
            name = FindViewById<EditText>(Resource.Id.editTextName);
            station = FindViewById<EditText>(Resource.Id.editTextStation);
            pcl = SharedObjects.pcl;

            addBtn.Click += delegate
            {
                if (name.Text != null && name.Text != "" && station.Text != null && station.Text != "" )
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);

                    alert.SetTitle("Voulez-vous ajouter ce favori?");
                    alert.SetMessage("Nom: "+name.Text+"\nStation: "+station.Text);

                    //if yes, add the favorite.
                    alert.SetPositiveButton("Confirmer", (senderAlert, args) => {
                        pcl.AddFavStation(name.Text, station.Text);
                        Finish();
                    });
                    //if no, do nothing
                    alert.SetNegativeButton("Corriger", (senderAlert, args) => {

                    });
                    alert.Show();
                }
            };

            cancelBtn.Click += delegate
            {
                Finish();
            };

        }
    }
}