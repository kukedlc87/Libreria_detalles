using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libreria_detalles.Capa_datos
{
    public class ConexionDao
    {
        private SqlConnection cnn; 
        private SqlCommand cmd;
        private DataTable dt;
        private SqlTransaction t;
        private static ConexionDao instancia;


        private ConexionDao () 
        { 
           cnn = new SqlConnection(Properties.Resources.SQlStr);
        
        }

        public static ConexionDao ObtenerInstancia()
        {
            if (instancia == null)
            { 
                instancia = new ConexionDao();
            }
            
            return instancia;
        
        }
        public  SqlConnection ObtenerConexion()
        {
            return cnn;

        }

        public void Conectar()
        {
            cnn.Open();
            cmd = new SqlCommand();
            cmd.Connection = cnn;

        }

        public void Desconectar()
        {
            cnn.Close();

        }

        public DataTable ReaderSP(string sp)
        {
            Conectar();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sp;
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            Desconectar();
            return dt;

        }

        public DataTable ReaderSP(string sp, List<SqlParameter> list)
        {
            Conectar();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sp;
            DataTable dt = new DataTable();
            foreach (SqlParameter p in list)
            {
                cmd.Parameters.Add(p);

            }
            dt.Load(cmd.ExecuteReader());
            Desconectar();
            return dt;

        }

        public int UltimoId()
        {

            Conectar();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ultimo_id";
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@ultimo";
            p.Direction = ParameterDirection.Output;
            p.SqlDbType = SqlDbType.Int;
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            Desconectar();
            return (int)p.Value;

        }

        public DataTable Reader(string sp)
        {
            Conectar();
            cmd.CommandText = sp;
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            Desconectar();
            return dt;

        }

       


       
        public bool Agregar(string sp, SqlParameter p)
        {
            bool resultado = false;
            t = null;
            try
            {
                t = cnn.BeginTransaction();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Connection = cnn;
                cmd.Transaction = t;
               
                
                    cmd.Parameters.Add(p);

                
                cmd.ExecuteNonQuery();
                t.Commit();
                resultado = true;

                Desconectar();

            }
            catch (SqlException)
            {
                if (t != null)
                { t.Rollback(); }

            }
            finally
            {
                Desconectar();
            }
            return resultado;

        }

        public void Agregar(string sp, SqlParameter p, SqlTransaction transa)
        {

                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Connection = cnn;
                cmd.Transaction = transa;
                cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
        }

        public void Agregar(string sp, List<SqlParameter> list, SqlTransaction transa)
        {
            cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sp;
            cmd.Connection = cnn;
            cmd.Transaction = transa;

            foreach (SqlParameter p in list)
            {
                cmd.Parameters.Add(p);
            
            }
            cmd.ExecuteNonQuery();
        }
    }
}
