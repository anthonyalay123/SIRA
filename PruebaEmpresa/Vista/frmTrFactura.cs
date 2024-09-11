using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PruebaEmpresa.Datos;
using PruebaEmpresa.Negocio;

namespace PruebaEmpresa
{
    public partial class frmTrFactura : Form
    {
        clsNFactura loLogicaNegocio = new clsNFactura();
        BindingSource bsDatos = new BindingSource();
        public frmTrFactura()
        {
            InitializeComponent();
            dgvDatos.CellValueChanged += dgvDatos_CellValueChanged;

        }

        private void frmTrFactura_Load(object sender, EventArgs e)
        {
            bsDatos.DataSource = new List<DetalleFactura>();
            dgvDatos.DataSource = bsDatos;
            txtNo.ReadOnly = true;
            txtBase0.ReadOnly = true;
            txtBase12.ReadOnly = true;
            txtIVA.ReadOnly = true;
            txtTotal.ReadOnly = true;

            dgvDatos.Columns[0].Visible = false; // Oculta IdDetalleFactura
            dgvDatos.Columns[1].Visible = false; // Oculta IdFactura
            dgvDatos.Columns[7].Visible = false; // Oculta Subtotal
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("¿Deseas guardar el registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Factura poObject = new Factura();

                    poObject.IdFactura = !string.IsNullOrEmpty(txtNo.Text) ? int.Parse(txtNo.Text) : 0;
                    poObject.Cliente = txtCliente.Text;
                    poObject.Fecha = dtpFecha.Value;
                    poObject.BaseImponibleIVA0 = decimal.Parse(txtBase0.Text);
                    poObject.BaseImponibleIVA12 = decimal.Parse(txtBase12.Text);
                    poObject.IVA = decimal.Parse(txtIVA.Text);
                    poObject.TotalAPagar = decimal.Parse(txtTotal.Text);

                    // Verificar que el DataSource sea una lista de FacturaDetalle
                    if (bsDatos.DataSource is List<DetalleFactura> listaDetalles)
                    {
                        poObject.DetalleFactura = listaDetalles;
                    }
                    else
                    {
                        return;
                    }

                    string psMsg = loLogicaNegocio.gsGuardar(poObject);

                    if (string.IsNullOrEmpty(psMsg))
                    {
                        MessageBox.Show("Factura guardada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                    }
                    else
                    {
                        MessageBox.Show(psMsg, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txtNo.Clear();
            txtCliente.Clear();
            dtpFecha.Value = DateTime.Now;
            txtBase0.Clear();
            txtBase12.Clear();
            txtIVA.Clear();
            txtTotal.Clear();
            dgvDatos.DataSource = new List<DetalleFactura>();
        }

        private void dgvDatos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica que la fila sea válida
            if (e.RowIndex >= 0)
            {
                var row = dgvDatos.Rows[e.RowIndex];

                // Usa índices en lugar de nombres de columna
                var cantidadCell = row.Cells[3]; // Índice de la columna Cantidad
                var precioUnitarioCell = row.Cells[4]; // Índice de la columna PrecioUnitario
                var ivaCell = row.Cells[5]; // Índice de la columna IVA
                var subtotalCell = row.Cells[6]; // Índice de la columna Subtotal

                if (cantidadCell.Value != null && precioUnitarioCell.Value != null && ivaCell.Value != null)
                {
                    decimal cantidad = Convert.ToDecimal(cantidadCell.Value);
                    decimal precioUnitario = Convert.ToDecimal(precioUnitarioCell.Value);
                    decimal iva = Convert.ToDecimal(ivaCell.Value);
                    decimal subtotal = cantidad * precioUnitario * (1 + iva);

                    subtotalCell.Value = subtotal;
                }
                ActualizarTotales();

            }
        }

        private void ActualizarTotales()
        {
            decimal baseImponibleIVA0 = 0;
            decimal baseImponibleIVA12 = 0;
            decimal ivaTotal = 0;
            decimal totalAPagar = 0;

            foreach (DataGridViewRow row in dgvDatos.Rows)
            {
                if (row.Cells[3].Value != null &&
                    row.Cells[4].Value != null &&
                    row.Cells[5].Value != null)
                {
                    decimal cantidad = Convert.ToDecimal(row.Cells[3].Value);
                    decimal precioUnitario = Convert.ToDecimal(row.Cells[4].Value);
                    decimal iva = Convert.ToDecimal(row.Cells[5].Value);
                    decimal subtotal = cantidad * precioUnitario;
                    decimal ivaCalculado = subtotal * iva;

                    // Sumar subtotal a base imponible según IVA
                    if (iva == 0)
                    {
                        baseImponibleIVA0 += subtotal;
                    }
                    else if (iva == 0.12m)
                    {
                        baseImponibleIVA12 += subtotal;
                    }

                    ivaTotal += ivaCalculado;
                    totalAPagar += subtotal + ivaCalculado;
                }
            }

            // Actualizar los campos
            txtBase0.Text = baseImponibleIVA0.ToString("F2");
            txtBase12.Text = baseImponibleIVA12.ToString("F2");
            txtIVA.Text = ivaTotal.ToString("F2");
            txtTotal.Text = totalAPagar.ToString("F2");
        }






    }
}
