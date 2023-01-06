using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Google.Android.Material.TextField;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Android.Webkit;
using Java.IO;
using Stream = System.IO.Stream;

namespace MedTimer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TimerConfig _timerConfig = new TimerConfig();
        private EditText _timerIntervalTextInputEditText;
        private EditText _timerLoopCountTextInputEditText;
        private TextView _timerOutputTextInputEditText;
        private Button _timerRunButton;
        private View root;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;


            _timerIntervalTextInputEditText = FindViewById<EditText>(Resource.Id.timerInterval);
            _timerLoopCountTextInputEditText = FindViewById<EditText>(Resource.Id.timerQuantity);
            _timerOutputTextInputEditText = FindViewById<TextView>(Resource.Id.timerCounter);
            _timerRunButton = FindViewById<Button>(Resource.Id.timerStart);
            root = FindViewById<View>(Resource.Id.content);
            _timerIntervalTextInputEditText.KeyPress += (object sender, View.KeyEventArgs e) =>
            {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    _timerConfig.SetInterval(_timerIntervalTextInputEditText.Text);
                    e.Handled = true;
                }
            };
            _timerIntervalTextInputEditText.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (!e.HasFocus)
                {
                    if (!String.IsNullOrEmpty(_timerIntervalTextInputEditText.Text))
                        _timerConfig.SetInterval(_timerIntervalTextInputEditText.Text);
                    else
                        _timerIntervalTextInputEditText.Text = _timerConfig.Interval.ToString();
                }
                else
                {
                    _timerIntervalTextInputEditText.Text = string.Empty;
                }
            };

            _timerLoopCountTextInputEditText.KeyPress += (object sender, View.KeyEventArgs e) =>
            {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    _timerConfig.SetLoopCount(_timerLoopCountTextInputEditText.Text);
                    e.Handled = false;
                }
            };

            _timerLoopCountTextInputEditText.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (!e.HasFocus)
                {
                    if (!String.IsNullOrEmpty(_timerLoopCountTextInputEditText.Text))
                        _timerConfig.SetLoopCount(_timerLoopCountTextInputEditText.Text);
                    else
                        _timerLoopCountTextInputEditText.Text = _timerConfig.LoopCount.ToString();
                }
                else
                {
                    _timerLoopCountTextInputEditText.Text = string.Empty;
                }
            };

            _timerRunButton.Click += TimerRunButtonOnClick;


        }

        /// <summary>
        /// Метод подстановки значения таймера на интерфейс
        /// </summary>
        /// <param name="val"></param>
        public void ShowTimer(string val)
        {
            _timerOutputTextInputEditText.Text = val;
        }

        /// <summary>
        /// Обработка события нажатия на кнопку запуска таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerRunButtonOnClick(object sender, EventArgs e)
        {
            if (TimerCode.RunTimer(_timerConfig, ShowTimer))
                _timerRunButton.Enabled = false;
            Task.Factory.StartNew(() =>
            {
                do { }
                while (!TimerCode.TimerTick(_timerConfig, ShowTimer, _timerOutputTextInputEditText.Text));
                _timerRunButton.Enabled = true;
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }


}
