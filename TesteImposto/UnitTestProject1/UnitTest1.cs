using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Imposto.Core.Domain;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestValorIpi()
        {
            Pedido pedido = new Pedido();
            pedido.NomeCliente = "teste";
            pedido.EstadoOrigem = "SP";
            pedido.EstadoDestino = "RJ";
            pedido.ItensDoPedido = new List<PedidoItem>();

            pedido.ItensDoPedido.Add(new PedidoItem { CodigoProduto = "001", NomeProduto = "teste", Brinde = false, ValorItemPedido = 150 });
  
            NotaFiscal notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            double esperado = 15;
            double resultado = 0;
            foreach (NotaFiscalItem item in notaFiscal.ItensDaNotaFiscal)
            {
                resultado = item.ValorIpi;
            }
            Assert.AreEqual(esperado, resultado, 0.001, "Valor do Ipi incorreto!");

        }
    }
}
    