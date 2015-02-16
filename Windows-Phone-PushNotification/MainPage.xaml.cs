using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using System.Text;
using com.shephertz.app42.paas.sdk.windows;
using com.shephertz.app42.paas.sdk.windows.push;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;

namespace Windows_Phone_PushNotification
{
    public partial class MainPage : PhoneApplicationPage, App42Callback
    {
        public ProgressIndicator indicator = new ProgressIndicator{IsVisible = true, IsIndeterminate = true, Text = "Please wait while app is loading..."};

        ServiceAPI sp = new ServiceAPI("1724bf1e455933c620ce55cfb72263d929858524dc78322ceed83b1a744c011c", "7d2a181c006ac4f7aa16a3d95746c67e16146c4efbfe3307661eda1d63d4fd1e");
        PushNotificationService pushObj = null;
		String userId = "engifestuser";
        NotificationCallBack callback;
        public MainPage()
        {
             HttpNotificationChannel channel;

             SystemTray.SetProgressIndicator(this, indicator);

             pushObj = sp.BuildPushNotificationService();

             String channelName = "App42PushNotificationTilest";

            InitializeComponent();

            channel = HttpNotificationChannel.Find(channelName);

            callback = new NotificationCallBack();

            if (channel == null)
            {
                channel = new HttpNotificationChannel(channelName);

                channel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                channel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
                channel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                channel.Open();
                Collection<Uri> TileLocations = new Collection<Uri>();
                //  remote images in the tile
                TileLocations.Add(new Uri("http://api.shephertz.com/"));
                channel.BindToShellTile(TileLocations);
                channel.BindToShellToast();

            }
            else
            {
                channel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                channel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
                channel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);
                StoreURIWithApp42(channel.ChannelUri.ToString());

            }
        }

       void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            StoreURIWithApp42(e.ChannelUri.ToString());
        }

        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("error occurred.",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }

        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("App42 Notification {0}:\n", DateTime.Now.ToShortTimeString());

             foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

            Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));

        }

        void StoreURIWithApp42(String ChannelUri) 
        {
            pushObj.StoreDeviceToken(userId, ChannelUri, this);
            
        }

        void App42Callback.OnException(App42Exception exception)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                indicator.IsVisible = false;
            });
            Console.WriteLine(exception.ToString());
        }

        void App42Callback.OnSuccess(object response)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                indicator.IsVisible = false;
            });
            Console.WriteLine(response.ToString());
       
        }

        private void SendTost(object sender, RoutedEventArgs e)
        {
            pushObj.SendPushToastMessageToUser(userId, tb1.Text, tb2.Text, "/MainPage.xaml", callback);
        }

        private void SendTile(object sender, RoutedEventArgs e)
        {
            Tile tileData = new Tile();
            tileData.SetBackgroundImage("http://api.shephertz.com/images/0.1/zone-images/BPaaS.png");
            tileData.SetCount("2");
            tileData.SetTitle("What's up");
            tileData.SetBackContent("Notification");
            tileData.SetBackBackgroundImage("http://api.shephertz.com/images/0.1/zone-images/Push-Notification.png");
            pushObj.SendPushTileMessageToUser(userId, tileData, callback);
         }
       
    }
}
