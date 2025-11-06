using System.Data;
using System.Data.OleDb;
using System.Globalization;
namespace Api
{
    public class DBAccess : IDisposable
    {
        private readonly OleDbConnection _connection;
        private OleDbTransaction? _transaction;
        private string? _errorDescription;
        private int _errorNumber;

        public OleDbDataReader? DataReader { get; private set; }

        public string? ErrDesc => _errorDescription;

        public string ErrNum => _errorNumber.ToString(CultureInfo.InvariantCulture);

        public DBAccess(string? connectionString = null)
        {
            var resolvedConnectionString = connectionString ?? ClsConfig.cadenaaccess;

            if (string.IsNullOrWhiteSpace(resolvedConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexión de Access no ha sido configurada.");
            }

            _connection = new OleDbConnection(resolvedConnectionString);
        }

        public static string CrearCadena(string databasePath, string? password = null, string? provider = null)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new ArgumentException("El path de la base de datos de Access es requerido.", nameof(databasePath));
            }

            var builder = new OleDbConnectionStringBuilder
            {
                Provider = string.IsNullOrWhiteSpace(provider) ? "Microsoft.ACE.OLEDB.12.0" : provider,
                DataSource = databasePath
            };

            builder["Persist Security Info"] = false;

            if (!string.IsNullOrWhiteSpace(password))
            {
                builder["Jet OLEDB:Database Password"] = password;
            }

            return builder.ConnectionString;
        }

        public bool Conectar()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return true;
            }
            catch (Exception ex)
            {
                AsignarError(ex);
                return false;
            }
        }

        public bool Desconectar()
        {
            try
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                    DataReader.Dispose();
                    DataReader = null;
                }

                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                AsignarError(ex);
                return false;
            }
        }

        public OleDbCommand CrearComando(string commandText, CommandType commandType = CommandType.Text)
        {
            var command = _connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (_transaction != null)
            {
                command.Transaction = _transaction;
            }

            return command;
        }

        public OleDbDataAdapter CrearDataAdapter(OleDbCommand command)
        {
            command.Connection = _connection;
            return new OleDbDataAdapter(command);
        }

        public void BeginTransaction()
        {
            if (_connection.State != ConnectionState.Open)
            {
                Conectar();
            }

            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction = null;
        }

        private void AsignarError(Exception ex)
        {
            _errorDescription = ex.Message;
            _errorNumber = ex.HResult;
        }

        public void Dispose()
        {
            Desconectar();
            _transaction?.Dispose();
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
