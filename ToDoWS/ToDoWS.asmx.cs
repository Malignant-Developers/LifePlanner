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
        // En un string no se puede usar un solo '\' ya que es usado para expresiones como \n, sino que se deben usar dos '\\' para indicar su uso: https://stackoverflow.com/questions/18532691/how-do-i-write-a-backslash-in-a-string
        static public string connectionString = "workstation id=LifePlanner.mssql.somee.com;packet size=4096;user id=TodoAppWS_SQLLogin_1;pwd=2agolwe6t3;data source=LifePlanner.mssql.somee.com;persist security info=False;initial catalog=LifePlanner";
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
        public int CrearUsario(string nombre, string correo, int telefono)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("INSERT INTO usuario VALUES(@nombre, @correo, @telefono)");
            // Parametros
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = nombre;
            comando.Parameters.Add("@correo", SqlDbType.NVarChar).Value = correo;
            comando.Parameters.Add("@telefono", SqlDbType.Int).Value = telefono;
            
            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
        // Obtener
        [WebMethod]
        public DataTable ObtenerUsuario()
        {
            // Obtener todos los contactos
            DataTable tabla = new DataTable();
            SqlCommand comando = new SqlCommand();
            // Usar string builder por cuestiones de rendimiento
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("SELECT * FROM Usuario");

            tabla = EjecutarConsulta(query, comando);
            return tabla;
        }

        // Actualizar
        [WebMethod]
        public int ActualizarUsuario(int id, string nombre, string correo, int telefono)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("UPDATE Usuario SET Nombre = @nombre, Correo = @correo, Telefono = @telefono WHERE Id = @id");
            // Parametros
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = nombre;
            comando.Parameters.Add("@correo", SqlDbType.NVarChar).Value = correo;
            comando.Parameters.Add("@telefono", SqlDbType.Int).Value = telefono;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
      
        // Borrar
        [WebMethod]
        public int BorrarUsuario(int id)
        {
            int resultado;
            SqlCommand comando = new SqlCommand();
            StringBuilder query = new StringBuilder();
            // Comando
            query.Append("DELETE FROM Usuario WHERE Id = @id");
            // Parametros
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

            resultado = EjecutarSentencia(query, comando);
            return resultado;
        }
    }
}
