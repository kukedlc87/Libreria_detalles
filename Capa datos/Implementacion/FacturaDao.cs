using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libreria_detalles.Capa_datos.Implementacion
{
    public class FacturaDao
    {

        public int UltimoId()
        {

          
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@ultimo";
            p.Direction = ParameterDirection.Output;
            p.SqlDbType = SqlDbType.Int;
            ConexionDao.ObtenerInstancia().Agregar("ultimo_id", p);

            return (int)p.Value;

        }

        public bool ConfirmarFactura(Factura nuevaFactura)
        {
            bool resultado = false;
            SqlTransaction t = null;
            SqlConnection cnn = ConexionDao.ObtenerInstancia().ObtenerConexion();

            try
            {
                cnn.Open();
                t = cnn.BeginTransaction();
                List<SqlParameter> listp = new List<SqlParameter>();
                SqlParameter parametro = new SqlParameter();
                parametro.ParameterName = "@cod_cliente";
                parametro.SqlDbType = SqlDbType.Int;
                parametro.Direction = ParameterDirection.Input;
                parametro.Value = nuevaFactura.Cod_cliente.ToString();
                listp.Add(parametro);
                SqlParameter parametro2 = new SqlParameter();
                parametro2.ParameterName = "@cod_vendedor";
                parametro2.SqlDbType = SqlDbType.Int;
                parametro2.Direction = ParameterDirection.Input;
                parametro2.Value = nuevaFactura.Cod_vendedor.ToString();
                listp.Add(parametro2);

                ConexionDao.ObtenerInstancia().Agregar("nueva_factura",listp, t);

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@ultimo";
                p.Direction = ParameterDirection.Output;
                p.SqlDbType = SqlDbType.Int;
                MessageBox.Show($"{p.ParameterName}");

                ConexionDao.ObtenerInstancia().Agregar("ultimo_id", p, t);
               
                int nro_factura = (int)p.Value;

                foreach (Detalle dt in nuevaFactura.Detalles)
                {
                    listp.Clear();
                    listp.Add(new SqlParameter("@nro_factura", nro_factura));
                    listp.Add(new SqlParameter("@cod_articulo", dt.Articulos.Id_articulo));
                    listp.Add(new SqlParameter("@pre_unitario", dt.Articulos.Pre_unitario));
                    listp.Add(new SqlParameter("@cantidad", dt.Cantidad));
                    ConexionDao.ObtenerInstancia().Agregar("insertar_detalle", listp, t);
                }

                t.Commit();

            }
            catch (SqlException)
            {
                if (t != null)
                {  t.Rollback();
                    resultado = false;
                    throw;
                }
                

            }
            finally
            {
                cnn.Close();

            }
            return resultado;

        }


    }
}
