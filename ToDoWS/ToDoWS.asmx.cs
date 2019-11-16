using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace ToDoWS
{
    /// <summary>
    /// WebService CRUD para la aplicacion de To-Do List
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ToDoWS : System.Web.Services.WebService
    {
        // Conexion al servidor donde se almacena la base de datos: http://freeasphosting.net
        // Usuario: todolistappprgv
        // Contraseña:QtKZ1D$w64Uy
        // En un string no se puede usar un solo '\' ya que es usado para expresiones como \n, sino que se deben usar dos '\\' para indicar su uso: https://stackoverflow.com/questions/18532691/how-do-i-write-a-backslash-in-a-string
        static public string connectionString = "Server=sql.freeasphost.net\\MSSQL2016;Database=todolistappprgv_ToDoList;uid=todolistappprgv;pwd=admin;";
        static SqlConnection conexion = new SqlConnection(connectionString);

        public DataTable EjecutarConsulta(StringBuilder query, SqlCommand comando = null)
        {
            DataTable tabla = new DataTable();
            try
            {
                // Correguir error de retorno en web service
                tabla.TableName = "Consulta";
                conexion.Open();
                if (comando == null)
                {
                    comando = new SqlCommand();
                }
                // Configurar comando de SQL
                comando.Connection = conexion;
                comando.CommandType = CommandType.Text;
                comando.CommandText = query.ToString();
                // Data Reader es mas veloz que DataSet y mejor para consultas pequeñas: https://www.techrepublic.com/article/which-is-best-for-you-datareader-or-dataset/
                SqlDataReader lector = comando.ExecuteReader();
                tabla.Load(lector);
                return tabla;
            }
            catch (Exception e)
            {
                throw new Exception("Error al consultar.");
            }
            finally
            {
                // Usando finally no se requiere cerrar conexion en ambos bloques
                conexion.Close();
            }
        }
        public int EjecutarSentencia(StringBuilder query, SqlCommand comando = null)
        {
            int resultado = 0;
            try
            {
                conexion.Open();
                // Configurar comando de SQL
                comando.Connection = conexion;
                comando.CommandType = CommandType.Text;
                comando.CommandText = query.ToString();

                resultado = comando.ExecuteNonQuery();
                return resultado;
            }
            catch (Exception)
            {

                throw new Exception("Error al ejecutar una sentencia.");
            }
            finally
            {
                // Usando finally no se requiere cerrar conexion en ambos bloques
                conexion.Close();
            }
        }
        // Create
        [WebMethod]
        public int CrearTelefono(int telefono, string descripcion)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("INSERT INTO Telefono VALUES(@telefono, @descripcion)");
            // Parametros
            comando.Parameters.Add("@telefono", SqlDbType.Int).Value = telefono;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;
            
            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        [WebMethod]
        public int CrearCorreo(string correo, string descripcion)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("INSERT INTO Correo VALUES(@correo, @descripcion)");
            // Parametros
            comando.Parameters.Add("@correo", SqlDbType.NVarChar).Value = correo;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        [WebMethod]
        public int CrearTarea(string descripcion)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("INSERT INTO Tarea VALUES(@descripcion)");
            // Parametros
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        // Obtener
        [WebMethod]
        public DataTable ObtenerTelefonos()
        {
            // Obtener todos los contactos
            DataTable tabla = new DataTable();
            SqlCommand comando = new SqlCommand();
            // Usar string builder por cuestiones de rendimiento
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("SELECT * FROM Telefono");

            tabla = EjecutarConsulta(query, comando);
            return tabla;
        }
        [WebMethod]
        public DataTable ObtenerCorreos()
        {
            // Obtener todos los correos
            DataTable tabla = new DataTable();
            SqlCommand comando = new SqlCommand();
            // Usar string builder por cuestiones de rendimiento
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("SELECT * FROM Correo");

            tabla = EjecutarConsulta(query, comando);
            return tabla;
        }
        [WebMethod]
        public DataTable ObtenerTareas()
        {
            // Obtener todas las tareas
            DataTable tabla = new DataTable();
            SqlCommand comando = new SqlCommand();
            // Usar string builder por cuestiones de rendimiento
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("SELECT * FROM Tarea");

            tabla = EjecutarConsulta(query, comando);
            return tabla;
        }
        

        // Actualizar
        [WebMethod]
        public int ActualizarTelefono(int id_telefono,int telefono, string descripcion)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("UPDATE Telefono SET Telefono = @telefono, Descripcion = @descripcion WHERE ID_Telefono = @id_telefono");
            // Parametros
            comando.Parameters.Add("@id_telefono", SqlDbType.Int).Value = id_telefono;
            comando.Parameters.Add("@telefono", SqlDbType.Int).Value = telefono;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        [WebMethod]
        public int ActualizarCorreo(int id_correo, string correo, string descripcion)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("UPDATE Correo SET Correo = @correo, Descripcion = @descripcion WHERE ID_Correo = @id_correo");
            // Parametros
            comando.Parameters.Add("@id_correo", SqlDbType.Int).Value = id_correo;
            comando.Parameters.Add("@correo", SqlDbType.NVarChar).Value = correo;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        [WebMethod]
        public int ActualizarTarea(int id_tarea, string tarea)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("UPDATE Tarea SET Tarea = @tarea WHERE ID_Tarea = @id_tarea");
            // Parametros
            comando.Parameters.Add("@id_tarea", SqlDbType.Int).Value = id_tarea;
            comando.Parameters.Add("@tarea", SqlDbType.NVarChar).Value = tarea;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        // Borrar
        [WebMethod]
        public int BorrarTelefono(int id_telefono)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("DELETE FROM Telefono WHERE ID_Telefono = @id_telefono");
            // Parametros
            comando.Parameters.Add("@id_telefono", SqlDbType.Int).Value = id_telefono;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        [WebMethod]
        public int BorrarCorreo(int id_correo)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("DELETE FROM Correo WHERE ID_Correo = @id_correo");
            // Parametros
            comando.Parameters.Add("@id_correo", SqlDbType.Int).Value = id_correo;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        [WebMethod]
        public int BorrarTarea(int id_tarea)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("DELETE FROM Tarea WHERE ID_Tarea = @id_tarea");
            // Parametros
            comando.Parameters.Add("@id_tarea", SqlDbType.Int).Value = id_tarea;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
    }
}
