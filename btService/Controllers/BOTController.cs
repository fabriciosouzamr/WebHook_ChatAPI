using FuncionariosAPIService.Services;
using Integradores;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static btService.Models.Repositorio;

namespace btService.Controllers
{
    public class BOTController : ApiController
    {
        [HttpPost]
        [Route("api/BOT/TratarMensagem")]
        [Trade2UPAuthentication]
        public HttpResponseMessage TratarMensagem(PafaTratarMensagem_Param Param)
        {
            try
            {
                Declaracao.oMensagem_log = new Mensagem_log();
                Config oConfig = new Config();
                Mensagem oMensagem = new Mensagem();

                oConfig.NaoValidarLicenca = true;
                oMensagem.Servico = Integradores._Funcoes.FNC_NuloString(Param.Servico);
                oMensagem.messagebody = Integradores._Funcoes.FNC_NuloString(Param.Mensagem);
                oMensagem.Termo = Integradores._Funcoes.FNC_NuloString(Param.Termo);
                oMensagem.botname = Integradores._Funcoes.FNC_NuloString(Param.Botname);
                oMensagem.contactuid = Integradores._Funcoes.FNC_NuloString(Param.Telefone);
                oMensagem.To = Integradores._Funcoes.FNC_NuloString(Param.Telefone);
                oMensagem.Agente = oMensagem.botname;
                oMensagem.messagedir = "I";
                oMensagem.EnviarSemTratar = false;
                oMensagem.Token = "";

                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, oMensagem.botname);

                Messengers.EnviarMensagem(ref oMensagem, ref oConfig, false);

                return Request.CreateResponse(HttpStatusCode.BadRequest, oMensagem.messagebody_response);
                //return Request.CreateResponse(HttpStatusCode.BadRequest, "");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/BOT/TratarMensagem")]
        [Trade2UPAuthentication]
        public HttpResponseMessage GetTratarMensagem(string Servico, string Mensagem, string Termo, string Botname, string Telefone = "")
        {
            try
            {
                Declaracao.oMensagem_log = new Mensagem_log();
                Config oConfig = new Config();
                Mensagem oMensagem = new Mensagem();

                oConfig.NaoValidarLicenca = true;
                oMensagem.Servico = Integradores._Funcoes.FNC_NuloString(Servico);
                oMensagem.messagebody = Integradores._Funcoes.FNC_NuloString(Mensagem);
                oMensagem.Termo = Integradores._Funcoes.FNC_NuloString(Termo);
                oMensagem.botname = Integradores._Funcoes.FNC_NuloString(Botname);
                oMensagem.contactuid = Integradores._Funcoes.FNC_NuloString(Telefone);
                oMensagem.To = Integradores._Funcoes.FNC_NuloString(Telefone);
                oMensagem.Agente = oMensagem.botname;
                oMensagem.messagedir = "I";
                oMensagem.EnviarSemTratar = false;
                oMensagem.Token = "";

                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, Botname);

                Integradores.Messengers.EnviarMensagem(ref oMensagem, ref oConfig, false);

                return Request.CreateResponse(HttpStatusCode.BadRequest, oMensagem.messagebody_response);
                //return Request.CreateResponse(HttpStatusCode.BadRequest, "");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/BOT/Pesquisa")]
        [Trade2UPAuthentication]
        public HttpResponseMessage Pesquisa(string Botname, string NomePesquisa, string TelefoneEnvio, string TelefoneCliente, string Param = "")
        {
            try
            {
                Declaracao.oMensagem_log = new Mensagem_log();
                Config oConfig = new Config();
                Mensagem oMensagem = new Mensagem();

                oConfig.NaoValidarLicenca = true;
                oMensagem.Servico = Integradores._Funcoes.FNC_NuloString(Constantes.const_Servico_SofiaPesquisas);
                oMensagem.messagebody = Constantes.const_Comando_Pesquisa + "." + Constantes.const_Servico_SofiaPesquisas + "." + NomePesquisa.Trim() + "." + TelefoneCliente.Trim();
                oMensagem.Termo = "";
                oMensagem.botname = Integradores._Funcoes.FNC_NuloString(Botname);
                oMensagem.contactuid = Integradores._Funcoes.FNC_NuloString(TelefoneEnvio);
                oMensagem.To = Integradores._Funcoes.FNC_NuloString(TelefoneEnvio);
                oMensagem.Agente = oMensagem.botname;
                oMensagem.messagedir = "I";
                oMensagem.EnviarSemTratar = false;
                oMensagem.Token = "";

                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, Botname);

                Integradores.Messengers.EnviarMensagem_Padrao(ref oMensagem, ref oConfig, true);

                return Request.CreateResponse(HttpStatusCode.BadRequest, oMensagem.messagebody_response);
                //return Request.CreateResponse(HttpStatusCode.BadRequest, "");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/BOT/PesquisaLote")]
        [Trade2UPAuthentication]
        public HttpResponseMessage PesquisaLote(string Botname,
                                                string Entrevista,
                                                string Pesquisa,
                                                string QuantidadeEnvio,
                                                string IntervaloSegundos,
                                                string AnaliticoSintetico,
                                                string TelefoneEnvio)
        {
            //pesquisa.purina.11.0.enviar.20.0.A
            try
            {
                Declaracao.oMensagem_log = new Mensagem_log();
                Config oConfig = new Config();
                Mensagem oMensagem = new Mensagem();

                oConfig.NaoValidarLicenca = true;
                oConfig.Provider = "ZP";
                oMensagem.Servico = Integradores._Funcoes.FNC_NuloString(Constantes.const_Servico_SofiaPesquisas);
                oMensagem.messagebody = Constantes.const_Comando_Pesquisa + "." + Botname.Trim() + "." +
                                                                                  Entrevista.ToString() + "." +
                                                                                  Pesquisa.ToString() + ".enviar." +
                                                                                  QuantidadeEnvio.ToString() + "." +
                                                                                  IntervaloSegundos.ToString() + "." +
                                                                                  AnaliticoSintetico.ToString();
                oMensagem.Termo = "";
                oMensagem.botname = Integradores._Funcoes.FNC_NuloString(Botname);
                oMensagem.contactuid = Integradores._Funcoes.FNC_NuloString(TelefoneEnvio);
                oMensagem.To = Integradores._Funcoes.FNC_NuloString(TelefoneEnvio);
                oMensagem.Agente = oMensagem.botname;
                oMensagem.messagedir = "I";
                oMensagem.EnviarSemTratar = false;
                oMensagem.Token = "";

                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, Botname);

                Integradores.Messengers.EnviarMensagem(ref oMensagem, ref oConfig, true);

                return Request.CreateResponse(HttpStatusCode.BadRequest, oMensagem.messagebody_response);
                //return Request.CreateResponse(HttpStatusCode.BadRequest, "");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/BOT/Boomerangue")]
        //[Trade2UPAuthentication]
        public HttpResponseMessage Boomerangue(string Botname, string NomeBoomerangue, string TelefoneEnvio, string TelefoneCliente)
        {
            try
            {
                Declaracao.oMensagem_log = new Mensagem_log();
                Config oConfig = new Config();
                Mensagem oMensagem = new Mensagem();

                oConfig.NaoValidarLicenca = true;
                oMensagem.Servico = Integradores._Funcoes.FNC_NuloString(Constantes.const_Servico_ID_BoomeranguePurina);
                oMensagem.messagebody = Constantes.const_Comando_Pesquisa + "." + Constantes.const_Servico_BoomeranguePurina + ".BOOMERANGUE_" + NomeBoomerangue.Trim() + "." + TelefoneCliente.Trim();
                oMensagem.Termo = "";
                oMensagem.botname = Integradores._Funcoes.FNC_NuloString(Botname);
                oMensagem.contactuid = Integradores._Funcoes.FNC_NuloString(TelefoneEnvio);
                oMensagem.To = Integradores._Funcoes.FNC_NuloString(TelefoneEnvio);
                oMensagem.Agente = oMensagem.botname;
                oMensagem.messagedir = "I";
                oMensagem.EnviarSemTratar = false;
                oMensagem.Token = "";

                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, Botname);

                Integradores.Messengers.EnviarMensagem_Padrao(ref oMensagem, ref oConfig, true);

                return Request.CreateResponse(HttpStatusCode.BadRequest, oMensagem.Erro) ;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, "");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
