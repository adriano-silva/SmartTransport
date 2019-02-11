using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Data;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Newtonsoft.Json.Linq;
using MvvmCross.Plugins.File;
using MvvmCross.Platform;

namespace SmartTransportNatif
{
    public class Worker
    {

        HttpClient client; //HTTPClient to send Web requests

        Dictionary<string , string> favStations; //List of favorite stations setted by the user

        Position position; //Current position of the device

        List<Route> directions; //List of routes returned by the Google Directions API

        const int POSITION_ACCURACY_IN_METERS = 50; //Accuracy in meters for the position retrieving


        public Worker()
        {
            //manually adding elements to the list of favorites stations, for testing purpose
            favStations = new Dictionary<string, string>();
            favStations.Add("Maison", "Rosé");
            favStations.Add("École", "Fribourg, Plateau-de-Pérolles");

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            position = new Position();
            directions = new List<Route>();
        }

        /*
         * <summary> This function retrieves the current position of the device</summary>
         * <returns>Task<Position> Position object </returns>
         */
        public async Task<Position> GetDevicePosition()
        {
            Position pos = new Position();
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = POSITION_ACCURACY_IN_METERS; // desired accuracy in meters
            try
            {

                pos = await locator.GetPositionAsync(61000); //10 seconds

            }
            catch (Exception ex)
            {
                Debug.WriteLine("WARNING:    Unable to get Location: " + ex);
            }

            return pos;
        }


        /*
         * <summary> This function send a GET request to the Transport OpenData API to get a list
         * of the nearest stations of the current device position</summary>
         * <returns>Task<Station> Station object </returns>
         */
        public async Task<Station> GetClosestStation()
        {

            List<Station> stations = new List<Station>();

            position = await GetDevicePosition();
            

            String restUrl = "http://transport.opendata.ch/v1/locations?x=" + position.Latitude + "&y=" + position.Longitude;

            var uri = new Uri(restUrl);

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var root = JsonConvert.DeserializeObject<RootObject>(content);
                    stations = root.stations;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"				ERROR {0}", ex);
            }

            Debug.WriteLine("RESPONSE1: " + stations[0].name);

            return stations[0];
        }



        /// <summary> This function send a GET request to the Transport OpenData API to 
        ///get a journey between two stations</summary>
        ///<returns>Task<Connection> Object representing a single connection between two stations </returns>
        public async Task<List<Connection>> GetJourney(String fromStation, String toStation)
        {
            List<Connection> connections = new List<Connection>();

            String restUrl = "http://transport.opendata.ch/v1/connections?from="+fromStation+"&to="+toStation;

            var uri = new Uri(restUrl);

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("CONTENT =       " + content);
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                    jsonSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                    var root = JsonConvert.DeserializeObject<RootObject2>(content);
                    connections = root.connections;
                    Debug.WriteLine("RESPONSE2: " + connections[0].to.station.name);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"				ERROR {1}", ex);
            }

            return connections;

        }

        /// <summary> This function send a GET request to the Google Directions API to 
        ///get directions between the current position and the location provided in parameters</summary>
        ///<returns>Task<Route> Object representing the routes between two locations</returns>
        public async Task<List<Route>> GetDirection(double latitude, double longitude)
        {
            List<Route> routes = null;

            String restUrl = "https://maps.googleapis.com/maps/api/directions/json?origin="+position.Latitude+","+position.Longitude+"&destination="+latitude+","+longitude+ "&mode=walking&key=AIzaSyB75Jpg_WKsrzDPG32QyMc_3lP2TjVp8cE";
            var uri = new Uri(restUrl);

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    //Debug.WriteLine("CONTENT =       " + content);
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                    jsonSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                    var root = JsonConvert.DeserializeObject<RootObjectDirection>(content);
                    routes = root.routes;
                    Debug.WriteLine("RESPONSE3: " + routes[0].summary);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"				ERROR {1}", ex);
            }

            return routes;
        }


        /// <summary> This function get the current device position and 
        /// send a GET request to the Google Directions API to 
        /// receive the routes between the current position and the provided location</summary>
        ///<returns>Task<Route> Object representing the routes between two locations</returns>
        public async Task<List<Route>> WalkingServiceWorker(double latitude, double longitude)
        {
            position = await GetDevicePosition();
            List<Route> routes = await GetDirection(latitude, longitude);


            return routes;
        }

        /// <summary> This function returns the favorite destinations of the user</summary>
        ///<returns>Dictionary<string> List of personal names and stations from the favorite list</returns>
        public Dictionary<string,string> getFavStations()
        {
            return favStations;
        }


        /// <summary> This function adds a destination to the favorite list</summary>
        public void AddFavStation(string name, string station)
        {
            favStations.Add(name, station);
        }

        /// <summary> This function removes a favorite destination from the favorite list</summary>
        public void RemoveFavStation(string name)
        {
            favStations.Remove(name);
        }

        /// <summary> This function returns the asved position</summary>
        public Position getPosition()
        {
            return position;
        }
    }
}

