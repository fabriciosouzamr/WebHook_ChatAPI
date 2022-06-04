using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Integradores
{
    public static class _Funcoes_Azure
    {
        public static void Config_Carregar(ref Mensagem oMensagem,
                                           ref Config oConfig,
                                           Microsoft.Azure.WebJobs.ExecutionContext context,
                                           HttpRequestMessage req, 
                                           bool CarregarParam = true)
        {
            //oMensagem - Inicializar - Início
            oMensagem.botname = "";
            oMensagem.events = "";
            oMensagem.contactuid = "";
            oMensagem.contactname = "";
            oMensagem.contacttype = "";
            oMensagem.messagedtm = DateTime.Now;
            oMensagem.messagercv = DateTime.Now;
            oMensagem.messagemtd = DateTime.Now;
            oMensagem.messagerqt = DateTime.Now;
            oMensagem.messagerst = DateTime.Now;
            oMensagem.messageuid = "";
            oMensagem.messagecuid = "";
            oMensagem.messagedir = "";
            oMensagem.messagetype = "";
            oMensagem.messagebody = "";
            oMensagem.messagebody_response = "";
            oMensagem.messageack = "";
            //oMensagem - Inicializar - Fim

            //Carregar parâmetro - Início
            string parametro = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "param", true) == 0).Value;

            if (CarregarParam)
            {
                try
                {
                    oConfig.Provider = parametro.Split('.')[1].ToString().ToUpper().Trim();
                    oMensagem.botname = parametro.Split('.')[0].ToString().ToUpper().Trim();

                    if (parametro.Split('.').Length >= 3)
                    {
                        if (parametro.Split('.')[2].ToString().ToUpper().Trim().Length == 3)
                        {
                            oMensagem.Servico = parametro.Split('.')[2].ToString().ToUpper().Trim();

                            if (parametro.Split('.').Length >= 4)
                            {
                                oMensagem.idBotExterno = parametro.Split('.')[3].ToString().ToUpper().Trim();
                            }
                        }
                        else
                        {
                            oMensagem.idBotExterno = parametro.Split('.')[2].ToString().ToUpper().Trim();
                        }
                    }

                }
                catch (Exception)
                {
                }
            }

            if (string.IsNullOrEmpty(oMensagem.botname))
                oMensagem.botname = "alice";
            if (string.IsNullOrEmpty(oConfig.Provider))
                oConfig.Provider = " ";
            //Carregar parâmetro - Fim

            //Carregar configuração - Início
            string root = context.FunctionAppDirectory;
            root += "\\Config.json";

            StreamReader r = new StreamReader(root);
            var ConfigJson = r.ReadToEnd();
            var Json = JObject.Parse(ConfigJson);

            oConfig.Carregar(Json, oMensagem.botname);
            //Carregar configuração - Fim

            //Ajusta as oMensagem - Inicio
            oMensagem.messagercv = DateTime.Now;
            oMensagem.messagercvd = DateTime.Now;
            oMensagem.messagedtm = DateTime.Now;
            oMensagem.messagedtmd = DateTime.Now;
            oMensagem.idMessageTerms = 0;
            oMensagem.Agente = oMensagem.botname;
            oMensagem.events = "message";
            oMensagem.Token = "ppbh90";
            oMensagem.messageack = "0";
            oMensagem.contacttype = "user";
            oMensagem.idStatusMensagem = 1;
            //Ajusta as oMensagem - Inicio

            Assembly _assembly = Assembly.GetExecutingAssembly();

            Declaracao.processador = "webhook_" + _assembly.GetName().Version.Major.ToString().Trim() + "." +
                                                  _assembly.GetName().Version.Minor.ToString().Trim() + "." +
                                                  _assembly.GetName().Version.Build.ToString().Trim() + "." +
                                                  _assembly.GetName().Version.Revision.ToString().Trim();
        }

        public static void Config_Carregar_Gerenciador(ref Config oConfig,
                                                       Microsoft.Azure.WebJobs.ExecutionContext context, 
                                                       HttpRequestMessage req)
        {
            //Carregar parâmetro - Início
            string parametro = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "param", true) == 0).Value;

            //Carregar configuração - Início
            string root = context.FunctionAppDirectory;
            root += "\\Config.json";

            StreamReader r = new StreamReader(root);
            var ConfigJson = r.ReadToEnd();
            var Json = JObject.Parse(ConfigJson);

            oConfig.Carregar(Json, "gerenciador");
            //Carregar configuração - Fim

            Assembly _assembly = Assembly.GetExecutingAssembly();

            Declaracao.processador = "webhook_" + _assembly.GetName().Version.Major.ToString().Trim() + "." +
                                                  _assembly.GetName().Version.Minor.ToString().Trim() + "." +
                                                  _assembly.GetName().Version.Build.ToString().Trim() + "." +
                                                  _assembly.GetName().Version.Revision.ToString().Trim();
        }
    }
}