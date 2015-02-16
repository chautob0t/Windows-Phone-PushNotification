using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using com.shephertz.app42.paas.sdk.windows;
using System.Windows.Threading;

namespace Windows_Phone_PushNotification
{
    public class NotificationCallBack : App42Callback
    {

        void App42Callback.OnException(App42Exception exception)
        {
            // here Exception is handled
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(exception.Message);
            });
        }

        void App42Callback.OnSuccess(object response)
        {
            // here success is shown
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(response.ToString());
            });

        }


    }
}
