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
using ToDoPlanner.WebReference;

namespace ToDoPlanner
{
    [Activity(Label = "activity_correos")]
    public class activity_correos : Activity
    {
        WebReference.ToDoWS cliente = new ToDoWS();
        DataTable tabla = new DataTable();

        List<string> correos = new List<string>();
        ListView listado;
        EditText input;
        EditText inputDescripcion;
        string correoPrevio;
        string descripcionPrevia;
        Button modificarCorreo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_correos);
            listado = FindViewById<ListView>(Resource.Id.correos);

            input = FindViewById<EditText>(Resource.Id.txt_correo);
            inputDescripcion = FindViewById<EditText>(Resource.Id.txt_correoDescripcion);

            Button agregarCorreo = FindViewById<Button>(Resource.Id.btn_agregarCorreo);
            Button borrarCorreo = FindViewById<Button>(Resource.Id.btn_borrarCorreo);
            modificarCorreo = FindViewById<Button>(Resource.Id.btn_actualizarCorreo);

            // Obtener el listado de correos
            ObtenerCorreos();
            // Se puede acceder el item con el evente ItemClick: https://docs.microsoft.com/en-us/xamarin/android/internals/api-design#Events_and_Listeners
            listado.ItemClick += (sender, e) =>
            {
                // Obtener el indice del item seleccionado
                int index = e.Position;
                // Capturar el contenido usando el indice para obtener el item dentro de la lista original
                string contenido = correos[index].ToString();
                // Referencia: https://www.geeksforgeeks.org/c-sharp-substring-method/
                // La cadena viene unida desde la lista, por lo cual sus elementos deben ser separados independientemente
                // Definir el indice del caracter separador
                int separador = contenido.LastIndexOf(":");
                // Separar el numero, desde el inicio hasta antes del separador
                string numero = contenido.Substring(0, separador);
                // Separar la descripcion desde despues del separador en adelanto
                string descripcion = contenido.Substring(separador + 1);
                // Asignar los valores en las cajas de texto
                input.Text = numero;
                inputDescripcion.Text = descripcion;
                // Guardar la tarea previa (por modificar) como referencia para poder encontrar su indice y ID.
                correoPrevio = contenido;
                modificarCorreo.Enabled = true;

            };
            agregarCorreo.Click += delegate
            {
                if (input.Text != "" && inputDescripcion.Text != "")
                {
                    if (correos.Contains(input.Text + ":" + inputDescripcion.Text))
                    {
                        Toast.MakeText(this, "Ese correo ya existe", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Si la tarea aun no existe
                        int resultado;
                        string correo = input.Text;
                        resultado = cliente.CrearCorreo(input.Text, inputDescripcion.Text);
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Correo agregado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerCorreos();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al agregar ese correo", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "Por favor escriba un correo y una descripccion", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            borrarCorreo.Click += delegate
            {
                if (input.Text != "" && inputDescripcion.Text != "")
                {
                    // Si la tarea NO existe
                    if (!correos.Contains(input.Text + ":" + inputDescripcion.Text))
                    {
                        Toast.MakeText(this, "Ese correo No existe", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Si la tarea existe, la podemos borrar
                        // Indice en la lista, es la fila en la tabla
                        int index = correos.IndexOf(input.Text + ":" + inputDescripcion.Text);
                        // ID en la tabla, accediendo a la fila indicada
                        int id = Convert.ToInt32(tabla.Rows[index][0]);

                        int resultado;
                        resultado = cliente.BorrarCorreo(id);
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Correo Eliminado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerCorreos();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al eliminar el correo", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }

                }
                else
                {
                    Toast.MakeText(this, "Por favor escriba un correo y una descripcion", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            modificarCorreo.Click += delegate
            {
                // Modificar trabaja con una variable global, solo se puede usar esta funcion si se selecciona un item!
                if (input.Text != "" && inputDescripcion.Text != "")
                {

                    if (!correos.Contains(correoPrevio))
                    {
                        Toast.MakeText(this, "Ese correo No existe", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Encontrar el indice de la tarea por modificar
                        // Indice en la lista, es la fila en la tabla. Usar a la tarea previa almacenada en alcance global como referencia.
                        int index = correos.IndexOf(correoPrevio);
                        // ID en la tabla, accediendo a la fila indicada
                        int id = Convert.ToInt32(tabla.Rows[index][0]);

                        int resultado;
                        // Modificar la tarea con el indice adecuado y pasar la descripcion nueva
                        resultado = cliente.ActualizarCorreo(id, input.Text, inputDescripcion.Text);
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Correo Actualizado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerCorreos();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al eliminar el correo", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "Por favor escriba un correo y una descripcion", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            Button btnVolver = FindViewById<Button>(Resource.Id.btn_correosVolver);
            btnVolver.Click += delegate
            {
                Intent menu = new Intent(this, typeof(MainActivity));
                StartActivity(menu);
            };

        }
        private void ObtenerCorreos()
        {
            // Limpiar el listado, asi se evita el duplicado
            correos.Clear();
            tabla = cliente.ObtenerCorreos();
            // Un listview puede ser rellenado con un adaptador: https://forums.xamarin.com/discussion/41387/fill-listview-from-dataset
            // Similar a rellenar un GridView en WinForms
            if (tabla.Rows.Count > 0)
            {
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    correos.Add(tabla.Rows[i][1].ToString() + ":" + tabla.Rows[i][2].ToString());
                }
                ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, correos);
                listado.Adapter = adaptador;
            }
            else
            {
                Toast.MakeText(this, "Aun no tienes correos registrados", ToastLength.Long).Show();
            }
        }
        private void LimpiarInput()
        {
            input.Text = "";
            inputDescripcion.Text = "";
        }
        private void DeshabilitarBoton()
        {
            modificarCorreo.Enabled = false;
        }
    }
}