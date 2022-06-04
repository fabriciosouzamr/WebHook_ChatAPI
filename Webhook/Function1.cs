using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Integradores;
using Microsoft.AspNet.WebHooks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Webhook_
{
    public static class Function1
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        
        [FunctionName("MessageWebHook")]
        public static async Task<HttpResponseMessage> MessageWebHook([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            log.Info("C# HTTP trigger function processed a request.");

            //Teste
            Declaracao.oMensagem_log = new Mensagem_log();
            Mensagem oMensagem = new Mensagem();
            Config oConfig = new Config();

            dynamic data = await req.Content.ReadAsAsync<JObject>();

            _Funcoes_Azure.Config_Carregar(ref oMensagem, ref oConfig, context, req);

            oMensagem.EnviarSemTratar = false;
            oMensagem.idMensagem = 0;

            if (oConfig.Provider.Trim() != "")
            {
                JObject my_obj = JsonConvert.DeserializeObject<JObject>(data.ToString());

                Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

                switch (oConfig.Provider.ToUpper().Trim())
                {
                    case Constantes.const_Provider_ChartAPI:
                        {
                            string sfromMe = "";
                            string schatId = "";
                            string sbody = "";
                            string stype = "";
                            string sinstanceId = "";

                            foreach (KeyValuePair<string, JToken> Item in dict)
                            {
                                switch (Item.Key.ToString())
                                {
                                    case "messages":
                                        {
                                            //Ler oMensagem - Início
                                            sfromMe = ((JObject)((JArray)Item.Value).First)["fromMe"].ToString();
                                            schatId = ((JObject)((JArray)Item.Value).First)["chatId"].ToString();
                                            sbody = ((JObject)((JArray)Item.Value).First)["body"].ToString();
                                            stype = ((JObject)((JArray)Item.Value).First)["type"].ToString();

                                            oMensagem.status = "";
                                            oMensagem.ParaLista = "";
                                            oMensagem.To = schatId;
                                            oMensagem.messagebody = sbody;
                                            oMensagem.messagetype = stype;
                                            oMensagem.messagecuid = "";
                                            if (sfromMe.Trim().ToUpper() == "FALSE") { oMensagem.messagedir = "i"; } else { oMensagem.messagedir = "o"; };
                                            oMensagem.messageuid = ((JObject)((JArray)Item.Value).First)["id"].ToString();
                                            oMensagem.contactuid = ((JObject)((JArray)Item.Value).First)["author"].ToString();
                                            if (oMensagem.contactuid.IndexOf("@") > -1)
                                                oMensagem.contactuid = _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid.Split(new char[] { '@' })[0]);
                                            oMensagem.contactname = ((JObject)((JArray)Item.Value).First)["senderName"].ToString();

                                            if (oMensagem.messagedir == "o")
                                            {
                                                oMensagem.Uid = oMensagem.contactuid;
                                                oMensagem.contactuid = _Funcoes.FNC_FormatarTelefone((oMensagem.messageuid.Split(new char[] { '@' })[0]).Substring(5));
                                            }
                                            //Ler oMensagem - Fim

                                            break;
                                        }
                                    case "instanceId":
                                        {
                                            if (oMensagem.idBotExterno == "") { oMensagem.idBotExterno = Item.Value.ToString(); }

                                            oMensagem.instanceId = Item.Value.ToString();

                                            break;
                                        }
                                    case "ack":
                                        {
                                            schatId = ((JObject)((JArray)Item.Value).First)["chatId"].ToString();

                                            oMensagem.To = schatId;
                                            oMensagem.chat_status = ((JObject)((JArray)Item.Value).First)["status"].ToString();
                                            oMensagem.messagedir = "o";

                                            break;
                                        }
                                }
                            }

                            if (oMensagem.messagedir == "o")
                            {
                                oMensagem.Uid = oMensagem.contactuid;
                                oMensagem.contactuid = _Funcoes.FNC_FormatarTelefone((oMensagem.messageuid.Split(new char[] { '@' })[0]).Substring(5));
                            }

                            break;
                        }
                    case Constantes.const_Provider_Telegram:
                        {
                            foreach (KeyValuePair<string, JToken> Item in dict)
                            {
                                switch (Item.Key)
                                {
                                    case "update_id":
                                        {
                                            oMensagem.messageuid = Item.Value.ToString();
                                            break;
                                        }
                                    case "message":
                                        {
                                            if (((JToken)Item.Value["from"]["is_bot"]).ToString().ToUpper() == "TRUE")
                                            {
                                                oMensagem.messagedir = "o";
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (((JToken)Item.Value["entities"]).First["type"].ToString() == "bot_command")
                                                    {
                                                        oMensagem.messagedir = "o";
                                                    }
                                                    else
                                                    {
                                                        oMensagem.messagedir = "i";
                                                    }
                                                }
                                                catch (System.Exception)
                                                {
                                                    oMensagem.messagedir = "i";
                                                }
                                            };

                                            oMensagem.Usuario = ((JToken)Item.Value["from"]["username"]).ToString();
                                            oMensagem.To = ((JToken)Item.Value["chat"]["id"]).ToString();
                                            oMensagem.messagebody = Item.Value["text"].ToString();
                                            oMensagem.messagetype = "text";
                                            oMensagem.contactuid = ((JToken)Item.Value["from"]["id"]).ToString();
                                            oMensagem.contactname = ((JToken)Item.Value["from"]["first_name"]).ToString() + " " +
                                                                   _Funcoes.FNC_NuloString(((JToken)Item.Value["from"]["last_name"]));
                                            oMensagem.contactname = oMensagem.contactname.Trim();
                                        }

                                        break;
                                }
                            }
                            //Ler oMensagem - Fim

                            break;
                        }
                    case Constantes.const_Provider_BTrive:
                        {
                            foreach (KeyValuePair<string, JToken> Item in dict)
                            {
                                switch (Item.Key.ToString())
                                {
                                    case "action":
                                        {
                                            if (Item.Value.ToString()=="sender")
                                            {
                                                return null;
                                            }

                                            break;
                                        }
                                    case "device_id":
                                        {
                                            oMensagem.idBotExterno = Item.Value.ToString();
                                            oMensagem.To = Item.Value.ToString();

                                            break;
                                        }
                                    case "message":
                                        {
                                            oMensagem.messagebody = Item.Value.ToString();

                                            break;
                                        }
                                    case "sender":
                                        {
                                            oMensagem.contactuid = Item.Value.ToString();

                                            break;
                                        }
                                    case "message_id":
                                        {
                                            oMensagem.messageuid = Item.Value.ToString();

                                            break;
                                        }
                                }
                            }

                            oMensagem.messagedir = "I";

                            if ( ! Declaracao.Controle_Requisicao_Validar(oMensagem.idBotExterno)) { return null;  }

                            break;
                        }
                }

                Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem, "Recebida");

                if (oMensagem.messagebody != null)
                {
                    Integradores.Messengers.EnviarMensagem(ref oMensagem, ref oConfig);
                }

                JObject response = new JObject();
                response.Add("status", "success");
                response.Add("attempt", oMensagem.contactuid);
                response.Add("id", oMensagem.messagecuid);
                response.Add("request_id", oMensagem.messageuid);
            }

            oMensagem = null;
            oConfig = null;

            return null;
        }

        [FunctionName("MessageWebHook_Fnc")]
        public static async Task<HttpResponseMessage> MessageWebHook_Fnc([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            log.Info("C# HTTP trigger function processed a request.");
            NameValueCollection request = await req.Content.ReadAsFormDataAsync();

            Declaracao.oMensagem_log = new Mensagem_log();
            Mensagem oMensagem = new Mensagem();
            Config oConfig = new Config();

            try
            {
                var data = request;// JObject.Parse(request);

                _Funcoes_Azure.Config_Carregar(ref oMensagem, ref oConfig, context, req);

                switch (oConfig.Provider)
                {
                    case Constantes.const_Provider_Waboxapp:
                        {
                            oMensagem.idMensagem = 0;
                            oMensagem.EnviarSemTratar = false;
                            oMensagem.Agente = oMensagem.botname;
                            oMensagem.BotNameResposta = oMensagem.botname;
                            oMensagem.events = data["event"].ToString();
                            oMensagem.Token = data["token"].ToString();                            
                            oMensagem.idBotExterno = data["uid"].ToString();
                            //oMensagem.Uid = data["uid"].ToString();
                            oMensagem.contactuid = data["contact[uid]"].ToString();
                            oMensagem.contactname = data["contact[name]"].ToString();
                            oMensagem.contacttype = data["contact[type]"].ToString();
                            oMensagem.messagedtm = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToDouble(data["message[dtm]"].ToString()));
                            oMensagem.messagedtmd = DateTime.Now;
                            oMensagem.messagerqt = DateTime.Now;
                            oMensagem.messagerqtd = DateTime.Now;
                            oMensagem.messagerst = DateTime.Now;
                            oMensagem.messageuid = data["message[uid]"].ToString();
                            oMensagem.messagecuid = data["message[cuid]"].ToString();
                            oMensagem.messagedir = data["message[dir]"].ToString();
                            oMensagem.messagetype = data["message[type]"].ToString();
                            oMensagem.messagebody = data["message[body][text]"].ToString();
                            oMensagem.messageack = data["message[ack]"].ToString();
                            oMensagem.status = "";
                            oMensagem.ParaLista = "";
                            oMensagem.Termo = "";
                            oMensagem.idStatusMensagem = 1;

                            break;
                        }
                }

                try
                {
                    bool bOk = false;

                    bOk = Integradores.Messengers.EnviarMensagem(ref oMensagem, ref oConfig);
                }
                catch (Exception ex)
                {
                    JObject errresponse = new JObject();
                    errresponse.Add("Error", ex.Message);
                    return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
                }

                JObject response = new JObject();
                response.Add("status", "success");
                response.Add("attempt", oMensagem.contactuid);
                response.Add("id", oMensagem.messagecuid);
                response.Add("request_id", oMensagem.messageuid);

                oMensagem = null;
                oConfig =  null;

                return req.CreateResponse(HttpStatusCode.OK, response.ToString());
            }
            catch (Exception ex)
            {
                JObject errresponse = new JObject();
                errresponse.Add("Error", ex.Message);
                return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
            }
        }

        [FunctionName("MessageWebHook_Util")]
        public static async Task<HttpResponseMessage> MessageWebHook_Util([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            const string const_Key_Solicitacao = "Solicitacao";
            const string const_Key_Solicitacao_EnviarMensagem = "1";
            const string const_Key_Provider = "Provider";
            const string const_Key_Servico = "Servico";
            const string const_Key_Mensagem = "Mensagem";
            const string const_Key_File = "File";
            const string const_Key_Termo = "Termo";
            const string const_Key_Para = "Para";
            const string const_Key_Chave = "Chave";
            const string const_Key_Botname = "Botname";
            const string const_Key_Contact_Name = "Contact_Name";
            const string const_Key_CodPuxada = "CodPuxada";
            const string const_Key_Tratar = "Tratar";

            Mensagem oMensagem = new Mensagem();
            Config oConfig = new Config();

            _Funcoes_Azure.Config_Carregar(ref oMensagem, ref oConfig, context, req);

            log.Info("C# HTTP trigger function processed a request.");

            Declaracao.oMensagem_log = new Mensagem_log();

            bool bOk = false;

            JObject response = new JObject();

            object data;

            try
            {
                try
                {
                    data = await req.Content.ReadAsAsync<JObject>();
                }
                catch (Exception)
                {
                    data = req;// JObject.Parse(request);
                }

                //Config_Carregar(context, req);

                try
                {
                    JObject my_obj = JsonConvert.DeserializeObject<JObject>(data.ToString());

                    Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

                    if (dict.ContainsKey(const_Key_Chave))
                    {
                        if (JsonHelper.Procurar(dict, const_Key_Chave).ToString() != "x3|b0$/; 0KvpP34%WUl|qN!|U~$OPbco`elYQGuN(gs(A#]0A!")
                        {
                            response.Add("status", "Identificação inválida");
                        }
                        else
                        {
                            if (dict.ContainsKey(const_Key_Solicitacao))
                            {
                                try
                                {
                                    string scontactuid = JsonHelper.Procurar(dict, const_Key_Para).ToString();
                                    string scontactname = JsonHelper.Procurar(dict, const_Key_Contact_Name).ToString();

                                    string[] contactuid = null;
                                    string[] contactname = null;

                                    if (scontactname.IndexOf(";") != -1)
                                    {
                                        contactuid = scontactuid.Split(';');
                                        contactname = scontactname.Split(';');
                                    }
                                    else
                                    {
                                        contactuid = new string[] { scontactuid };
                                        contactname = new string[] { scontactname };
                                    }

                                    switch (JsonHelper.Procurar(dict, const_Key_Solicitacao).ToString())
                                    {
                                        case const_Key_Solicitacao_EnviarMensagem:
                                            {
                                                for (int i = 0; i < contactuid.Length; i++)
                                                {
                                                    oConfig.Provider = JsonHelper.Procurar(dict, const_Key_Provider).ToString();
                                                    oConfig.NaoValidarLicenca = true;
                                                    oMensagem.ToDirect = true;
                                                    oMensagem.Servico = JsonHelper.Procurar(dict, const_Key_Servico).ToString();
                                                    oMensagem.messagebody = JsonHelper.Procurar(dict, const_Key_Mensagem).ToString();
                                                    oMensagem.Termo = JsonHelper.Procurar(dict, const_Key_Termo).ToString();
                                                    oMensagem.contactuid = contactuid[i];
                                                    oMensagem.To = JsonHelper.Procurar(dict, const_Key_Para).ToString();
                                                    oMensagem.ToDirect = false;
                                                    oMensagem.botname = JsonHelper.Procurar(dict, const_Key_Botname).ToString();

                                                    try
                                                    {
                                                        oMensagem.CodPuxada = JsonHelper.Procurar(dict, const_Key_CodPuxada).ToString();
                                                    }
                                                    catch (Exception)
                                                    {
                                                    }
                                                    try
                                                    {
                                                        oMensagem.messagefile = JsonHelper.Procurar(dict, const_Key_File).ToString();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        oMensagem.messagefile = "";
                                                    }
                                                    try
                                                    {
                                                        oMensagem.EnviarSemTratar = (JsonHelper.Procurar(dict, const_Key_Tratar).ToString() == "S");
                                                    }
                                                    catch (Exception)
                                                    {
                                                        oMensagem.EnviarSemTratar = true;
                                                    }

                                                    oMensagem.contactname = contactname[i];
                                                    oMensagem.Agente = oMensagem.botname;
                                                    oMensagem.messagedir = "I";
                                                    oMensagem.Token = "";
                                                    oMensagem.status = "";
                                                    oMensagem.ParaLista = "";
                                                    oMensagem.EnvioInterno = true;
                                                    oMensagem.MessageWebHook_Util = true;
                                                    oConfig.NaoValidarLicenca = true;
                                                    oConfig.UsuarioAdminValido = true;
                                                    bOk = Integradores.Messengers.EnviarMensagem_Padrao(ref oMensagem, ref oConfig);
                                                }

                                                break;
                                            }
                                    }

                                    response.Add("status", "success");

                                    if (Declaracao.ErroMensagem != "" && Declaracao.ErroMensagem != "Gravar")
                                        response.Add("ErroMensagem", Declaracao.ErroMensagem);
                                }
                                catch (Exception Ex)
                                {
                                    response.Add("status", Ex.Message);
                                }
                            }
                            else
                            {
                                response.Add("status", "Requisição não reconhecida");
                            }
                        }
                    }
                    else
                    {
                        response.Add("status", "Identificação não informada");
                    }

                    oMensagem = null;
                    oConfig = null;

                    return req.CreateResponse(HttpStatusCode.OK, response.ToString());
                }
                catch (Exception ex)
                {
                    JObject errresponse = new JObject();
                    errresponse.Add("Error", ex.Message);
                    return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
                }
            }
            catch (Exception ex)
            {
                JObject errresponse = new JObject();
                errresponse.Add("Error", ex.Message);
                return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
            }
        }

        public class RequestState
        {
            // This class stores the request state of the request.
            public WebRequest request;
            public RequestState()
            {
                request = null;
            }
        }

        private static void ReadCallback(IAsyncResult asynchronousResult)
        {
            RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
            WebRequest myWebRequest = myRequestState.request;

            // End the Asynchronus request.
            Stream streamResponse = myWebRequest.EndGetRequestStream(asynchronousResult);

            // Create a string that is to be posted to the uri.
            Console.WriteLine("Please enter a string to be posted:");
            string postData = Console.ReadLine();
            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Write the data to the stream.
            streamResponse.Write(byteArray, 0, postData.Length);
            streamResponse.Close();
            allDone.Set();
        }
    }
}
