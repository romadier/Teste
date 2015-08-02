using Imposto.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Imposto.Core.Service
{
    public class NotaFiscalService
    {
        public NotaFiscal GerarNotaFiscal(Domain.Pedido pedido)
        {
            NotaFiscal notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            return notaFiscal;
        }

        public void GravarXML(NotaFiscal notaFiscal, String diretorio)
        {
            XmlSerializer xs = new XmlSerializer(typeof(NotaFiscal));
            String arquivo =  diretorio + "nf_" + notaFiscal.NumeroNotaFiscal + "_" + notaFiscal.Serie + ".xml";
            TextWriter tw = new StreamWriter(@arquivo);
            xs.Serialize(tw, notaFiscal);
        }

        public void PersistirNota(NotaFiscal notaFiscal, String conexao)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = conexao;
            conn.Open();

            SqlCommand cmd = new SqlCommand("dbo.P_NOTA_FISCAL",conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
            parId.Direction = ParameterDirection.InputOutput;
            parId.Value = 0;
            cmd.Parameters.Add(parId);
            cmd.Parameters.Add("pSerie", SqlDbType.Int).Value = notaFiscal.Serie;
            cmd.Parameters.Add("pNumeroNotaFiscal", SqlDbType.Int).Value = notaFiscal.NumeroNotaFiscal;
            cmd.Parameters.Add("pNomeCliente", SqlDbType.VarChar).Value = notaFiscal.NomeCliente;
            cmd.Parameters.Add("pEstadoDestino", SqlDbType.VarChar).Value = notaFiscal.EstadoDestino;
            cmd.Parameters.Add("pEstadoOrigem", SqlDbType.VarChar).Value = notaFiscal.EstadoOrigem;
            cmd.ExecuteNonQuery();

            
            foreach (NotaFiscalItem item in notaFiscal.ItensDaNotaFiscal)
            {
                SqlCommand cmdItem = new SqlCommand("dbo.P_NOTA_FISCAL_ITEM", conn);
                cmdItem.CommandType = CommandType.StoredProcedure;
                cmdItem.Parameters.Add("pIdNotaFiscal", SqlDbType.Int).Value = parId.Value;
                cmdItem.Parameters.Add("pId", SqlDbType.Int).Value = 0;
                cmdItem.Parameters.Add("pCfop", SqlDbType.VarChar).Value = item.Cfop;
                cmdItem.Parameters.Add("pTipoIcms", SqlDbType.VarChar).Value = item.TipoIcms;
                cmdItem.Parameters.Add("pBaseIcms", SqlDbType.Decimal).Value = item.BaseIcms;
                cmdItem.Parameters.Add("pAliquotaIcms", SqlDbType.Decimal).Value = item.AliquotaIcms;
                cmdItem.Parameters.Add("pValorIcms", SqlDbType.Decimal).Value = item.ValorIcms;
                cmdItem.Parameters.Add("pBaseIpi", SqlDbType.Decimal).Value = item.BaseIpi;
                cmdItem.Parameters.Add("pAliquotaIpi", SqlDbType.Decimal).Value = item.AliquotaIpi;
                cmdItem.Parameters.Add("pValorIpi", SqlDbType.Decimal).Value = item.ValorIpi;
                cmdItem.Parameters.Add("pDesconto", SqlDbType.Decimal).Value = item.Desconto;
                cmdItem.Parameters.Add("pNomeProduto", SqlDbType.VarChar).Value = item.NomeProduto;
                cmdItem.Parameters.Add("pCodigoProduto", SqlDbType.VarChar).Value = item.CodigoProduto;
                cmdItem.ExecuteNonQuery();
            }

        }
    }
}
