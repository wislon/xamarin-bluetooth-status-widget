using System.Collections;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Bluetooth;
using Android.Util;
using Android.Widget;

namespace BluetoothWidget
{
  // see https://developer.android.com/guide/topics/appwidgets/index.html and
  // http://stackoverflow.com/questions/4073907/update-android-widget-from-activity?rq=1
  [BroadcastReceiver(Label = "Bluetooth Widget")]
  [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE", Android.Bluetooth.BluetoothAdapter.ActionStateChanged })]
  [MetaData("android.appwidget.provider", Resource = "@xml/bt_widget")]
  public class BTWidget : AppWidgetProvider
  {
    private const string APP_NAME = "BTWidget";

    public override void OnReceive(Context context, Intent intent)
    {
      if (intent.Action == Android.Bluetooth.BluetoothAdapter.ActionStateChanged)
      {
        Log.Info(APP_NAME, "Received BT Action State change message");
        ProcessBTStateChangeMessage(context, intent);
      }
    }

    private void ProcessBTStateChangeMessage(Context context, Intent intent)
    {
      int oldState = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousState, -1);
      int newState = intent.GetIntExtra(BluetoothAdapter.ExtraState, -1);
      string message = string.Format("Bluetooth State Change from {0} to {1}", oldState, newState);
      Log.Info(APP_NAME, message);

      var appWidgetManager = AppWidgetManager.GetInstance(context);
      RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.initial_layout);
      Log.Debug(APP_NAME, "this.GetType().ToString(): {0}", this.GetType().ToString());

      // http://stackoverflow.com/questions/4073907/update-android-widget-from-activity?rq=1 - 'this.Class' is the key (not .GetType())
      ComponentName thisWidget = new ComponentName(context, this.Class);
      Log.Debug(APP_NAME, thisWidget.FlattenToString());
      Log.Debug(APP_NAME, string.Format("{0}->{1}", oldState, newState));
      Log.Debug(APP_NAME, "remoteViews: {0}", remoteViews.ToString());
      int imgResource = Resource.Drawable.bluetooth_off;
      switch((Android.Bluetooth.State)newState)
      {
        case Android.Bluetooth.State.Off:
        case Android.Bluetooth.State.TurningOn:
          {
          imgResource = Resource.Drawable.bluetooth_off;
          break;
        }

        case Android.Bluetooth.State.On:
        case Android.Bluetooth.State.TurningOff:
          {
          imgResource = Resource.Drawable.bluetooth_on;
          break;
        }

        default:
        {
          imgResource = Resource.Drawable.bluetooth_off;
          break;
        }

      }
      
      remoteViews.SetImageViewResource(Resource.Id.imgBluetooth, imgResource);
      appWidgetManager.UpdateAppWidget(thisWidget, remoteViews);
    }
  }
}