using PruebaEmpresa.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

}
