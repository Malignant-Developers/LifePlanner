using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace ContactApp
{
    [Activity(Label = "ContactApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            Button contactos = FindViewById<Button>(Resource.Id.btn_contactos);
            contactos.Click += delegate
            {
                Intent actividadContactos = new Intent(this, typeof(ContactosActivity));
                StartActivity(actividadContactos);
            };
        }
    }
}

