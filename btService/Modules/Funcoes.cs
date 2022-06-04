using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace btService.Modules
{
    public static class Funcoes
    {
        public static string Webook_Util(int Solicitacao, string Servico, string Termo, string Mensagem, string Provider, string USUARIO, string Para, string Botname)
        {
            string sWebHook_Url = "";
            string sErro = "";

            sWebHook_Url = "http://localhost:7071/api";
            sWebHook_Url = "http://a94f119e.ngrok.io/api";
            sWebHook_Url = "http://plugthink.azurewebsites.net/api";

            try
            {
                sWebHook_Url = sWebHook_Url.Trim();
                if (sWebHook_Url.Substring(sWebHook_Url.Length - 1, 1) != "/")
                    sWebHook_Url = sWebHook_Url.Trim() + "/";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(sWebHook_Url + "MessageWebHook_Util"));

                request.ContentType = "application/json";
                request.Method = "POST";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        Solicitacao = Solicitacao,
                        Chave = "x3|b0$/; 0KvpP34%WUl|qN!|U~$OPbco`elYQGuN(gs(A#]0A!",
                        Provider = Provider,
                        Servico = Servico,
                        Termo = Termo,
                        Mensagem = Mensagem,
                        Para = Para,
                        Contact_Name = USUARIO,
                        Botname = Botname
                    });
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                sErro = "Ok";
            }
            catch (Exception Ex)
            {
                sErro = Ex.Message;
            }

            return sErro;
        }

        public static string FNC_FormatarTelefone(string sTelefone)
        {
            sTelefone = sTelefone.Trim().Replace("-", "").Replace("(", "").Replace(")", "");

            if ((sTelefone.Length == 10) || (sTelefone.Length == 9))
            {
                if (sTelefone.Substring(0, 2) != "55")
                {
                    sTelefone = "55" + sTelefone;
                }
            }

            if (sTelefone.Length == 12)
            {
                sTelefone = sTelefone.Substring(0, 4) + "9" + sTelefone.Substring(4);
            }

            return sTelefone;
        }
    }
}