using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using LifePlanner.WebReference;

namespace LifePlanner
{
    [Activity(Label = "Tareas")]
    public class TareasActivity : Activity
    {
        ToDoWS cliente = new ToDoWS();
        DataTable tabla = new DataTable();

        List<string> tareas = new List<string>();
        ListView listado;
        EditText input;
        string tareaPrevia;
        Button modificarTarea;
        Button borrarTarea;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.TareasLayout);
            listado = FindViewById<ListView>(Resource.Id.tareas);

            input = FindViewById<EditText>(Resource.Id.txt_tarea);
            Button agregarTarea = FindViewById<Button>(Resource.Id.btn_agregarTarea);
            borrarTarea = FindViewById<Button>(Resource.Id.btn_borrarTarea);
            modificarTarea = FindViewById<Button>(Resource.Id.btn_actualizarTarea);

            // Obtener el listado de tareas
            ObtenerTareas();

            // Se puede acceder el item con el evente ItemClick: https://docs.microsoft.com/en-us/xamarin/android/internals/api-design#Events_and_Listeners
            listado.ItemClick += (sender, e) =>
            {
                // Obtener el indice del item seleccionado
                int index = e.Position;
                // Capturar el contenido usando el indice para obtener el item dentro de la lista original
                string contenido = tareas[index].ToString();
                input.Text = contenido;
                // Guardar la tarea previa (por modificar) como referencia para poder encontrar su indice y ID.
                tareaPrevia = contenido;
                modificarTarea.Enabled = true;
                borrarTarea.Enabled = true;
            };
            agregarTarea.Click += delegate
            {
                // Si la tarea ya existe
                if (tareas.Contains(input.Text))
                {
                    Toast.MakeText(this, "Esa tarea ya existe", ToastLength.Short).Show();
                }
                else
                {
                    // Si la tarea aun no existe
                    int resultado;
                    resultado = cliente.CrearTarea(input.Text);
                    if (resultado > 0)
                    {
                        Toast.MakeText(this, "Tarea agregada", ToastLength.Short).Show();
                        // Actualizar el listado al mas reciente
                        ObtenerTareas();
                        LimpiarInput();
                        DeshabilitarBoton();
                    }
                    else
                    {
                        Toast.MakeText(this, "Se genero un error al agregar la tarea", ToastLength.Short).Show();
                        LimpiarInput();
                        DeshabilitarBoton();
                    }
                }
            };

            borrarTarea.Click += delegate
            {
                // Si la tarea NO existe
                if (!tareas.Contains(input.Text))
                {
                    Toast.MakeText(this, "Esa tarea No existe", ToastLength.Short).Show();
                }
                else
                {
                    // Si la tarea existe, la podemos borrar
                    // Indice en la lista, es la fila en la tabla
                    int index = tareas.IndexOf(input.Text);
                    // ID en la tabla, accediendo a la fila indicada
                    int id = Convert.ToInt32(tabla.Rows[index][0]);

                    int resultado;
                    resultado = cliente.BorrarTarea(id);
                    if (resultado > 0)
                    {
                        Toast.MakeText(this, "Tarea Eliminada", ToastLength.Short).Show();
                        // Actualizar el listado al mas reciente
                        ObtenerTareas();
                        LimpiarInput();
                        DeshabilitarBoton();
                    }
                    else
                    {
                        Toast.MakeText(this, "Se genero un error al eliminar la tarea", ToastLength.Short).Show();
                        LimpiarInput();
                        DeshabilitarBoton();
                    }
                }
            };
            modificarTarea.Click += delegate
            {
                // Modificar trabaja con una variable global, solo se puede usar esta funcion si se selecciona un item!
                if (!tareas.Contains(tareaPrevia))
                {
                    Toast.MakeText(this, "Esa tarea No existe", ToastLength.Short).Show();
                }
                else
                {
                    //! Encontrar el indice de la tarea por modificar
                    // Indice en la lista, es la fila en la tabla. Usar a la tarea previa almacenada en alcance global como referencia.
                    int index = tareas.IndexOf(tareaPrevia);
                    // ID en la tabla, accediendo a la fila indicada
                    int id = Convert.ToInt32(tabla.Rows[index][0]);

                    int resultado;
                    // Modificar la tarea con el indice adecuado y pasar la descripcion nueva
                    resultado = cliente.ActualizarTarea(id, input.Text);
                    if (resultado > 0)
                    {
                        Toast.MakeText(this, "Tarea Actualizada", ToastLength.Short).Show();
                        // Actualizar el listado al mas reciente
                        ObtenerTareas();
                        LimpiarInput();
                        DeshabilitarBoton();
                    }
                    else
                    {
                        Toast.MakeText(this, "Se genero un error al eliminar la tarea", ToastLength.Short).Show();
                        LimpiarInput();
                        DeshabilitarBoton();
                    }
                }
            };
            Button btnVolver = FindViewById<Button>(Resource.Id.btn_tareasVolver);
            btnVolver.Click += delegate
            {
                Intent menu = new Intent(this, typeof(MainActivity));
                StartActivity(menu);
            };
        }
        private void ObtenerTareas()
        {
            // Limpiar el listado, asi se evita el duplicado
            tareas.Clear();
            tabla = cliente.ObtenerTarea();
            // Un listview puede ser rellenado con un adaptador: https://forums.xamarin.com/discussion/41387/fill-listview-from-dataset
            // Similar a rellenar un GridView en WinForms
            if (tabla.Rows.Count > 0)
            {
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    tareas.Add(tabla.Rows[i][1].ToString());
                }
                ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, tareas);
                listado.Adapter = adaptador;
            }
            else
            {
                Toast.MakeText(this, "Aun no tienes tareas por hacer!", ToastLength.Long).Show();
                ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, tareas);
                listado.Adapter = adaptador;
            }
        }
        private void LimpiarInput()
        {
            input.Text = "";
        }
        private void DeshabilitarBoton()
        {
            modificarTarea.Enabled = false;
            borrarTarea.Enabled = false;
        }
    }
}