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
    [Activity(Label = "activity_telefonos")]
    public class activity_telefonos : Activity
    {
        WebReference.ToDoWS cliente = new ToDoWS();
        DataTable tabla = new DataTable();

        List<string> telefonos = new List<string>();
        ListView listado;
        EditText input;
        EditText inputDescripcion;
        string telefonoPrevio;
        string descripcionPrevia;
        Button modificarTelefono;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_telefonos);
            listado = FindViewById<ListView>(Resource.Id.telefonos);

            input = FindViewById<EditText>(Resource.Id.txt_numero);
            inputDescripcion = FindViewById<EditText>(Resource.Id.txt_numeroDescripcion);

            Button agregarTelefono = FindViewById<Button>(Resource.Id.btn_agregarTelefono);
            Button borrarTelefono = FindViewById<Button>(Resource.Id.btn_borrarTelefono);
            modificarTelefono = FindViewById<Button>(Resource.Id.btn_actualizarTelefono);

            // Obtener el listado de telefonos
            ObtenerTelefonos();
            // Se puede acceder el item con el evente ItemClick: https://docs.microsoft.com/en-us/xamarin/android/internals/api-design#Events_and_Listeners
            listado.ItemClick += (sender, e) =>
            {
                // Obtener el indice del item seleccionado
                int index = e.Position;
                // Capturar el contenido usando el indice para obtener el item dentro de la lista original
                string contenido = telefonos[index].ToString();
                // Referencia: https://www.geeksforgeeks.org/c-sharp-substring-method/
                // La cadena viene unida desde la lista, por lo cual sus elementos deben ser separados independientemente
                // Definir el indice del caracter separador
                int separador = contenido.LastIndexOf("-");
                // Separar el numero, desde el inicio hasta antes del separador
                string numero = contenido.Substring(0, separador);
                // Separar la descripcion desde despues del separador en adelanto
                string descripcion = contenido.Substring(separador + 1 );
                // Asignar los valores en las cajas de texto
                input.Text = numero;
                inputDescripcion.Text = descripcion;
                // Guardar la tarea previa (por modificar) como referencia para poder encontrar su indice y ID.
                telefonoPrevio = contenido;
                modificarTelefono.Enabled = true;

            };
            agregarTelefono.Click += delegate {
                // Si la tarea ya existe
                if (input.Text != "" && inputDescripcion.Text != "")
                {
                    // Si los campos de telefono y descripcion tienen texto
                    if (input.Text.Length <= 8)
                    {

                        if (telefonos.Contains(input.Text + "-" + inputDescripcion))
                        {
                            Toast.MakeText(this, "Ese telefono ya existe", ToastLength.Short).Show();
                        }
                        else
                        {
                            // Si la tarea aun no existe
                            int resultado;
                            resultado = cliente.CrearTelefono(Convert.ToInt32(input.Text), inputDescripcion.Text);
                            if (resultado > 0)
                            {
                                Toast.MakeText(this, "Telefono agregado", ToastLength.Short).Show();
                                // Actualizar el listado al mas reciente
                                ObtenerTelefonos();
                                LimpiarInput();
                                DeshabilitarBoton();
                            }
                            else
                            {
                                Toast.MakeText(this, "Se genero un error al agregar ese telefono", ToastLength.Short).Show();
                                LimpiarInput();
                                DeshabilitarBoton();
                            }
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "El telefono debe ser de 8 caracteres", ToastLength.Short).Show();
                        LimpiarInput();
                        DeshabilitarBoton();
                    }
                }
                else {
                    Toast.MakeText(this, "Por favor escriba un numero y una descripccion", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            borrarTelefono.Click += delegate
            {
                if (input.Text != "" && inputDescripcion.Text != "")
                {
                    // Si la tarea NO existe
                    if (!telefonos.Contains(input.Text + "-" + inputDescripcion.Text))
                    {
                        Toast.MakeText(this, "Ese telefono No existe", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Si la tarea existe, la podemos borrar
                        // Indice en la lista, es la fila en la tabla
                        int index = telefonos.IndexOf(input.Text + "-" + inputDescripcion.Text);
                        // ID en la tabla, accediendo a la fila indicada
                        int id = Convert.ToInt32(tabla.Rows[index][0]);

                        int resultado;
                        resultado = cliente.BorrarTelefono(id);
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Telefono Eliminado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerTelefonos();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al eliminar el telefono", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }

                }
                else {
                    Toast.MakeText(this, "Por favor escriba un numero y una descripcion", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            modificarTelefono.Click += delegate {
                // Modificar trabaja con una variable global, solo se puede usar esta funcion si se selecciona un item!
                if (input.Text != "" && inputDescripcion.Text != "")
                {

                    if (!telefonos.Contains(telefonoPrevio))
                    {
                        Toast.MakeText(this, "Esa tarea No existe", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Encontrar el indice de la tarea por modificar
                        // Indice en la lista, es la fila en la tabla. Usar a la tarea previa almacenada en alcance global como referencia.
                        int index = telefonos.IndexOf(telefonoPrevio);
                        // ID en la tabla, accediendo a la fila indicada
                        int id = Convert.ToInt32(tabla.Rows[index][0]);

                        int resultado;
                        // Modificar la tarea con el indice adecuado y pasar la descripcion nueva
                        resultado = cliente.ActualizarTelefono(id, Convert.ToInt32(input.Text), inputDescripcion.Text);
                        if (resultado > 0)
                        {
                            Toast.MakeText(this, "Telefono Actualizado", ToastLength.Short).Show();
                            // Actualizar el listado al mas reciente
                            ObtenerTelefonos();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                        else
                        {
                            Toast.MakeText(this, "Se genero un error al eliminar el telefono", ToastLength.Short).Show();
                            LimpiarInput();
                            DeshabilitarBoton();
                        }
                    }
                }
                else {
                    Toast.MakeText(this, "Por favor escriba un numero y una descripcion", ToastLength.Short).Show();
                    LimpiarInput();
                    DeshabilitarBoton();
                }
            };
            Button btnVolver = FindViewById<Button>(Resource.Id.btn_telefonosVolver);
            btnVolver.Click += delegate
            {
                Intent menu = new Intent(this, typeof(MainActivity));
                StartActivity(menu);
            };

        }
        private void ObtenerTelefonos()
        {
            // Limpiar el listado, asi se evita el duplicado
            telefonos.Clear();
            tabla = cliente.ObtenerTelefonos();
            // Un listview puede ser rellenado con un adaptador: https://forums.xamarin.com/discussion/41387/fill-listview-from-dataset
            // Similar a rellenar un GridView en WinForms
            if (tabla.Rows.Count > 0)
            {
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    telefonos.Add(tabla.Rows[i][1].ToString() +"-"+ tabla.Rows[i][2].ToString());
                }
                ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, telefonos);
                listado.Adapter = adaptador;
            }
            else
            {
                Toast.MakeText(this, "Aun no tienes telefonos registrados", ToastLength.Long).Show();
            }
        }
        private void LimpiarInput()
        {
            input.Text = "";
            inputDescripcion.Text = "";
        }
        private void DeshabilitarBoton()
        {
            modificarTelefono.Enabled = false;
        }
    }

}