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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Plugin.Geolocator.Abstractions;
using Android.Locations;
using Android.Graphics.Drawables;

namespace SmartTransportNatif.Droid
{
    [Activity(Label = "MapsActivity", ParentActivity = typeof(JourneyActivity), ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MapsActivity : Activity
    {
        MapFragment mapFrag;
        GoogleMap map;
        Position position;
        TextView stationtxV;
        TextView distanceTxV;
        TextView timeTxV;
        ProgressDialog progress;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Maps);

            //Toolbar configuration and displaying
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Trajet à pied vers "+ SharedObjects.closestStation.name;
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);

            progress = new ProgressDialog(this);

            mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            stationtxV = FindViewById<TextView>(Resource.Id.stationLbl);
            distanceTxV = FindViewById<TextView>(Resource.Id.distanceLbl);
            timeTxV = FindViewById<TextView>(Resource.Id.timeLbl);

            //getting the position from the PCL
            position = SharedObjects.pcl.getPosition();

            stationtxV.Text = SharedObjects.closestStation.name;
            distanceTxV.Text = "Distance: "+SharedObjects.closestStation.distance+ "m";
            timeTxV.Text = "";

            if (map != null)
            {
                map.Clear();
                map.Dispose();


            }
            var mapReadyCallback = new OnMapReadyClass();
            mapReadyCallback.MapReadyAction += delegate (GoogleMap googleMap)
            {
                map = googleMap;
                
                fnInitiateMapAsync();
                
            };

            mapFrag.GetMapAsync(mapReadyCallback);


        }

        private object getResources()
        {
            throw new NotImplementedException();
        }

        //This function initilize the Map
        public void fnInitiateMapAsync()
        {

            if (map != null)
            {
                ShowLoading();
                map.MapType = GoogleMap.MapTypeSatellite; //select the map type
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(new LatLng(position.Latitude, position.Longitude)); //Target to some location hardcoded
                builder.Zoom(18); //Zoom multiplier
                builder.Bearing(45);//bearing is the compass measurement clockwise from North
                builder.Tilt(90); //tilt is the viewing angle from vertical
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                LatLng latLng = new LatLng(position.Latitude, position.Longitude);
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(latLng, 17));
                map.MyLocationEnabled = true;
                map.MapType = GoogleMap.MapTypeNormal;

                Worker pclClass = SharedObjects.pcl;
                List<Route> routes = SharedObjects.routes;

                timeTxV.Text = "Temps: " + routes[0].legs[0].duration.text;

                string encodedPoints = routes[0].overview_polyline.points;
                var lstDecodedPoints = FnDecodePolylinePoints(encodedPoints);
                //convert list of location point to array of latlng type
                var latLngPoints = new LatLng[lstDecodedPoints.Count];
                int index = 0;
                foreach (Position loc in lstDecodedPoints)
                {
                    latLngPoints[index++] = new LatLng(loc.Latitude, loc.Longitude);
                }
                var polylineoption = new PolylineOptions();
                polylineoption.InvokeColor(Android.Graphics.Color.Red);
                polylineoption.Geodesic(true);
                polylineoption.Add(latLngPoints);
                map.AddPolyline(polylineoption);
                DismissLoading();
            }
        }

        //OnMapReadyClass
        public class OnMapReadyClass : Java.Lang.Object, IOnMapReadyCallback
        {
            public GoogleMap Map { get; private set; }
            public event Action<GoogleMap> MapReadyAction;

            public void OnMapReady(GoogleMap googleMap)
            {
                Map = googleMap;

                MapReadyAction?.Invoke(Map);
            }
        }


        //function to decode,encoded points
        //Author : Suchith Madavu
        List<Position> FnDecodePolylinePoints(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                return null;
            var poly = new List<Position>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylinechars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                Position p = new Position();
                
                p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
                poly.Add(p);
            }

            return poly;
        }

        //Function to display loading popup
        private void ShowLoading()
        {

            progress.SetTitle("Chargement");
            progress.SetMessage("Veuillez patienter...");
            progress.SetCancelable(false); // disable dismiss by tapping outside of the dialog
            progress.Show();
        }

        //Function to dismiss loading popup
        private void DismissLoading()
        {
            progress.Dismiss();
        }
    }
}