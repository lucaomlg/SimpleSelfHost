using SimpleSelfHost.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ZebraEPL;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace SimpleSelfHost.Controller
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ImpressoraController : ApiController
    {


        //GET api/impressora
        public IEnumerable<object> Get()
        {
            var listaImpressoras = new List<object>();

            foreach (var impressora in Program.CarregarImpressoras()) 
            {
                listaImpressoras.Add(impressora);
            }

            return listaImpressoras;
        }

        //GET api/impressora/stringDesejada 
        public object Get(string id)
        {
            var exemplo = new ImprimirModel() { Conteudo = "teste", Impressora = Program.CarregarImpressoras()[0] };

            if (id == "modelojson")
                return exemplo;
            else
                return "Utilize a palavra 'modelojson'";
        }


        // POST api/impressora 
        [HttpPost]
        public string Post([FromBody] object value)
        {
            try
            {
                var objeto = JsonConvert.DeserializeObject<ImprimirModel>(value.ToString());
                var teste = string.Empty;

                if (objeto.Modelo == "EPL")
                {
                    teste = objeto.Conteudo.Replace("§", "\n\x0A");
                    teste = teste.Replace('£', '"');

                    Console.WriteLine(teste);

                    if (!RawPrinterHelper.SendStringToPrinter(objeto.Impressora, teste))
                        throw new ApplicationException("Falha ao enviar para impressora. Verifique se o dispositivo está devidamente configurado.");

                    return "Imprimindo";
                }
                else if (objeto.Impressora.Contains("ZDesigner ZT230-200dpi ZPL") && objeto.Modelo == "ZPL")
                {
                    teste = objeto.Conteudo.Replace("§", "\n");
                    teste = teste.Replace('£', '"');

                    if (!RawPrinterHelper.SendStringToPrinter(objeto.Impressora, teste))
                        throw new ApplicationException("Falha ao enviar para impressora. Verifique se o dispositivo está devidamente configurado.");

                    return "Imprimindo";
                }

                else
                    return "Falha na Impressão, Impressora e Modelo são incompatíveis";


               
            }
            catch (Exception ex)
            {
                return $"ERRO\n {ex}";
            }

        }
        // PUT api/impressora/5 
        public void Put(int id, [FromBody] object value) => throw new NotImplementedException();
        

        // DELETE api/impressora/5 
        public void Delete(int id) => throw new NotImplementedException();


        
    }

}
