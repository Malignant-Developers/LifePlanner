using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace ToDoPlanner
{
    [Activity(Label = "ToDoPlanner", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Enlace al webservice: https://somee.com/DOKA/DOC/DOLoginOrRegister.aspx
            // Usuario: TodoAppWS
            // Contraseña: 6ok6Cs&EJ&6N
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button tareas = FindViewById<Button>(Resource.Id.btn_tareas);
            tareas.Click += delegate
            {
                Intent actividadTareas = new Intent(this, typeof(activity_tareas));
                StartActivity(actividadTareas);
            };
            Button telefonos = FindViewById<Button>(Resource.Id.btn_telefonos);
            telefonos.Click += delegate
            {
                Intent actividadTelefonos = new Intent(this, typeof(activity_telefonos));
                StartActivity(actividadTelefonos);
            };
            Button correos = FindViewById<Button>(Resource.Id.btn_correos);
            correos.Click += delegate
            {
                Intent actividadCorreos = new Intent(this, typeof(activity_correos));
                StartActivity(actividadCorreos);
            };
        }
    }
}

