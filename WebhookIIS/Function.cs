using Integradores;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNet.WebHooks;

namespace WebhookIIS
{
    public class Function
    {
        //[FunctionName("MessageText")]
        //public static async Task<HttpResponseMessage> MessageText([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        //{
        //    log.Info("C# HTTP trigger function processed a request.");

        //    //Teste
        //    Declaracao.oMensagem_log = new Mensagem_log();
        //    Mensagem oMensagem = new Mensagem();
        //    Config oConfig = new Config();

        //    dynamic data = await req.Content.ReadAsAsync<JObject>();

        //    _Funcoes_NET.Config_Carregar(ref oMensagem, ref oConfig, context, req);

        //    oMensagem.EnviarSemTratar = false;
        //    oMensagem.idMensagem = 0;

        //    if (oConfig.Provider.Trim() != "")
        //    {
        //        JObject my_obj = JsonConvert.DeserializeObject<JObject>(data.ToString());

        //        Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

        //        switch (oConfig.Provider.ToUpper().Trim())
        //        {
        //            case Constantes.const_Provider_ChartAPI:
        //                {
        //                    string sfromMe = "";
        //                    string schatId = "";
        //                    string sbody = "";
        //                    string stype = "";

        //                    foreach (KeyValuePair<string, JToken> Item in dict)
        //                    {
        //                        switch (Item.Key.ToString())
        //                        {
        //                            case "messages":
        //                                {
        //                                    //Ler oMensagem - Início
        //                                    sfromMe = ((JObject)((JArray)Item.Value).First)["fromMe"].ToString();
        //                                    schatId = ((JObject)((JArray)Item.Value).First)["chatId"].ToString();
        //                                    sbody = ((JObject)((JArray)Item.Value).First)["body"].ToString();
        //                                    stype = ((JObject)((JArray)Item.Value).First)["type"].ToString();

        //                                    oMensagem.status = "";
        //                                    oMensagem.ParaLista = "";
        //                                    oMensagem.To = schatId;
        //                                    oMensagem.messagebody = sbody;
        //                                    oMensagem.messagetype = stype;
        //                                    oMensagem.messagecuid = "";
        //                                    if (sfromMe.Trim().ToUpper() == "FALSE") { oMensagem.messagedir = "i"; } else { oMensagem.messagedir = "o"; };
        //                                    oMensagem.messageuid = ((JObject)((JArray)Item.Value).First)["id"].ToString();
        //                                    oMensagem.contactuid = ((JObject)((JArray)Item.Value).First)["author"].ToString();
        //                                    if (oMensagem.contactuid.IndexOf("@") > -1)
        //                                        oMensagem.contactuid = _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid.Split(new char[] { '@' })[0]);
        //                                    oMensagem.contactname = ((JObject)((JArray)Item.Value).First)["senderName"].ToString();

        //                                    if (oMensagem.messagedir == "o")
        //                                    {
        //                                        oMensagem.Uid = oMensagem.contactuid;
        //                                        oMensagem.contactuid = _Funcoes.FNC_FormatarTelefone((oMensagem.messageuid.Split(new char[] { '@' })[0]).Substring(5));
        //                                    }
        //                                    //Ler oMensagem - Fim

        //                                    break;
        //                                }
        //                            case "instanceId":
        //                                {
        //                                    if (oMensagem.idBotExterno == "") { oMensagem.idBotExterno = Item.Value.ToString(); }

        //                                    break;
        //                                }
        //                        }
        //                    }

        //                    if (oMensagem.messagedir == "o")
        //                    {
        //                        oMensagem.Uid = oMensagem.contactuid;
        //                        oMensagem.contactuid = _Funcoes.FNC_FormatarTelefone((oMensagem.messageuid.Split(new char[] { '@' })[0]).Substring(5));
        //                    }

        //                    break;
        //                }
        //            case Constantes.const_Provider_Telegram:
        //                {
        //                    foreach (KeyValuePair<string, JToken> Item in dict)
        //                    {
        //                        switch (Item.Key)
        //                        {
        //                            case "update_id":
        //                                {
        //                                    oMensagem.messageuid = Item.Value.ToString();
        //                                    break;
        //                                }
        //                            case "message":
        //                                {
        //                                    if (((JToken)Item.Value["from"]["is_bot"]).ToString().ToUpper() == "TRUE")
        //                                    {
        //                                        oMensagem.messagedir = "o";
        //                                    }
        //                                    else
        //                                    {
        //                                        try
        //                                        {
        //                                            if (((JToken)Item.Value["entities"]).First["type"].ToString() == "bot_command")
        //                                            {
        //                                                oMensagem.messagedir = "o";
        //                                            }
        //                                            else
        //                                            {
        //                                                oMensagem.messagedir = "i";
        //                                            }
        //                                        }
        //                                        catch (System.Exception)
        //                                        {
        //                                            oMensagem.messagedir = "i";
        //                                        }
        //                                    };

        //                                    oMensagem.Usuario = ((JToken)Item.Value["from"]["username"]).ToString();
        //                                    oMensagem.To = ((JToken)Item.Value["chat"]["id"]).ToString();
        //                                    oMensagem.messagebody = Item.Value["text"].ToString();
        //                                    oMensagem.messagetype = "text";
        //                                    oMensagem.contactuid = ((JToken)Item.Value["from"]["id"]).ToString();
        //                                    oMensagem.contactname = ((JToken)Item.Value["from"]["first_name"]).ToString() + " " +
        //                                                           _Funcoes.FNC_NuloString(((JToken)Item.Value["from"]["last_name"]));
        //                                    oMensagem.contactname = oMensagem.contactname.Trim();
        //                                }

        //                                break;
        //                        }
        //                    }
        //                    //Ler oMensagem - Fim

        //                    break;
        //                }
        //        }

        //        Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem, "Recebida");

        //        if (oMensagem.messagebody != null)
        //        {
        //            Integradores.Messengers.EnviarMensagem(ref oMensagem, ref oConfig);
        //        }

        //        JObject response = new JObject();
        //        response.Add("status", "success");
        //        response.Add("attempt", oMensagem.contactuid);
        //        response.Add("id", oMensagem.messagecuid);
        //        response.Add("request_id", oMensagem.messageuid);
        //    }

        //    oMensagem = null;
        //    oConfig = null;

        //    return null;
        //}

        public static Task MessageWebHook_Generico(string receiver, WebHookHandlerContext context)
        {
            try
            {
                // Get JSON from WebHook
                JObject data = context.GetDataOrDefault<JObject>();

                // Get the action for this WebHook coming from the action query parameter in the URI
                string action = context.Actions.FirstOrDefault();

            }
            catch (Exception)
            {

                throw;
            }

            return Task.FromResult(true);

        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

