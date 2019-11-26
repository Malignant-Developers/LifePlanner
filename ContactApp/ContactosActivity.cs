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
using ContactApp.WebReference;

namespace ContactApp
{
    [Activity(Label = "ContactosActivity")]
    public class ContactosActivity : Activity
    {
        WebReference.ToDoWS cliente = new ToDoWS();
        DataTable tabla = new DataTable();

        List<string> usuarios = new List<string>();
        ListView listado;
        EditText inputNombre;
        EditText inputCorreo;
        EditText inputTelefono;

        string contenidoPrevio;
        //string nombrePrevio;
        //string correoPrevia;
        //string telefonoPrevio;

        Button modificarUsuario;
        Button borrarUsuario;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_contactos);
            listado = FindViewById<ListView>(Resource.Id.usuarios);

            inputNombre = FindViewById<EditText>(Resource.Id.txt_nombre);
            inputCorreo = FindViewById<EditText>(Resource.Id.txt_correo);
            inputTelefono = FindViewById<EditText>(Resource.Id.txt_telefono);

            Button agregarUsuario = FindViewById<Button>(Resource.Id.btn_agregar);
            borrarUsuario = FindViewById<Button>(Resource.Id.btn_borrar);
            modificarUsuario = FindViewById<Button>(Resource.Id.btn_actualizar);

            // Obtener el listado de correos
            ObtenerUsuarios();
            // Se puede acceder el item con el evente ItemClick: https://docs.microsoft.com/en-us/xamarin/android/internals/api-design#Events_and_Listeners
            listado.ItemClick += (sender, e) =>
            {
                // Obtener el indice del item seleccionado
                int index = e.Position;
                // Capturar el contenido usando el indice para obtener el item dentro de la lista original
                string contenido = usuarios[index].ToString();
                // Referencia: https://www.geeksforgeeks.org/c-sharp-substring-method/
                // La cadena viene unida desde la lista, por lo cual sus elementos deben ser separados independientemente
                // Definir el indice del caracter separador

                int separadorCorreo = contenido.LastIndexOf(":");
                int separadorTelefono = contenido.LastIndexOf("-");
                // Separar el nombre, desde el inicio hasta antes del separador
                string nombre = contenido.Substring(0, separadorCorreo);
                // Separar el correo desde el separador en adelante
                int spanCorreo = contenido.Length - separadorCorreo - (contenido.Length - separadorTelefono);
                string correo = contenido.Substring(separadorCorreo + 1, spanCorreo-1);
                // Separar el telefono desde el separador en adelante
                string telefono = contenido.Substring(separadorTelefono+1);
                // Asignar los valores en las cajas de texto
                inputNombre.Text = nombre;
                inputCorreo.Text = correo;
                inputTelefono.Text = telefono;
                // Guardar la tarea previa (por modificar) como referencia para poder encontrar su indice y ID.
                contenidoPrevio = contenido;
                modificarUsuario.Enabled = true;
                borrarUsuario.Enabled = true;
            };
            agregarUsuario.Click += delegate
            {
                if (inputNombre.Text != "" || inputCorreo.Text != "" || inputTelefono.Text != "")
                {
                    if (usuarios.Contains(inputNombre.Text + ":" + inputCorreo.Text + "-" + inputTelefono))
                    {
                        Toast.MakeText(this, "Ese contacto ya existe!", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Si la tarea aun no existe
                        int resultado;
                        string nombre = inputNombre.Text;
                        string correo = inputCorreo.Text;
                        int telefono = Convert.ToInt32(inputTelefono.Text);
                        resultado = cliente.CrearUsario(nombre, correo, telefono);
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Contacto agregado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerUsuarios();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al agregar ese contacto", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "Por favor rellene el formulario", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            borrarUsuario.Click += delegate
            {
                if (inputNombre.Text != "" || inputCorreo.Text != "" || inputTelefono.Text != "")
                {
                    // Si la tarea NO existe
                    if (!usuarios.Contains(inputNombre.Text + ":" + inputCorreo.Text + "-" + inputTelefono.Text))
                    {
                        Toast.MakeText(this, "Ese contacto NO existe", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Si la tarea existe, la podemos borrar
                        // Indice en la lista, es la fila en la tabla
                        int index = usuarios.IndexOf(contenidoPrevio);
                        // ID en la tabla, accediendo a la fila indicada
                        int id = Convert.ToInt32(tabla.Rows[index][0]);

                        int resultado;
                        resultado = cliente.BorrarUsuario(id);
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Contacto Eliminado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerUsuarios();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al eliminar ese contacto", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }

                }
                else
                {
                    Toast.MakeText(this, "Por favor rellene el formulario", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            modificarUsuario.Click += delegate
            {
                // Modificar trabaja con una variable global, solo se puede usar esta funcion si se selecciona un item!
                if (inputNombre.Text != "" || inputCorreo.Text != "" || inputTelefono.Text != "")
                {

                    if (!usuarios.Contains(contenidoPrevio))
                    {
                        Toast.MakeText(this, "Ese contacto NO existe", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Encontrar el indice de la tarea por modificar
                        // Indice en la lista, es la fila en la tabla. Usar a la tarea previa almacenada en alcance global como referencia.
                        int index = usuarios.IndexOf(contenidoPrevio);
                        // ID en la tabla, accediendo a la fila indicada
                        int id = Convert.ToInt32(tabla.Rows[index][0]);

                        int resultado;
                        // Modificar la tarea con el indice adecuado y pasar la descripcion nueva
                        resultado = cliente.ActualizarUsuario(id, inputNombre.Text, inputCorreo.Text, Convert.ToInt32(inputTelefono.Text));
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Contacto Actualizado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerUsuarios();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al eliminar ese contacto", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "Por favor rellene el formulario", ToastLength.Short).Show();
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
        private void ObtenerUsuarios()
        {
            // Limpiar el listado, asi se evita el duplicado
            usuarios.Clear();
            tabla = cliente.ObtenerUsuario();
            // Un listview puede ser rellenado con un adaptador: https://forums.xamarin.com/discussion/41387/fill-listview-from-dataset
            // Similar a rellenar un GridView en WinForms
            if (tabla.Rows.Count > 0)
            {
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    usuarios.Add(tabla.Rows[i][1].ToString() + ":" + tabla.Rows[i][2].ToString() + "-" + tabla.Rows[i][3].ToString());
                }
                ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, usuarios);
                listado.Adapter = adaptador;
            }
            else
            {
                Toast.MakeText(this, "Aun no tienes contactos registrados", ToastLength.Long).Show();
            }
        }
        private void LimpiarInput()
        {
            inputNombre.Text = "";
            inputCorreo.Text = "";
            inputTelefono.Text = "";
        }
        private void DeshabilitarBoton()
        {
            modificarUsuario.Enabled = false;
            borrarUsuario.Enabled = false;
        }
    }
}