using PruebaEmpresa.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;


namespace PruebaEmpresa.Negocio
{
    public class clsNFactura
    {
        public string gsGuardar(Factura toObject)
        {
            try
            {
                using (var context = new PCGERENTE_FACTURAEntities())
                {
                    // Verifica que no se está intentando actualizar una factura existente
                    if (toObject.IdFactura != 0)
                    {
                        return "No se puede guardar una factura existente con un IdFactura ya definido.";
                    }

                    // Añadir la nueva factura
                    context.Factura.Add(toObject);

                    // Añadir detalles de la factura
                    if (toObject.DetalleFactura != null)
                    {
                        foreach (var item in toObject.DetalleFactura)
                        {
                            item.IdFactura = toObject.IdFactura; // Asegúrate de que la FK esté configurada correctamente
                            context.DetalleFactura.Add(item);
                        }
                    }

                    // Guardar los cambios en la base de datos
                    context.SaveChanges();
                }

                return null; // Sin errores
            }
            catch (Exception ex)
            {
                return ex.Message; // Mensaje de error
            }
        }
        public string goBuscarCodigo(string tsTipo, string tsCodigo)
        {
            string psCodigo = string.Empty;

            // Si el valor de tsCodigo es null o vacío, tratarlo como 0
            int codigoActual;
            if (string.IsNullOrEmpty(tsCodigo) || !int.TryParse(tsCodigo, out codigoActual))
            {
                codigoActual = 0; // Si tsCodigo es inválido, lo tratamos como si fuera 0
            }

            using (var context = new PCGERENTE_FACTURAEntities())
            {
                if (tsTipo == "A")
                {
                    // Buscar facturas con un ID menor al actual
                    var poListaCodigo = context.Factura
                        .Where(x => x.IdFactura < codigoActual) // Consulta directa con Where
                        .OrderByDescending(x => x.IdFactura) // Ordenar en orden descendente
                        .FirstOrDefault(); // Tomar la primera factura que coincida

                    if (poListaCodigo != null)
                    {
                        psCodigo = poListaCodigo.IdFactura.ToString();
                    }
                    else
                    {
                        psCodigo = tsCodigo; // Si no se encuentra, devolver el valor original
                    }
                }
                else if (tsTipo == "S")
                {
                    // Buscar facturas con un ID mayor al actual
                    var poListaCodigo = context.Factura
                        .Where(x => x.IdFactura > codigoActual) // Consulta directa con Where
                        .OrderBy(x => x.IdFactura) // Ordenar en orden ascendente
                        .FirstOrDefault(); // Tomar la primera factura que coincida

                    if (poListaCodigo != null)
                    {
                        psCodigo = poListaCodigo.IdFactura.ToString();
                    }
                    else
                    {
                        psCodigo = tsCodigo; // Si no se encuentra, devolver el valor original
                    }
                }
            }

            return psCodigo;
        }



        public Factura goConsultarMovimiento(int tId)
        {
            using (var context = new PCGERENTE_FACTURAEntities())
            {
                // Obtener la factura con sus detalles asociados
                var factura = context.Factura
                                     .Include(x => x.DetalleFactura) // Incluir los detalles de la factura
                                     .Where(x => x.IdFactura == tId)
                                     .FirstOrDefault();
                return factura;
            }
        }

        public List<Factura> gsListar()
        {
            try
            {
                using (var context = new PCGERENTE_FACTURAEntities())
                {
                    // Recupera todas las facturas de la base de datos
                    var listaFacturas = context.Factura
                        .OrderBy(f => f.Fecha) // Ordenar por fecha (puedes cambiarlo a cualquier otro criterio)
                        .ToList();

                    return listaFacturas;
                }
            }
            catch (Exception ex)
            {
                // Manejar el error, en este caso, devolvemos una lista vacía si hay un error
                // Puedes lanzar una excepción o manejar el error de otra manera
                MessageBox.Show(ex.Message, "Error al listar facturas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Factura>();
            }
        }

    }

}
