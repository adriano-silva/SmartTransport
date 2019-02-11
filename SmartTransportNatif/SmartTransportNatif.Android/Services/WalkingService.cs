using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;
using Android.Runtime;
using Java.Lang;
using System.Collections.Generic;

namespace SmartTransportNatif.Droid.Services
{
    [Service]
    public class WalkingService : Android.App.Service
    {

        // This is any integer value unique to the application.
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const string NOTIFICATION_BROADCAST_ACTION = "SmartTransport.Notification.Action";
        public const string ACTION_START_SERVICE = "SmartTransport.action.START_SERVICE";
        public const string ACTION_STOP_SERVICE = "SmartTransport.action.STOP_SERVICE";
        public const string ACTION_RESTART_TIMER = "SmartTransport.action.RESTART_TIMER";
        public const string ACTION_MAIN_ACTIVITY = "SmartTransport.action.MAIN_ACTIVITY";

        public const int EXTRA_MINUTES = 1; //extra time for the calculations, to mitigate the position innaccuracies
        public string notif_text = "";

        bool isStarted;

        static readonly string TAG = typeof(WalkingService).FullName;

        private Handler mHandler;

        private Worker pcl; //Worker from the PCL

        private volatile bool running;

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");
            mHandler = new Handler();
            pcl = SharedObjects.pcl;
            running = true;
        }



        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {

            //If the intent action is start, start service
            if (intent.Action.Equals(ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    Log.Info(TAG, "OnStartCommand: The service is already running.");
                }
                else
                {
                    Log.Info(TAG, "OnStartCommand: The service is starting.");
                    UpdateNotifText();
                    RegisterForegroundService(false);
                    CheckWalkingPace();

                    isStarted = true;
                }
            }
            else if (intent.Action.Equals(ACTION_STOP_SERVICE)) //If intent action is stop, stop service
            {
                Log.Info(TAG, "OnStartCommand: The service is stopping.");
                StopForeground(true);
                StopSelf();
                isStarted = false;
                running = false;

            }

            return StartCommandResult.Sticky;
        }


        //This function checks the walking pace by sending a Get request to Google Directions API. It calculates and sets the notification
        //text
        public async void CheckWalkingPace()
        {

            var timer = new System.Threading.Thread(async () =>
            {
                bool firstTime = true;
                bool leave = true;
                while (running)
                {
                    if (!firstTime)
                    {
                        System.Threading.Thread.Sleep(60000);
                    }
                    SharedObjects.routes = await pcl.WalkingServiceWorker(SharedObjects.closestStation.coordinate.x, SharedObjects.closestStation.coordinate.y);


                    //transport Departure time
                    DateTime? transportDeparture = SharedObjects.bestConnection.from.departure;

                    //walking duration in seconds to the station
                    int durationSecs = SharedObjects.routes[0].legs[0].duration.value;
                    if (durationSecs < 60)
                    {
                        durationSecs = 60;
                    }

                    durationSecs += 60; //Extra minute

                    //walking duration in a TimeSpan
                    TimeSpan walkDuration = TimeSpan.FromSeconds(durationSecs);

                    //Departure time from the actual position
                    DateTime walkDeparture = transportDeparture.Value.Subtract(walkDuration);

                    DateTime now = DateTime.Now;
                    TimeSpan remainingTime = walkDeparture.Subtract(now);




                    //if transport Departure time is past, stop service
                    if (transportDeparture.Value <= DateTime.Now)
                    {
                        running = false;
                        StopForeground(true);
                        var stopServiceIntent = new Intent(this, GetType());
                        StopService(stopServiceIntent);
                    }
                    else
                    {
                        if (now < walkDeparture.Subtract(new TimeSpan(0, EXTRA_MINUTES, 0)))//if transportDeparture time is still in future
                        {
                            if (remainingTime.Hours == 0)
                            {
                                notif_text = "Vous devriez partir dans " + remainingTime.Minutes + " minutes";
                            }
                            else
                            {
                                notif_text = "Vous devriez partir dans " + remainingTime.Hours + " heure(s) et" + remainingTime.Minutes + " minute(s)";
                            }
                            UpdateNotifText();
                            RegisterForegroundService(false);
                        }
                        else
                        {
                            
                            if (now.Add(walkDuration).Add(new TimeSpan(0, EXTRA_MINUTES, 0)) >= transportDeparture)
                            {
                                if (leave)
                                {
                                    notif_text = "C'est le moment de partir!";
                                    leave = false;
                                }
                                else
                                {
                                    notif_text = "Augmentez votre rythme de marche pour ne pas louper votre transport";
                                }
                                UpdateNotifText();
                                RegisterForegroundService(true);
                            }
                            else if (now.Add(walkDuration) < transportDeparture)
                            {
                                notif_text = "Vous êtes dans les temps";
                                UpdateNotifText();
                                RegisterForegroundService(false);
                            }
                        }
                        Log.Info(TAG, "The thread is running");
                    }
                    firstTime = false;
                }
            });


            timer.Start(); //Start the thread

        }

        //Create or update the notification
        public void UpdateNotifText()
        {
            DateTime? departure = SharedObjects.bestConnection.from.departure;

            int durationSecs = SharedObjects.routes[0].legs[0].duration.value;
            if (durationSecs < 60)
            {
                durationSecs = 60;
            }

            durationSecs += 60; //Extra minute
            TimeSpan t = TimeSpan.FromSeconds(durationSecs);

            DateTime diff = departure.Value.Subtract(t);

            DateTime now = DateTime.Now;
            TimeSpan remainingTime = diff.Subtract(now);


        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        //Register the foreground service. 
        void RegisterForegroundService(bool vibrate)
        {
            long[] vibrations = new long[] { 0, 0, 0, 0 };

            if (vibrate)
            {
                vibrations = new long[] { 0, 200, 200, 200 };
            }

            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(notif_text)
                .SetSmallIcon(Resource.Drawable.ic_notification_icon)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .SetVibrate(vibrations)
                .AddAction(BuildStopServiceAction())
                .Build();

            // Enlist this instance of the service as a foreground service
            
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        /// <summary>
        /// Builds a PendingIntent that will display the main activity of the app. This is used when the 
        /// user taps on the notification; it will take them to the main activity of the app.
        /// </summary>
        /// <returns>The content intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        /// <summary>
        /// Builds the Notification.Action that will allow the user to stop the service via the
        /// notification in the status bar
        /// </summary>
        /// <returns>The stop service action.</returns>
        Notification.Action BuildStopServiceAction()
        {
            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            var builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause,
                                                          GetText(Resource.String.stop_service),
                                                          stopServicePendingIntent);
            return builder.Build();

        }

    }
}