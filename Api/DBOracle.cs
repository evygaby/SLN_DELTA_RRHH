
using Newtonsoft.Json.Converters;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace Api
{
    public class DBOracle : IDisposable
    {

        // Variables.
        private OracleConnection ora_Connection;
        private OracleTransaction ora_Transaction;
        public OracleDataReader ora_DataReader;

        private struct stConnDB
        {
            public string CadenaConexion;
            public string ErrorDesc;
            public int ErrorNum;
        }
        private stConnDB info;

        // Indica el numero de intentos de conectar a la BD sin exito.
        public byte ora_intentos = 0;

        #region "Propiedades"

        /// <summary>
        /// Devuelve la descripcion de error de la clase.
        /// </summary>
        public string ErrDesc
        {
            get { return this.info.ErrorDesc; }
        }

        /// <summary>
        /// Devuelve el numero de error de la clase.
        /// </summary>
        public string ErrNum
        {
            get { return info.ErrorNum.ToString(); }
        }

        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public DBOracle()
        {
            // Creamos la cadena de conexión de la base de datos.
            info.CadenaConexion = ClsConfig.cadenaoracle; // string.Format("Data Source={0};User Id={1};Password={2};", Servidor, Usuario, Password);

            // Instanciamos objeto conecction.
            ora_Connection = new OracleConnection();

        }

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose de la clase.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberamos objetos manejados.
            }

            try
            {
                // Liberamos los obtetos no manejados.
                if (ora_DataReader != null)
                {
                    ora_DataReader.Close();
                    ora_DataReader.Dispose();
                }

                // Cerramos la conexión a DB.
                if (!Desconectar())
                {
                    // Grabamos Log de Error...
                }

            }
            catch (Exception ex)
            {
                // Asignamos error.
                AsignarError(ref ex);
            }

        }


        /// <summary>
        /// Destructor.
        /// </summary>
        ~DBOracle()
        {
            Dispose(false);
        }


        /// <summary>
        /// Se conecta a una base de datos de Oracle.
        /// </summary>
        /// <returns>True si se conecta bien.</returns>
        private bool Conectar()
        {

            bool ok = false;

            try
            {
                if (ora_Connection != null)
                {
                    // Fijamos la cadena de conexión de la base de datos.
                    ora_Connection.ConnectionString = info.CadenaConexion;
                    ora_Connection.Open();
                    ok = true;
                }
            }
            catch (Exception ex)
            {
                // Desconectamos y liberamos memoria.
                Desconectar();
                // Asignamos error.
                AsignarError(ref ex);
                // Asignamos error de función
                ok = false;
            }

            return ok;

        }


        /// <summary>
        /// Cierra la conexión de BBDD.
        /// </summary>
        public bool Desconectar()
        {
            try
            {
                // Cerramos la conexion
                if (ora_Connection != null)
                {
                    if (ora_Connection.State != ConnectionState.Closed)
                    {
                        ora_Connection.Close();
                    }
                }
                // Liberamos su memoria.
                ora_Connection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                return false;
            }
        }

        public string crearcadena(string datasource, string usuario, string pass)
        {
            string cadena = null;
            cadena = "DATA SOURCE=" + datasource + ";PASSWORD=" + pass + ";PERSIST SECURITY INFO=True;USER ID=" + usuario + ";pooling=false;";
            info.CadenaConexion = cadena;
            return cadena;
        }
        /// <summary>
        /// Ejecuta un procedimiento almacenado de Oracle.
        /// </summary>
        /// <param name="oraCommand">Objeto Command con los datos del procedimiento.</param>
        /// <param name="SpName">Nombre del procedimiento almacenado.</param>
        /// <returns>True si el procedimiento se ejecuto bien.</returns>
        public bool EjecutaSP(ref OracleCommand OraCommand, string SpName)
        {

            bool ok = true;

            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }

                if (ok)
                {
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }
        public bool EjecutaSPR(string SpName, string? tabla, string usu, string pass)
        {
            bool ok = true;
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    crearcadena(ClsConfig.DATA_SOURCE, usu, pass);
                    ok = Conectar();
                }
                if (ok)
                {
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }
                    OracleCommand OraCommand = new OracleCommand();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.Parameters.Add("nom_tabla", tabla);
                    OraCommand.Parameters.Add("LISTA", OracleDbType.RefCursor);
                    OraCommand.Parameters["LISTA"].Direction = ParameterDirection.Output;
                    // OraCommand.ExecuteNonQuery();
                    ora_DataReader = OraCommand.ExecuteReader();
                    //      var kiekis = Convert.ToString(OraCommand.Parameters["LISTA"].Value);
                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }
        public bool EjecutaSimple(string SpName, int? idempresa, string usu, string pass)
        {
            bool ok = true;
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    crearcadena(ClsConfig.DATA_SOURCE, usu, pass);
                    ok = Conectar();
                }
                if (ok)
                {
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }
                    OracleCommand OraCommand = new OracleCommand();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.Parameters.Add("PN_EMPRESA", idempresa);
                    OraCommand.Parameters.Add("LISTA", OracleDbType.RefCursor);
                    OraCommand.Parameters["LISTA"].Direction = ParameterDirection.Output;
                    // OraCommand.ExecuteNonQuery();
                    ora_DataReader = OraCommand.ExecuteReader();
                    //      var kiekis = Convert.ToString(OraCommand.Parameters["LISTA"].Value);
                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }

        public bool Ejecuta(string SpName, string? sentencia, string usu, string pass)
        {
            bool ok = true;
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    crearcadena(ClsConfig.DATA_SOURCE, usu, pass);
                    ok = Conectar();
                }
                if (ok)
                {
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }
                    OracleCommand OraCommand = new OracleCommand();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.Parameters.Add("sentencia", OracleDbType.Clob).Value = sentencia;
                    OraCommand.Parameters.Add("LISTA", OracleDbType.RefCursor);
                    OraCommand.Parameters["LISTA"].Direction = ParameterDirection.Output;

                    ora_DataReader = OraCommand.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }
        public bool AfectarTabla(string SpName, string? sentencia, string usu, string pass)
        {
            bool ok = true;
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    crearcadena(ClsConfig.DATA_SOURCE, usu, pass);
                    ok = Conectar();
                }
                if (ok)
                {
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }
                    OracleCommand OraCommand = new OracleCommand();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.Parameters.Add("sentencia", sentencia);
                    int res = OraCommand.ExecuteNonQuery();
                    ok = true;

                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }
        public bool login(string SpName, string? usu, string? pass)
        {
            bool ok = true;
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    info.CadenaConexion = crearcadena(ClsConfig.DATA_SOURCE, usu, pass);
                    ok = Conectar();
                }
                if (ok)
                {
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }
                    OracleCommand OraCommand = new OracleCommand();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.Parameters.Add("pv_usuario", usu.ToUpper());
                    OraCommand.Parameters.Add("pv_pass", pass);

                    OraCommand.Parameters.Add("existe", OracleDbType.Int32);
                    //OraCommand.Parameters["existe"].Size = 100;
                    OraCommand.Parameters["existe"].Direction = ParameterDirection.Output;
                    OraCommand.Parameters.Add("lgerror", OracleDbType.Varchar2);
                    OraCommand.Parameters["lgerror"].Size = 100;
                    OraCommand.Parameters["lgerror"].Direction = ParameterDirection.Output;
                    ora_DataReader = OraCommand.ExecuteReader();
                    var kiekis = Convert.ToString(OraCommand.Parameters["existe"].Value);
                    var lgerror = Convert.ToString(OraCommand.Parameters["lgerror"].Value);

                    if (kiekis == "0")
                    {

                        ok = false;
                        throw new Exception(lgerror);
                    }



                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }
        public bool MenuPerfilUsuario(int codEmpleado, string usu, string pass)
        {
            bool ok = true;
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    crearcadena(ClsConfig.DATA_SOURCE, usu, pass);
                    ok = Conectar();
                }
                if (ok)
                {
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }
                    OracleCommand OraCommand = new OracleCommand();
                    OraCommand.Connection = ora_Connection;
                    OraCommand.CommandText = "rh_mantenimientos.QRY_MenuPerfilUsuario";
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.Parameters.Add("Pv_CodUsuario", usu);
                    OraCommand.Parameters.Add("Pv_CodEmpleado", codEmpleado);
                    OraCommand.Parameters.Add("menuUsuario", OracleDbType.RefCursor);
                    OraCommand.Parameters["menuUsuario"].Direction = ParameterDirection.Output;
                    // OraCommand.ExecuteNonQuery();
                    ora_DataReader = OraCommand.ExecuteReader();
                    //      var kiekis = Convert.ToString(OraCommand.Parameters["LISTA"].Value);
                }
            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }


        /// <summary>
        /// Ejecuta una sql que rellenar un DataReader (sentencia select).
        /// </summary>
        /// <param name="SqlQuery">sentencia sql a ejecutar</param>
        /// <returns></returns> 
        public bool EjecutaSQL(string SqlQuery, string usu, string pass)
        {

            bool ok = true;

            OracleCommand ora_Command = new OracleCommand();

            try
            {

                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    info.CadenaConexion = crearcadena(ClsConfig.DATA_SOURCE, usu, pass);
                    ok = Conectar();
                }

                if (ok)
                {
                    // Cerramos cursores abiertos, para evitar el error ORA-1000
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }

                    ora_Command.Connection = ora_Connection;
                    ora_Command.CommandType = CommandType.Text;
                    ora_Command.CommandText = SqlQuery;

                    // Ejecutamos sql.
                    ora_DataReader = ora_Command.ExecuteReader();
                }

            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }
            finally
            {
                if (ora_Command != null)
                {
                    ora_Command.Dispose();
                }
            }

            return ok;

        }


        public bool EjecutaSQL(string SqlQuery)
        {

            bool ok = true;

            OracleCommand ora_Command = new OracleCommand();

            try
            {

                // Si no esta conectado, se conecta.
                if (!IsConected())
                {

                    ok = Conectar();
                }

                if (ok)
                {
                    // Cerramos cursores abiertos, para evitar el error ORA-1000
                    if ((ora_DataReader != null))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }

                    ora_Command.Connection = ora_Connection;
                    ora_Command.CommandType = CommandType.Text;
                    ora_Command.CommandText = SqlQuery;

                    // Ejecutamos sql.
                    ora_DataReader = ora_Command.ExecuteReader();
                }

            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }
            finally
            {
                if (ora_Command != null)
                {
                    ora_Command.Dispose();
                }
            }

            return ok;

        }



        /// <summary>
        /// Ejecuta una sql que no devuelve datos (update, delete, insert).
        /// </summary>
        /// <param name="SqlQuery">sentencia sql a ejecutar</param>
        /// <param name="FilasAfectadas">Fila afectadas por la sentencia SQL</param>
        /// <returns></returns>
        public bool EjecutaSQL(string SqlQuery, ref int FilasAfectadas)
        {

            bool ok = true;
            OracleCommand ora_Command = new OracleCommand();

            try
            {

                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }

                if (ok)
                {
                    ora_Transaction = ora_Connection.BeginTransaction();
                    ora_Command = ora_Connection.CreateCommand();
                    ora_Command.CommandType = CommandType.Text;
                    ora_Command.CommandText = SqlQuery;
                    FilasAfectadas = ora_Command.ExecuteNonQuery();
                    ora_Transaction.Commit();
                }

            }
            catch (Exception ex)
            {
                // Hacemos rollback.
                ora_Transaction.Rollback();
                AsignarError(ref ex);
                ok = false;
            }
            finally
            {
                // Recolectamos objetos para liberar su memoria.
                if (ora_Command != null)
                {
                    ora_Command.Dispose();
                }
            }

            return ok;

        }


        /// <summary>
        /// Captura Excepciones
        /// </summary>
        /// <param name="ex">Excepcion producida.</param>
        private void AsignarError(ref Exception ex)
        {
            // Si es una excepcion de Oracle.
            if (ex is OracleException)
            {
                info.ErrorNum = ((OracleException)ex).Number;
                info.ErrorDesc = ex.Message;
            }
            else
            {
                info.ErrorNum = 0;
                info.ErrorDesc = ex.Message;
            }
            // Grabamos Log de Error...
        }



        /// <summary>
        /// Devuelve el estado de la base de datos
        /// </summary>
        /// <returns>True si esta conectada.</returns>
        public bool IsConected()
        {

            bool ok = false;

            try
            {
                // Si el objeto conexion ha sido instanciado
                if (ora_Connection != null)
                {
                    // Segun el estado de la Base de Datos.
                    switch (ora_Connection.State)
                    {
                        case ConnectionState.Closed:
                        case ConnectionState.Broken:
                        case ConnectionState.Connecting:
                            ok = false;
                            break;
                        case ConnectionState.Open:
                        case ConnectionState.Fetching:
                        case ConnectionState.Executing:
                            ok = true;
                            break;
                    }
                }
                else
                {
                    ok = false;
                }

            }
            catch (Exception ex)
            {
                AsignarError(ref ex);
                ok = false;
            }

            return ok;

        }

    }
}
