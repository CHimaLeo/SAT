using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SAT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            string url = "https://tramitesdigitales.sat.gob.mx/Sicofi.wsExtValidacionCFD/WsValidacionCFDsExt.asmx?WSDL";
            //string xml = "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'><soapenv:Heade/><soapenv:Body><tem:Consulta><tem:expresionImpresa><![CDATA[?re=LAN8507268IA&rr=LAN7008173R5&tt=5800.00&id=4e87d1d7-a7d0-465f-a771-1dd216f63c1a]]></tem:expresionImpresa></tem:Consulta></soapenv:Body></soapenv:Envelope>";
            string xml = "<?xml version='1.0' encoding='UTF-8'?><ColleccionFoliosCfd xmlns = 'http://www.sat.gob.mx/Asf/Sicofi/ValidacionFoliosCFD/1.0.0'><Folio><Id>1</Id><Rfc>AAA121212AAA</Rfc><Serie>FA</Serie><NumeroFolio>9</NumeroFolio><NumeroAprobacion>12345</NumeroAprobacion><AnioAprobacion>2010</AnioAprobacion><CertificadoNumeroSerie>0000011111111</CertificadoNumeroSerie><CertificadoFechaEmision>2011-12-12T14:20:15</CertificadoFechaEmision></Folio></ColleccionFoliosCfd>";

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
            
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xml);

                request.ContentType = " text/xml;charset='utf-8'";
                request.ContentLength = bytes.Length;
                request.Headers.Add("SOAPAction", "http://tempuri.org/IConsultaCFDIService/Consulta");
                request.Headers.Add("Host", "consultaqr.facturaelectronica.sat.gob.mx");
                request.Headers.Add("accept - encoding", "gzip, deflate");

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                using (dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                }

                response.Close();

                return Enumerable.Range(1, 5).Select(index => new WeatherForecast 
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                //Cadena = respuesta
            });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
