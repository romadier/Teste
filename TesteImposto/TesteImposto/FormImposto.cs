using Imposto.Core.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imposto.Core.Domain;

namespace TesteImposto
{
    public partial class FormImposto : Form
    {
        private Pedido pedido = new Pedido();

        public FormImposto()
        {
            InitializeComponent();
            dataGridViewPedidos.AutoGenerateColumns = true;                       
            dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();
        }

        private void ResizeColumns()
        {
            double mediaWidth = dataGridViewPedidos.Width / dataGridViewPedidos.Columns.GetColumnCount(DataGridViewElementStates.Visible);

            for (int i = dataGridViewPedidos.Columns.Count - 1; i >= 0; i--)
            {
                var coluna = dataGridViewPedidos.Columns[i];
                coluna.Width = Convert.ToInt32(mediaWidth);
            }   
        }

        private object GetTablePedidos()
        {
            DataTable table = new DataTable("pedidos");
            table.Columns.Add(new DataColumn("Nome do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Codigo do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Valor", typeof(decimal)));
            table.Columns.Add(new DataColumn("Brinde", typeof(bool)));
                     
            return table;
        }

        private void buttonGerarNotaFiscal_Click(object sender, EventArgs e)
        {   
         
            NotaFiscalService service = new NotaFiscalService();
            pedido.EstadoOrigem = txtEstadoOrigem.Text;
            pedido.EstadoDestino = txtEstadoDestino.Text;
            pedido.NomeCliente = textBoxNomeCliente.Text;

            DataTable table = (DataTable)dataGridViewPedidos.DataSource;

            foreach (DataRow row in table.Rows)
            {
                pedido.ItensDoPedido.Add(
                    new PedidoItem()
                    {
                        Brinde =  row["Brinde"] is DBNull ? false : Convert.ToBoolean(row["Brinde"] )  ,
                        CodigoProduto =  row["Codigo do produto"].ToString(),
                        NomeProduto = row["Nome do produto"].ToString(),
                        ValorItemPedido = Convert.ToDouble(row["Valor"].ToString())            
                    });
            }

            NotaFiscal notafiscal = service.GerarNotaFiscal(pedido);
            service.GravarXML(notafiscal, System.Configuration.ConfigurationSettings.AppSettings["DiretorioXMLs"]);
            service.PersistirNota(notafiscal, System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
            MessageBox.Show("Operação efetuada com sucesso");
            txtEstadoDestino.Clear();
            txtEstadoOrigem.Clear();
            textBoxNomeCliente.Clear();
            table.Rows.Clear();
            textBoxNomeCliente.Focus();
        }
        private bool UFValida(string uf)
        {
            string[] ufs = new string[] { "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PR", "PB", "PA", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SE", "SP", "TO" };

            return (Array.IndexOf(ufs, uf) > -1);
        }

        private void txtEstadoOrigem_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = ! UFValida((sender as TextBox).Text);
            if (e.Cancel)
                MessageBox.Show("Informe um cep válido!");
        }

        private void txtEstadoDestino_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !UFValida((sender as TextBox).Text);
            if (e.Cancel)
                MessageBox.Show("Informe um cep válido!");

        }

       
    }
}
