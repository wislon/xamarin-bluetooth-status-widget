using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Bluetooth;
using Android.Util;
using Android.OS;
using Java.Lang;
using Android.Widget;

namespace BluetoothWidget
{
    // see https://developer.android.com/guide/topics/appwidgets/index.html and
    // http://stackoverflow.com/questions/4073907/update-android-widget-from-activity?rq=1
    [BroadcastReceiver(Label = "Bluetooth Widget")]
    [IntentFilter(new string[]{ "android.appwidget.action.APPWIDGET_UPDATE", Android.Bluetooth.BluetoothAdapter.ActionStateChanged })]
    [MetaData("android.appwidget.provider", Resource = "@xml/bt_widget")]
    public class BTWidget : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);
//      for (int i = 0; i < appWidgetIds.Length; i++)
//      {
//        int appWidgetId = appWidgetIds[i];
//        RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.inital_layout);
//        appWidgetManager.UpdateAppWidget(appWidgetId, views);
//      }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Android.Bluetooth.BluetoothAdapter.ActionStateChanged)
            {
                Log.Info("BTService", "Received BT State change message");
                ProcessBTStateChangeMessage(context, intent);
            }
        }

        private void ProcessBTStateChangeMessage(Context context, Intent intent)
        {
            int oldState = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousState, -1);
            int newState = intent.GetIntExtra(BluetoothAdapter.ExtraState, -1);
            string message = string.Format("Bluetooth State Change from {0} to {1}", oldState, newState);
            Log.Info("BTService", message);

            var appWidgetManager = AppWidgetManager.GetInstance(context);
            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.initial_layout);
            Log.Debug("BTService", "this.GetType().ToString(): {0}", this.GetType().ToString());

            // http://stackoverflow.com/questions/4073907/update-android-widget-from-activity?rq=1 - 'this.Class' is the key (not .GetType())
            ComponentName thisWidget = new ComponentName(context, this.Class);
            Log.Debug("BTService", thisWidget.FlattenToString());
            string miniMessage = string.Format("{0}->{1}", oldState, newState);
            Log.Debug("BTService", "remoteViews: {0}", remoteViews.ToString());
            int imgResource = (newState == 10 || newState == 11) ? Resource.Drawable.bluetooth_off : Resource.Drawable.bluetooth_on;

            remoteViews.SetImageViewResource(Resource.Id.imgBluetooth, imgResource);
            appWidgetManager.UpdateAppWidget(thisWidget, remoteViews);
        }
    }
}