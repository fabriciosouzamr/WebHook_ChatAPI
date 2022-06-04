using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Integradores;
using Microsoft.AspNet.WebHooks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace botService
{
    public static class botService_Funcoes
    {
        //[FunctionName("MessageText")]
        //public static async Task<HttpResponseMessage> MessageText([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        //{
        //    const string const_Key_Solicitacao = "Solicitacao";
        //    const string const_Key_Solicitacao_EnviarMensagem = "1";
        //    const string const_Key_Provider = "Provider";
        //    const string const_Key_Servico = "Servico";
        //    const string const_Key_Mensagem = "Mensagem";
        //    const string const_Key_Termo = "Termo";
        //    const string const_Key_Para = "Para";
        //    const string const_Key_Chave = "Chave";
        //    const string const_Key_Botname = "Botname";
        //    const string const_Key_Contact_Name = "Contact_Name";

        //    //_Funcoes_Azure.Config_Carregar(context, req);

        //    //log.Info("C# HTTP trigger function processed a request.");

        //    //bool bOk = false;

        //    //JObject response = new JObject();

        //    //object data;

        //    //try
        //    //{
        //    //    try
        //    //    {
        //    //        data = await req.Content.ReadAsAsync<JObject>();
        //    //    }
        //    //    catch (Exception)
        //    //    {
        //    //        data = req;// JObject.Parse(request);
        //    //    }

        //    //    //Config_Carregar(context, req);

        //    //    try
        //    //    {
        //    //        JObject my_obj = JsonConvert.DeserializeObject<JObject>(data.ToString());

        //    //        Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

        //    //        if (dict.ContainsKey(const_Key_Chave))
        //    //        {
        //    //            if (JsonHelper.Procurar(dict, const_Key_Chave).ToString() != "x3|b0$/; 0KvpP34%WUl|qN!|U~$OPbco`elYQGuN(gs(A#]0A!")
        //    //            {
        //    //                response.Add("status", "Identificação inválida");
        //    //            }
        //    //            else
        //    //            {
        //    //                if (dict.ContainsKey(const_Key_Solicitacao))
        //    //                {
        //    //                    try
        //    //                    {
        //    //                        string scontactuid = JsonHelper.Procurar(dict, const_Key_Para).ToString();
        //    //                        string scontactname = JsonHelper.Procurar(dict, const_Key_Contact_Name).ToString();

        //    //                        string[] contactuid = null;
        //    //                        string[] contactname = null;

        //    //                        if (scontactname.IndexOf(";") != -1)
        //    //                        {
        //    //                            contactuid = scontactuid.Split(';');
        //    //                            contactname = scontactname.Split(';');
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            contactuid = new string[] { scontactuid };
        //    //                            contactname = new string[] { scontactname };
        //    //                        }

        //    //                        switch (JsonHelper.Procurar(dict, const_Key_Solicitacao).ToString())
        //    //                        {
        //    //                            case const_Key_Solicitacao_EnviarMensagem:
        //    //                                {
        //    //                                    for (int i = 0; i < contactuid.Length; i++)
        //    //                                    {
        //    //                                        Config.Provider = JsonHelper.Procurar(dict, const_Key_Provider).ToString();
        //    //                                        Mensagem.Servico = JsonHelper.Procurar(dict, const_Key_Servico).ToString();
        //    //                                        Mensagem.messagebody = JsonHelper.Procurar(dict, const_Key_Mensagem).ToString();
        //    //                                        Mensagem.Termo = JsonHelper.Procurar(dict, const_Key_Termo).ToString();
        //    //                                        Mensagem.contactuid = contactuid[i];
        //    //                                        Mensagem.To = JsonHelper.Procurar(dict, const_Key_Para).ToString();
        //    //                                        Mensagem.botname = JsonHelper.Procurar(dict, const_Key_Botname).ToString();
        //    //                                        Mensagem.contactname = contactname[i];
        //    //                                        Mensagem.Agente = Mensagem.botname;
        //    //                                        Mensagem.messagedir = "I";
        //    //                                        Mensagem.EnviarSemTratar = false;
        //    //                                        Mensagem.Token = "";
        //    //                                        bOk = Integradores.Messengers.EnviarMensagem(Mensagem.messagebody, false);
        //    //                                    }

        //    //                                    break;
        //    //                                }
        //    //                        }

        //    //                        response.Add("status", "success");
        //    //                        response.Add("Mensagem", Mensagem.messagebody_response);
        //    //                    }
        //    //                    catch (Exception Ex)
        //    //                    {
        //    //                        response.Add("status", "error");
        //    //                        response.Add("error", Ex.Message);
        //    //                    }
        //    //                }
        //    //                else
        //    //                {
        //    //                    response.Add("status", "Requisição não reconhecida");
        //    //                }
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            response.Add("status", "Identificação não informada");
        //    //        }

        //    //        return req.CreateResponse(HttpStatusCode.OK, response.ToString());
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        JObject errresponse = new JObject();
        //    //        errresponse.Add("Error", ex.Message);
        //    //        return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    JObject errresponse = new JObject();
        //    //    errresponse.Add("Error", ex.Message);
        //    //    return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
        //    //}
        //}

        //[FunctionName("MessageTextAut")]
        //public static async Task<HttpResponseMessage> MessageTextAut([HttpTrigger(AuthorizationLevel.User , "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        //{
        //    const string const_Key_Servico = "Servico";
        //    const string const_Key_Mensagem = "Mensagem";
        //    const string const_Key_Termo = "Termo";
        //    const string const_Key_Chave = "Chave";
        //    const string const_Key_Para = "Para";
        //    const string const_Key_Botname = "Botname";

        //    _Funcoes_Azure.Config_Carregar(context, req);

        //    log.Info("C# HTTP trigger function processed a request.");

        //    Declaracao.oMensagem_log = new Mensagem_log();

        //    bool bOk = false;

        //    JObject response = new JObject();

        //    object data;

        //    try
        //    {
        //        try
        //        {
        //            data = await req.Content.ReadAsAsync<JObject>();
        //        }
        //        catch (Exception)
        //        {
        //            data = req;// JObject.Parse(request);
        //        }

        //        //Config_Carregar(context, req);

        //        try
        //        {
        //            JObject my_obj = JsonConvert.DeserializeObject<JObject>(data.ToString());

        //            Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

        //            if (dict.ContainsKey(const_Key_Chave))
        //            {
        //                if (JsonHelper.Procurar(dict, const_Key_Chave).ToString() != "x3|b0$/; 0KvpP34%WUl|qN!|U~$OPbco`elYQGuN(gs(A#]0A!")
        //                {
        //                    response.Add("status", "Identificação inválida");
        //                }
        //                else
        //                {
        //                    try
        //                    {
        //                        Config.NaoValidarLicenca = true;
        //                        Mensagem.Servico = JsonHelper.Procurar(dict, const_Key_Servico).ToString();
        //                        Mensagem.messagebody = JsonHelper.Procurar(dict, const_Key_Mensagem).ToString();
        //                        Mensagem.Termo = JsonHelper.Procurar(dict, const_Key_Termo).ToString();
        //                        Mensagem.botname = JsonHelper.Procurar(dict, const_Key_Botname).ToString();
        //                        Mensagem.contactuid = JsonHelper.Procurar(dict, const_Key_Para).ToString();
        //                        Mensagem.To = JsonHelper.Procurar(dict, const_Key_Para).ToString();
        //                        Mensagem.Agente = Mensagem.botname;
        //                        Mensagem.messagedir = "I";
        //                        Mensagem.EnviarSemTratar = false;
        //                        Mensagem.Token = "";
        //                        bOk = Integradores.Messengers.EnviarMensagem(Mensagem.messagebody, false);

        //                        response.Add("status", "success");
        //                        response.Add("Mensagem", Mensagem.messagebody_response);
        //                    }
        //                    catch (Exception Ex)
        //                    {
        //                        response.Add("status", "error");
        //                        response.Add("error", Ex.Message);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                response.Add("status", "Identificação não informada");
        //            }

        //            return req.CreateResponse(HttpStatusCode.OK, response.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            JObject errresponse = new JObject();
        //            errresponse.Add("Error", ex.Message);
        //            return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        JObject errresponse = new JObject();
        //        errresponse.Add("Error", ex.Message);
        //        return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
        //    }
        //}

        //[FunctionName("Ativacao")]
        //public static async Task<HttpResponseMessage> Ativacao([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        //{
        //    const string const_Ativar_Usuario = "1P0";
        //    const string const_Ativar_Empresa = "2E3";

        //    string sRetorno = "Erro na solicitação";
        //    string sMensagem = "";

        //    try
        //    {
        //        string parametro = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "param", true) == 0).Value;

        //        string sSql_Pesquisa = "";
        //        string sSql_Ativacao = "";
        //        string sCdAtivacao = "";
        //        bool bAtiviar = false;
        //        int iId = 0;
        //        string sTipo = "";  //C Cliente -   E Empresa

        //        sCdAtivacao = parametro.Split('.')[0];
        //        iId = Convert.ToInt32(parametro.Split('.')[1]);

        //        switch (parametro.Substring(0, 3))
        //        {
        //            case const_Ativar_Usuario:
        //                {
        //                    sTipo = "U";
        //                    bAtiviar = true;
        //                    sSql_Pesquisa = "";

        //                    sSql_Pesquisa = "select usu.Nome, usu.TELEFONE, usu.TELEGRAM, usu.CNPJ, EMP.ContatoEMail, EMP.Empresa Partner" +
        //                                    " from tb_Usuario usu" +
        //                                     " left join tb_empresas EMP on EMP.idEmpresa = usu.idEmpresa" +
        //                                    " where upper(usu.cd_ativacao) = '" + sCdAtivacao + "'";

        //                    sSql_Ativacao = "update tb_Usuario set tp_liberado = 'S' where upper(cd_ativacao) = '" + sCdAtivacao + "'";
        //                    break;
        //                }
        //            case const_Ativar_Empresa:
        //                {
        //                    sTipo = "E";
        //                    sSql_Pesquisa = "select EMP.Empresa, EMP.CodTelefone, EMP.CNPJ, EMP.ContatoEMail, PTN.Empresa Partner" +
        //                                    " from tb_empresas EMP" +
        //                                     " left join tb_empresas PTN on PTN.idEmpresa = EMP.idEmpresa_Gestora" +
        //                                     " where upper(EMP.cd_ativacao) = '" + sCdAtivacao + "'";
        //                    sSql_Ativacao = "update tb_empresas set tp_liberado = 'S' where upper(cd_ativacao) = '" + sCdAtivacao + "'";
        //                    bAtiviar = true;
        //                    break;
        //                }
        //        }

        //        if (bAtiviar)
        //        {
        //            _Funcoes_Azure.Config_Carregar(context, req, false);
        //            Config.oBancoDados = new clsBancoDados();
        //            Config.oBancoDados.DBConectar(Config.tipobancodados, Config.dbconstring);
        //            System.Data.DataTable oData;

        //            oData = Config.oBancoDados.DBQuery(sSql_Pesquisa);

        //            if (oData.Rows.Count == 0)
        //            {
        //                switch (parametro.Substring(0, 3))
        //                {
        //                    case const_Ativar_Usuario:
        //                        {
        //                            sRetorno = "Código de ativação não encontrado";
        //                            break;
        //                        }
        //                    case const_Ativar_Empresa:
        //                        {
        //                            sRetorno = "Código de ativação não encontrado";
        //                            break;
        //                        }
        //                }
        //            }
        //            else
        //            {
        //                Config.oBancoDados.DBExecutar(sSql_Ativacao);

        //                Integradores.Messengers.Propriedade_Carregar();

        //                switch (parametro.Substring(0, 3))
        //                {
        //                    case const_Ativar_Usuario:
        //                        {
        //                            sMensagem = "Usuário ativado" + Environment.NewLine + Environment.NewLine;

        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["Nome"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "Nome do usuário: " + _Funcoes.FNC_NuloString(oData.Rows[0]["Nome"]), Environment.NewLine);
        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["TELEFONE"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "Número do telefone: " + _Funcoes.FNC_NuloString(oData.Rows[0]["TELEFONE"]), Environment.NewLine);
        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["TELEGRAM"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "Código do telegram: " + _Funcoes.FNC_NuloString(oData.Rows[0]["TELEGRAM"]), Environment.NewLine);
        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["CNPJ"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "CNPJ: " + _Funcoes.FNC_NuloString(oData.Rows[0]["CNPJ"]), Environment.NewLine);
        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["Partner"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "Partner: " + _Funcoes.FNC_NuloString(oData.Rows[0]["Partner"]), Environment.NewLine);

        //                            sRetorno = "Usuário " + oData.Rows[0]["Nome"].ToString() + " ativado";
        //                            break;
        //                        }
        //                    case const_Ativar_Empresa:
        //                        {
        //                            sMensagem = "Empresa ativada" + Environment.NewLine + Environment.NewLine;

        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["Empresa"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "Nome da empresa: " + _Funcoes.FNC_NuloString(oData.Rows[0]["Empresa"]), Environment.NewLine);
        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["CNPJ"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "CNPJ: " + _Funcoes.FNC_NuloString(oData.Rows[0]["CNPJ"]), Environment.NewLine);
        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["CodTelefone"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "Número do telefone: " + _Funcoes.FNC_NuloString(oData.Rows[0]["CodTelefone"]), Environment.NewLine);
        //                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["Partner"]) != "")
        //                                _Funcoes.FNC_Str_Adicionar(ref sMensagem, "Partner: " + _Funcoes.FNC_NuloString(oData.Rows[0]["Partner"]), Environment.NewLine);

        //                            sRetorno = "Empresa " + oData.Rows[0]["Empresa"].ToString() + " ativada";
        //                            break;
        //                        }
        //                }
        //            }

        //            Declaracao.oMensagem_log = new Mensagem_log();

        //            if (sMensagem != "")
        //            {
        //                string sTo = "";
        //                string sSql = "";

        //                sSql = "select * from vw_usuario_notificacao" +
        //                       " where Tipo in ('N', 'T') or (Tipo = '" + sTipo + "' and Id = " + iId.ToString() + ")";
        //                oData = Config.oBancoDados.DBQuery(sSql);

        //                Integradores.Mensagem.Agente = "alice";
        //                Integradores.Mensagem.botname = "alice";
        //                Integradores.Mensagem.messagedir = "I";
        //                Integradores.Mensagem.messagercv = _Funcoes.FNC_Data_Atual_DB();
        //                Integradores.Mensagem.messagercvd = DateTime.Now;
        //                Integradores.Mensagem.messagedtm = _Funcoes.FNC_Data_Atual_DB();
        //                Integradores.Mensagem.messagedtmd = DateTime.Now;
        //                Integradores.Mensagem.messagemtd = _Funcoes.FNC_Data_Atual_DB();
        //                Integradores.Mensagem.messagemtdd = DateTime.Now;
        //                Integradores.Mensagem.messagerqt = _Funcoes.FNC_Data_Atual_DB();
        //                Integradores.Mensagem.messagerqtd = DateTime.Now;
        //                Integradores.Mensagem.messagerst = _Funcoes.FNC_Data_Atual_DB();
        //                Integradores.Mensagem.messagerstd = DateTime.Now;
        //                Integradores.Mensagem.EnviarSemTratar = true;

        //                foreach (DataRow oRow in oData.Rows)
        //                {
        //                    if (_Funcoes.FNC_NuloString(oData.Rows[0]["Email"]) != "")
        //                    {
        //                        if (!_Funcoes.FNC_Lista_Existe(sTo, oData.Rows[0]["Email"].ToString()))
        //                            _Funcoes.FNC_Str_Adicionar(ref sTo, oData.Rows[0]["Email"].ToString(), ";");
        //                    }

        //                    if (_Funcoes.FNC_NuloString(oRow["TELEGRAM"]).Trim() != "")
        //                    {
        //                        Integradores.Mensagem.To = oRow["TELEGRAM"].ToString();
        //                        Integradores.Mensagem.contactname = _Funcoes.FNC_NuloString(oRow["Nome"].ToString()).Trim();
        //                        Integradores.Mensagem.messagebody = sMensagem;
        //                        Integradores.Config.Provider = "TG";
        //                        Integradores.Messengers.EnviarMensagem(sMensagem);
        //                    }
        //                    if (_Funcoes.FNC_NuloString(oRow["TELEFONE"]).Trim() != "")
        //                    {
        //                        Integradores.Mensagem.contactuid = oRow["TELEFONE"].ToString();
        //                        Integradores.Mensagem.To = oRow["TELEFONE"].ToString();
        //                        Integradores.Mensagem.contactname = _Funcoes.FNC_NuloString(oRow["Nome"].ToString()).Trim();
        //                        Integradores.Mensagem.messagebody = sMensagem;
        //                        Integradores.Config.Provider = "ZP";
        //                        Integradores.Messengers.EnviarMensagem(sMensagem);
        //                    }
        //                }

        //                if (sTo.Trim() != "")
        //                {
        //                    switch (parametro.Substring(0, 3))
        //                    {
        //                        case const_Ativar_Usuario:
        //                            {
        //                                Integradores.Messengers.FNC_Enviar(sTo, "", "Ativação de cadastro de usuário", sMensagem);
        //                                break;
        //                            }
        //                        case const_Ativar_Empresa:
        //                            {
        //                                Integradores.Messengers.FNC_Enviar(sTo, "", "Ativação de cadastro de empresa", sMensagem);
        //                                break;
        //                            }
        //                    }
        //                }
        //            }

        //            Config.oBancoDados.DBDesconectar();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    JObject errresponse = new JObject();
        //    errresponse.Add("Error", sRetorno);
        //    //return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
        //    return req.CreateResponse(HttpStatusCode.OK, sRetorno);
        //}

        [FunctionName("MessageText")]
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
