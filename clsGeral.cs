using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TelegramBot;
using static Integradores.Entidades;

namespace Integradores
{
    public enum LogTipo
    {
        ArquivoImportadoComSucesso = 50000,
        ArquivoNaoEncontrado = 50001,
        ErroImportarQuantidadeRegistrosInvalida = 50002,
        ErroNoBancoDados = 50500,
        ErroNaRotina_ImportaEstoque = 50501,
        ErroNaRotina_ImportaPreco = 50502,
        ErroLeituraTarefas = 50503,
        ErroNaRotina_FlagWSWhatsapp_DisDevolucao = 50504,
        ErroNaRotina_FlagWSWhatsapp_DisTroca = 50505,
        ErroNaRotina_Sofia = 50506
    }

    public static class Constantes
    {
        public const string const_Sistema_Nome = "mrSoft - trade2UP - Integrador";

        public const int const_TipoIntegracao_1_PedFacil = 1;
        public const int const_TipoIntegracao_2_Boomerangue = 2;
        public const int const_TipoIntegracao_3_Lokalizei = 3;
        public const int const_TipoIntegracao_4_MeLeva = 4;
        public const int const_TipoIntegracao_5_DashPlus = 5;
        public const int const_TipoIntegracao_6_FlexxPower = 6;
        public const int const_TipoIntegracao_7_PedFacilREP = 7;
        public const int const_TipoIntegracao_8_B2BWeb = 8;
        public const int const_TipoIntegracao_9_FlagWSWhatsapp_DisDevolucao = 9;
        public const int const_TipoIntegracao_10_FlagWSWhatsapp_DisTroca = 10;
        public const int const_TipoIntegracao_11_FlagWSWhatsapp_DisIav_iv = 11;
        public const int const_TipoIntegracao_12_FlagWSWhatsapp_DisInadimplencia = 12;
        public const int const_TipoIntegracao_13_FlagWSWhatsapp_DisLogDevolucao = 13;
        public const int const_TipoIntegracao_14_FlagWSWhatsapp_DisLogDespersaoRota = 14;
        public const int const_TipoIntegracao_15_SofiaChat = 15;

        public const string const_Template_Command_SEMACESSO = "SEMACESSO";
        public const string const_Template_Command_OFFLINE = "OFFLINE";
        public const string const_Template_Command_AJUDAERRO = "AJUDAERRO";
        public const string const_Template_Command_ERRO998 = "ERRO998";
        public const string const_Template_Command_SERVICOFORA = "SERVICOFORA";
        public const string const_Template_Command_OLA = "OLA";
        public const string const_Template_Command_NOTIFICACAOCONFIRMADA = "NOTIFICACAOCONFIRMADA";
        public const string const_Template_Command_CONFIRMARNOTIFICACAO = "CONFIRMARNOTIFICACAO";
        public const string const_Template_Command_HNK_COBRANCA_TESTE = "HNK_COBRANCA_TESTE";
        public const string const_Template_Command_BOOMERANGUE_PURINA = "BOOMERANGUE_PURINA";

        public const int const_Servico_ID_SofiaPesquisas = 16;
        public const int const_Servico_ID_BoomeranguePurina = 17;

        public const string const_Servico_SofiaPesquisas = "SPQ";
        public const string const_Servico_BoomeranguePurina = "BPU";

        public const string const_TipoBancoDados_MySql = "MYSQL";
        public const string const_TipoBancoDados_SqlServer = "SQLSRV";

        public const string const_Mensagem_Log_Mensagem = "Mensagem";
        public const string const_Mensagem_Log_Mensagem_ChatAPI = "ChatAPI";
        public const string const_Mensagem_Log_Mensagem_Waboxapp = "Waboxapp";
        public const string const_Mensagem_Log_BancoDados = "BancoDados";
        public const string const_Mensagem_Log_BancoDados_Tems = "BancoDados_Tems";
        public const string const_Mensagem_Log_ProviderFlexXTools = "Provider_FlexXTools";

        public const string const_Provider_Waboxapp = "WA";
        public const string const_Provider_Telegram = "TG";
        public const string const_Provider_ChartAPI = "CA";
        public const string const_Provider_WhatsApp = "ZP";
        public const string const_Provider_PickyAssit = "PA";
        public const string const_Provider_BTrive = "BT";

        public const string const_Comando_Broadcast = "Broadcast";

        public const string const_Comando_Geral_Ativar = "/ativar";
        public const string const_Comando_Geral_Query = "*";

        public const string const_Comando_BotHelp = "/bot.help";
        public const string const_Comando_BotAtivar = "/bot.ativar";
        public const string const_Comando_BotCadastrar = "/bot.cadastrar";
        public const string const_Comando_BotDescadastrar = "/bot.descadastrar";
        public const string const_Comando_BotListarServicos = "/bot.ListarServicos";
        public const string const_Comando_BotConsultarCliente = "/bot.ConsultarCliente";

        public const string const_Comando_BotCobranca = "/cobranca";

        public const string const_Comando_DashHelp = "/dash.help";
        public const string const_Comando_DashAtivar = "/dash.ativar";
        public const string const_Comando_DashFeira = "/feira";

        public const string const_Comando_OlaPdvMenu = "/olapdv.menu";
        public const string const_Comando_OlaPdvIntegra = "/olapdv.integra";
        public const string const_Comando_OlaPdvAjuda = "/olapdv.ajuda";

        public const string const_Comando_Pesquisa = "/pesquisa";
        public const string const_Comando_Boomerangue = "/boomerangue ";

        public const string const_Status_PendenteConfirmacao = "PC";
        public const string const_Status_Cancelado = "CN";
        public const string const_Status_Enviado = "EV";

        public const string const_BotServico_Sistema_KITEI = "KTI";
        public const string const_BotServico_Sistema_KITEI_OLD = "KITEI_OLD";

        public const string FlexXTools_Usuario = "flagwhats4280en";
        public const string FlexXTools_Senha = "Odfofot59flag2344959";

        public const int const_TabelasAuxiliares_Quantidade = 7;
        public const int const_TabelasAuxiliares_TabelaView_Bot = 0;
        public const int const_TabelasAuxiliares_TabelaView_Message = 1;
        public const int const_TabelasAuxiliares_TabelaView_Empresa = 2;
        public const int const_TabelasAuxiliares_TabelaView_Usuario = 3;
        public const int const_TabelasAuxiliares_TabelaView_Query = 4;
        public const int const_TabelasAuxiliares_TabelaView_ultMensagem = 5;
        public const int const_TabelasAuxiliares_TabelaView_Servico = 6;

        public const int const_Acao_Boomerangue_ConfirmarEnvio = 1;
        public const int const_Acao_Boomerangue_ConfirmarAceito = 2;

        public const int const_Processo_Pesquisa = 1;
        public const int const_Processo_Boomerangue = 2;
        public const int const_Processo_MenuGeral = 3;
    }

    public class Config
    {
        public string dbconstring = "";
        public string tipobancodados = "";

        public string Chat_Url = "";

        public string ChatImage_Url = "";

        public string Provider { get; set; }

        public bool NaoValidarLicenca { get; set; }
        public bool UsuarioAdminValido { get; set; }

        public clsBancoDados oBancoDados = new clsBancoDados();
        public clsBancoDados oBancoDadosBot = new clsBancoDados();

        public void Carregar(JObject Json, string sBot = "")
        {
            try
            {
                dbconstring = Json[sBot.ToLower().Trim()]["conectionstring"].ToString();
                tipobancodados = Json[sBot.ToLower().Trim()]["tipobancodados"].ToString();
            }
            catch (Exception)
            {
                dbconstring = Json["Config"]["conectionstring"].ToString();
                tipobancodados = Json["Config"]["tipobancodados"].ToString();
            }

            //tipobancodados = Constantes.const_TipoBancoDados_MySql;
            // dbconstring = "Server=167.172.193.108;Port=3306;Database=i9ativa;Uid=dbafabricio;Pwd=gQIp1b1JGNG6PLn0;SslMode=none";
        }
    }

    public class Erro
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
    }

    public class Mensagem
    {
        public const int const_Erro_BoomeangueEnviado = 10000;
        public const int const_Erro_SemBoomeangue = 90000;
        public const int const_Erro_ErroBoomeangue = -10000;

        public DataTable[] oTabela;
        public string CodPuxada { get; set; }
        public string events { get; set; }
        public string contactuid { get; set; }
        public string contactname { get; set; }
        public string contacttype { get; set; }
        public DateTime messagedtm { get; set; }
        public DateTime messagedtmd { get; set; }
        public DateTime messagercv { get; set; }
        public DateTime messagercvd { get; set; }
        public DateTime messagemtd { get; set; }
        public DateTime messagemtdd { get; set; }
        public DateTime messagerqt { get; set; }
        public DateTime messagerqtd { get; set; }
        public DateTime messagerst { get; set; }
        public DateTime messagerstd { get; set; }
        public string messageuid { get; set; }
        public string messagecuid { get; set; }
        public string messagedir { get; set; }
        public string messagetype { get; set; }
        public string messagebody { get; set; }
        public string messagefile { get; set; }
        public string messagebody_response { get; set; }
        public string messageack { get; set; }
        public int idMessageTerms { get; set; }
        public string command { get; set; }
        public string status { get; set; }
        public string ParaLista { get; set; }
        public string BotNameResposta { get; set; }
        public string MensagemFinal { get; set; }
        public string Agente { get; set; }
        public string Token { get; set; }
        public string Token2 { get; set; }
        public string Uid { get; set; }
        public string To { get; set; }
        public bool ToDirect { get; set; }
        public string Usuario { get; set; }

        public long idMensagem { get; set; }
        public Int32 idStatusMensagem { get; set; }
        public Int32 idBot { get; set; }
        public Int32 idBot_nroOrigem { get; set; }
        public string idBotExterno { get; set; }
        public Int32 idServico { get; set; }
        public string botname { get; set; }
        public string Servico { get; set; }
        public string Termo { get; set; }
        public bool EnviarSemTratar { get; set; }
        public bool EnvioInterno { get; set; }

        public bool MessageWebHook_Util { get; set; }
        public long idTarefa { get; set; }

        public Int32 idbot_requisicao;
        public string Custom_uid { get; set; }
        public Boolean SemServico { get; set; }
        public cls_tbmessageterms.cls_messageterms Terms { get; set; }

        public string dsUrl_Image { get; set; }

        public object CampoAuxiliar_01 { get; set; }
        public object CampoAuxiliar_02 { get; set; }

        public int TipoAcao { get; set; }

        public string instanceId { get; set; }

        public string chat_status { get; set; }

        public bool ReprocessarTerms { get; set; }

        public Erro Erro { set; get; }

        public void InformarErro(int Codigo, string Descricao)
        {
            if (Erro == null) { Erro = new Erro(); };

            Erro.Codigo = Codigo;
            Erro.Descricao = Descricao;
        }
    }

    public class Mensagem_log
    {
        public class Item
        {
            public string messageuid { get; set; }
            public int message_log_ordem { get; set; }
            public DateTime message_log_evento { get; set; }
            public string message_log_processo { get; set; }
            public string message_log_comentario { get; set; }
        }

        public Item[] Itens;
        public int Itens_Qtde = 0;

        public void Adicionar(ref Mensagem oMensagem, string _message_log_processo, string _message_log_comentario = "")
        {
            Itens_Qtde++;

            Array.Resize(ref Itens, Itens_Qtde);

            Itens[Itens_Qtde - 1] = new Item()
            {
                messageuid = oMensagem.messageuid,
                message_log_ordem = Itens_Qtde,
                message_log_evento = DateTime.Now,
                message_log_processo = _message_log_processo,
                message_log_comentario = _message_log_comentario
            };
        }
    }

    public class Entidades
    {
        public class DadosDISTSfVenda
        {
            public string COD_PUXADA { get; set; }
            public double VALOR_HECTO { get; set; }
            public double VALOR_VENDA { get; set; }
        }

        public class DadosDISTSfVendaRoot
        {
            public List<DadosDISTSfVenda> DadosDISTSfVenda { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public class DadosDISTSfCoberturaRoot
        {
            public List<DadosDISTSfCobertura> DadosDISTSfCobertura { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public class DadosDISTSfCobertura
        {
            public int COD_GRUPO { get; set; }
            public string COD_PUXADA { get; set; }
            public string DSC_GRUPO { get; set; }
            public double QTDE_COBERTURA { get; set; }
        }

        public class DadosDISTSfVendasGrupoRoot
        {
            public List<DadosDISTSfVendasGrupo> DadosDISTSfVendas_Grupo { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public class DadosDISTSfVendasGrupo
        {
            public int COD_GRUPO { get; set; }
            public string COD_PUXADA { get; set; }
            public string DSC_GRUPO { get; set; }
            public double QTDE_VOLUME { get; set; }
            public double VALOR_HECTO { get; set; }
            public double VALOR_VENDA { get; set; }
        }

        public class DadosDISTSfIVIAVRoot
        {
            public List<DadosDISTSfIVIAV> DadosDISTSfIV_IAV { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public class DadosDISTSfIVIAV
        {
            public string COD_PUXADA { get; set; }
            public double PERCENTUAL_IAV { get; set; }
            public double PERCENTUAL_IV { get; set; }
            public double QTDE_IAV { get; set; }
            public double QTDE_IV { get; set; }
            public double QTDE_VISITA_PREVISTA { get; set; }
        }

        public class DadosDISTSfDevolucaoRoot
        {
            public List<DadosDISTSfDevolucao> DadosDISTSfDevolucao { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public class DadosDISTSfDevolucao
        {
            public string COD_PUXADA { get; set; }
            public double PERCENTUAL_DEVOLUCAO { get; set; }
            public double VALOR_DEVOLUCAO { get; set; }
        }

        public class DadosDISTSfDocumentosaVencer
        {
            public int CODIGO_CLASSE { get; set; }
            public int CODIGO_CLIENTE { get; set; }
            public string COD_PUXADA { get; set; }
            public string DATA_VENCIMENTO { get; set; }
            public int DDD_CELULAR { get; set; }
            public string DESCRICAO_CLASSE { get; set; }
            public int DIA_A_VENCER { get; set; }
            public string DOCUMENTO { get; set; }
            public string DOCUMENTO_ORIGEM { get; set; }
            public string E_MAIL { get; set; }
            public string LINHA_DIGITAVEL { get; set; }
            public string NOME_FANTASIA { get; set; }
            public int NUMERO_CELULAR { get; set; }
            public string RAZAO_SOCIAL { get; set; }
            public double VALOR_DOCUMENTO { get; set; }
        }

        public class DadosDISTSfDocumentosaVencerRoot
        {
            public List<DadosDISTSfDocumentosaVencer> DadosDISTSfDocumentosaVencer { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public class WaboxApp_WhatsApp
        {
            public bool success { get; set; }
            public string custom_uid { get; set; }
        }
    }

    public static class Declaracao
    {
        public static Mensagem_log oMensagem_log;
        public static cls_tbmessageterms otbmessageterms = new cls_tbmessageterms();

        public static string processador = "";

        public static string ErroMensagem = "";

        public static double REMUNERACAO_TOTAL;
        public static double DROP_TOTAL;

        public static string[] Mensagens = new string[200];

        private static List<string> oControle_Requisicao;

        public static bool Controle_Requisicao_Validar(string sChave)
        {
            bool Valido = true;

            if (oControle_Requisicao == null) { oControle_Requisicao = new List<string>(); };

            if (oControle_Requisicao.IndexOf(sChave) == -1)
            {
                oControle_Requisicao.Add(sChave);
            }
            else
            {
                Valido = false;
            }

            return Valido;
        }

        public static bool Mensagem_Validar(string sChave)
        {
            bool Achou = false;

            foreach (string sLista in Mensagens)
            {
                if (_Funcoes.FNC_NuloString(sLista) == "")
                {
                    break;
                }
                else
                {
                    if (sLista.Trim().ToUpper() == sChave.Trim().ToUpper())
                    {
                        Achou = true;
                        break;
                    }
                }
            }

            if (!Achou)
            {
                try
                {
                    for (int i = Mensagens.Length - 1; i > 0; i--)
                    {
                        try
                        {
                            Mensagens[i] = Mensagens[i - 1];
                        }
                        catch (Exception)
                        {
                            Achou = Achou;
                        }
                    }
                }
                catch (Exception)
                {
                }

                Mensagens[0] = sChave;
            }

            return Achou;
        }
    }

    public class JsonHelper
    {
        private const int INDENT_SIZE = 4;

        public static string FormatJson(string str)
        {
            str = (str ?? "").Replace("{}", @"\{\}").Replace("[]", @"\[\]");

            var inserts = new List<int[]>();
            bool quoted = false, escape = false;
            int depth = 0/*-1*/;

            for (int i = 0, N = str.Length; i < N; i++)
            {
                var chr = str[i];

                if (!escape && !quoted)
                    switch (chr)
                    {
                        case '{':
                        case '[':
                            inserts.Add(new[] { i, +1, 0, INDENT_SIZE * ++depth });
                            //int n = (i == 0 || "{[,".Contains(str[i - 1])) ? 0 : -1;
                            //inserts.Add(new[] { i, n, INDENT_SIZE * ++depth * -n, INDENT_SIZE - 1 });
                            break;
                        case ',':
                            inserts.Add(new[] { i, +1, 0, INDENT_SIZE * depth });
                            //inserts.Add(new[] { i, -1, INDENT_SIZE * depth, INDENT_SIZE - 1 });
                            break;
                        case '}':
                        case ']':
                            inserts.Add(new[] { i, -1, INDENT_SIZE * --depth, 0 });
                            //inserts.Add(new[] { i, -1, INDENT_SIZE * depth--, 0 });
                            break;
                        case ':':
                            inserts.Add(new[] { i, 0, 1, 1 });
                            break;
                    }

                quoted = (chr == '"') ? !quoted : quoted;
                escape = (chr == '\\') ? !escape : false;
            }

            if (inserts.Count > 0)
            {
                var sb = new System.Text.StringBuilder(str.Length * 2);

                int lastIndex = 0;
                foreach (var insert in inserts)
                {
                    int index = insert[0], before = insert[2], after = insert[3];
                    bool nlBefore = (insert[1] == -1), nlAfter = (insert[1] == +1);

                    sb.Append(str.Substring(lastIndex, index - lastIndex));

                    if (nlBefore) sb.AppendLine();
                    if (before > 0) sb.Append(new String(' ', before));

                    sb.Append(str[index]);

                    if (nlAfter) sb.AppendLine();
                    if (after > 0) sb.Append(new String(' ', after));

                    lastIndex = index + 1;
                }

                str = sb.ToString();
            }

            return str.Replace(@"\{\}", "{}").Replace(@"\[\]", "[]");
        }

        public static object Procurar(Dictionary<string, JToken> dict, string Chave)
        {
            object Valor = null;

            foreach (KeyValuePair<string, JToken> Item in dict)
            {
                if (Item.Key == Chave)
                {
                    Valor = Item.Value;
                    break;
                }
            }

            return Valor;
        }
    }

    public class cls_tbmessageterms
    {
        public class cls_messageterms
        {
            public int idMessageTerms { get; set; }
            public int idMessageTermsPai { get; set; }
            public string Cod_Puxada { get; set; }
            public string Agente { get; set; }
            public string Command { get; set; }

            public int nrOrdem { get; set; }
            public string Servico { get; set; }
            public string TipoComando { get; set; }
            public string dsTerms { get; set; }
            public string dsTemplateHeader { get; set; }
            public string dsTemplate { get; set; }
            public string dsTemplateFooter { get; set; }

            public string dsUrl_Image { get; set; }
            public string StringConexao { get; set; }
            public string TipoBancoDados { get; set; }
            public string Query_Tipo { get; set; }
            public string Query_String { get; set; }
            public string WS_TipoRegistro { get; set; }
            public string WS_TipoConsulta { get; set; }
            public string WS_JsonParametros { get; set; }

            public int WS_IntervaloConsulta { get; set; }
            public int WS_VezesAoDia { get; set; }
            public int WS_TempoEspera { get; set; }
            public string WS_TipoRetorno { get; set; }
            public string WS_TipoRetornoCampo { get; set; }
            public string WS_TipoRetornoRespostas { get; set; }
            public string WS_TipoRetornoScript { get; set; }
            public string WS_TipoRetornoTipo { get; set; }
            public int WS_ComprimentoMin { get; set; }
            public int WS_CampoObrigatorio { get; set; }
            public string WS_MetodoValidarUsuario { get; set; }

            public Boolean SepararMensagens { get; set; }
            public int WS_ProximaMensagem { get; set; }
        }

        public cls_messageterms[] messageterms;

        public cls_messageterms IdentificarTermo(ref Config oConfig, ref Mensagem oMensagem, string sExpressao)
        {
            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_BancoDados_Tems, "Identificando");

            cls_messageterms Ret = null;

            Ret = PesquisarPorTermo(ref oConfig, ref oMensagem, sExpressao);

            if (Ret == null)
                Ret = PesquisarPorTermo(ref oConfig, ref oMensagem, Constantes.const_Template_Command_AJUDAERRO, true);

            if (Ret == null)
                Ret = PesquisarPorTermo(ref oConfig, ref oMensagem, Constantes.const_Template_Command_SEMACESSO, true);

            if (Ret == null)
                Ret = PesquisarPorTermo(ref oConfig, ref oMensagem, Constantes.const_Template_Command_SEMACESSO, true);

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_BancoDados_Tems, "Identificado");

            return Ret;
        }

        public cls_messageterms PesquisarPorId(int idMessageTerms)
        {
            cls_messageterms Ret = null;

            foreach (cls_messageterms Item in messageterms)
            {
                if (Item.idMessageTerms == idMessageTerms)
                {
                    Ret = Item;
                    break;
                }
            }

            return Ret;
        }

        public cls_messageterms PesquisarPorTermo(ref Config oConfig,
                                                  ref Mensagem oMensagem,
                                                  string sExpressao,
                                                  Boolean PorComnado = false,
                                                  string sCod_Puxada = "",
                                                  string sServico = "",
                                                  Boolean SemServico = false)
        {
            cls_messageterms Ret = null;
            bool Valido = false;
            DataTable oData = null;
            int idMessageTermsPai = 0;

            if (sServico.Trim() == "" && _Funcoes.FNC_NuloString(oMensagem.Servico).Trim() != "")
            {
                sServico = oMensagem.Servico;
            }

            foreach (cls_messageterms Item in messageterms)
            {
                if (Item.idMessageTerms == 69)
                {
                    Valido = Valido;
                }

                if ((_Funcoes.FNC_NuloString(Item.Agente).Trim().ToUpper() == oMensagem.Agente.Trim().ToUpper() ||
                     _Funcoes.FNC_NuloString(Item.Agente).Trim().ToUpper() == "<TODOS>") &&
                    (_Funcoes.FNC_NuloString(Item.Cod_Puxada).Trim().ToUpper() == sCod_Puxada.Trim().ToUpper() ||
                     _Funcoes.FNC_NuloString(Item.Cod_Puxada).Trim().ToUpper() == "<TODAS>" || sCod_Puxada == "") &&
                   (((_Funcoes.FNC_NuloString(Item.Servico).Trim().ToUpper() == sServico.Trim().ToUpper() || sServico == "") && !SemServico) ||
                    (_Funcoes.FNC_NuloString(Item.Servico).Trim().ToUpper() == "" && SemServico) ||
                    (_Funcoes.FNC_NuloString(Item.Servico).Trim().ToUpper() == "TDS")))
                {
                    Valido = false;

                    if (Item.idMessageTermsPai != 0)
                    {
                        if (idMessageTermsPai == 0)
                        {
                            oData = oConfig.oBancoDadosBot.DBQuery("select * from vw_message_ultmsg_contact_i where contact_uid = '" + _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid.Trim()) + "'");

                            if (!_Funcoes.FNC_Data_Vazio(oData))
                            {
                                idMessageTermsPai = Convert.ToInt32(oData.Rows[0]["idMessageTerms"]);
                            }
                        }

                        if (idMessageTermsPai == Item.idMessageTermsPai)
                        {
                            Valido = true;
                        }
                    }
                    else
                    {
                        Valido = true;
                    }

                    if (Valido)
                    {
                        if (PorComnado)
                        {
                            if (Item.Command.Trim().ToUpper() == sExpressao.Trim().ToUpper())
                            {
                                Ret = Item;
                                break;
                            }
                        }
                        else
                        {
                            if (Item.nrOrdem == 0)
                            {
                                Ret = Item;
                                break;
                            }
                            else
                            {
                                string[] Termos = Item.dsTerms.Split(new char[] { ';' });

                                foreach (string Termo in Termos)
                                {
                                    if (Termo.ToUpper().IndexOf(sExpressao.ToUpper().Trim()) > -1)
                                    {
                                        Ret = Item;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (Ret != null)
                        break;
                }

            }

            return Ret;
        }
    }

    public static class _Funcoes
    {
        public static string Replace(string sTexto, string sCampo, object Valor)
        {
            try
            {
                int iIndice = sTexto.IndexOf(sCampo.Trim() + "]");
                string sValor = "";
                decimal dValor = 0;

                if (iIndice > -1)
                {
                    for (int i = iIndice - 1; i >= 0; i--)
                    {
                        if (sTexto.Substring(i, 1) == "[")
                        {
                            sCampo = sTexto.Substring(i, iIndice + sCampo.Length - i + 1);

                            string[] sSeparador = sCampo.Substring(1, sCampo.Length - 2).Replace("||", "|").Split(new char[] { '|' });

                            //Divisão
                            if (sSeparador[1].Trim() != "-")
                            {
                                dValor = (Convert.ToDecimal(Valor) / Convert.ToInt32(sSeparador[1].Substring(1)));
                            }
                            else
                            {
                                if (_Funcoes.FNC_IsNumeric(Valor.ToString()))
                                {
                                    dValor = Convert.ToDecimal(Valor);
                                }
                            }
                            //Formatação
                            if (sSeparador[0].Trim() != "-")
                            {
                                if (_Funcoes.FNC_IsNumeric(Valor.ToString()))
                                {
                                    sValor = dValor.ToString(sSeparador[0].ToString());
                                }
                                else if (FNC_Data_Valida(Valor.ToString()))
                                {
                                    sValor = Convert.ToDateTime(Valor).ToString(sSeparador[0]);
                                }
                                else
                                {
                                    sValor = Valor.ToString();
                                }
                            }
                            else
                            {
                                if (_Funcoes.FNC_IsNumeric(Valor.ToString()))
                                {
                                    sValor = dValor.ToString();
                                }
                                else
                                {
                                    sValor = Valor.ToString();
                                }
                            }

                            sTexto = sTexto.Replace(sCampo, sValor);

                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return sTexto;
        }

        public static bool FNC_Data_Vazio(DataTable oData)
        {
            bool bOk = false;

            if (oData != null)
            {
                bOk = (oData.Rows.Count == 0);
            }

            return bOk;
        }

        public static string FNC_FormatarTelefone(string sTelefone, bool FormatoAntigo = false)
        {
            sTelefone = sTelefone.Trim().Replace("-", "").Replace("(", "").Replace(")", "");

            if ((sTelefone.Length == 10) || (sTelefone.Length == 9))
            {
                if (sTelefone.Substring(0, 2) != "55")
                {
                    sTelefone = "55" + sTelefone;
                }
            }
            else
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

            if (FormatoAntigo)
            {
                sTelefone = sTelefone.Substring(0, 4) + sTelefone.Substring(5);
            }

            return sTelefone;
        }

        public static string FNC_Senha_Gerar(int TamanhoDaSenha)
        {
            string sSenha = "";
            //string validar = "abcdefghijklmnozABCDEFGHIJKLMNOZ1234567890";
            string validar = "1234567890";
            try
            {
                StringBuilder strbld = new StringBuilder(100);
                Random random = new Random();
                while (0 < TamanhoDaSenha--)
                {
                    strbld.Append(validar[random.Next(validar.Length)]);
                }
                sSenha = strbld.ToString();
            }
            catch (Exception ex)
            {
            }

            return sSenha;
        }

        public static string FNC_Diretorio_Tratar(string sPath)
        {
            if (FNC_Right(sPath.Trim(), 1) != @"\")
                return sPath.Trim() + @"\";
            else
                return sPath.Trim();
        }

        public static string FNC_Right(string sTexto, int Tamanho)
        {
            string sRet;

            sRet = sTexto.Trim();

            if (sRet.Length > Tamanho)
            {
                sRet = sRet.Substring(sRet.Length - Tamanho);
            }

            return sRet;
        }

        public static bool FNC_IsHour(string sHora)
        {
            try
            {
                if (sHora.Length == 5)
                {
                    if (sHora.Trim().Substring(2, 1) == ":")
                    {
                        if ((Convert.ToInt32(sHora.Trim().Substring(0, 2)) >= 0) && (Convert.ToInt32(sHora.Trim().Substring(0, 2)) <= 23))
                        {
                            if ((Convert.ToInt32(sHora.Trim().Substring(3, 2)) >= 0) && (Convert.ToInt32(sHora.Trim().Substring(3, 2)) <= 59))
                            {
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static string FNC_SoNumero(string sValor)
        {
            int iCont;
            string sAux = "";

            for (iCont = 1; (iCont <= sValor.Length); iCont++)
            {
                if (FNC_IsNumeric(sValor.Substring((iCont - 1), 1)))
                {
                    sAux = (sAux + sValor.Substring((iCont - 1), 1));
                }
            }

            return sAux;
        }

        public static bool FNC_IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static double FNC_StringValor(string sValor)
        {
            sValor = sValor.Replace(",", ".");

            return Convert.ToDouble(sValor);
        }

        public static double FNC_NuloZero(object Valor)
        {
            if (Valor == null)
                return 0;
            else
            {
                if (FNC_IsNumeric(Valor.ToString()))
                {
                    Valor = Valor.ToString().Replace(",", "").Replace(".", ",");

                    return Convert.ToDouble(Valor);
                }
                else
                    return 0;
            }
        }

        public static string FNC_NuloString(object Valor)
        {
            if (Valor == null)
                return "";
            else
            {
                return Valor.ToString();
            }
        }

        public static decimal FNC_NuloZeroDecimal(object Valor)
        {
            if (Valor == null)
                return 0;
            else
            {
                if (FNC_IsNumeric(Valor.ToString()))
                {
                    return Convert.ToDecimal(Valor);
                }
                else
                    return 0;
            }
        }

        public static string FNC_NomeArquivo(string sNome, string sBarra)
        {
            string sRet = "";
            int iCont = 0;
            int iIndice = 0;

            for (int i = 0; i < sNome.Length - 1; i++)
            {
                if (sNome.Substring(i, 1) == sBarra)
                    iIndice = i;
            }

            sRet = sNome.Substring(iIndice + 1);

            return sRet;
        }

        public static string FNC_CPFCNPJ_Formatar(string sCPFCNPJ)
        {
            if (sCPFCNPJ.Length <= 11)
            {
                MaskedTextProvider mtpCpf = new MaskedTextProvider(@"000\.000\.000-00");
                mtpCpf.Set(FNC_ZerosEsquerda(sCPFCNPJ, 11));
                return mtpCpf.ToString();
            }
            else
            {
                MaskedTextProvider mtpCnpj = new MaskedTextProvider(@"00\.000\.000/0000-00");
                mtpCnpj.Set(FNC_ZerosEsquerda(sCPFCNPJ, 11));
                return mtpCnpj.ToString();
            }
        }

        public static string FNC_ZerosEsquerda(string strString, int intTamanho)
        {
            string strResult = "";

            strString = strString.Trim();

            for (int intCont = 1; intCont <= (intTamanho - strString.Length); intCont++)
            {
                strResult += "0";
            }
            return strResult + strString;
        }

        public static string FNC_Lista_ColocarAspar(string sLista)
        {
            string[] Lista = null;
            string sRet = "";

            if (sLista.Trim() != "")
            {
                if (sLista.Contains(","))
                {
                    Lista = sLista.Split(',');
                }
                else
                {
                    Lista = new string[] { sLista };
                }
            }

            foreach (string Item in Lista)
            {
                FNC_Str_Adicionar(ref sRet, "'" + Item.Trim() + "'", ",");
            }

            return sRet;
        }

        public static bool FNC_Lista_Existe(string sLista, string sItem)
        {
            bool bOk = false;

            if (sLista.Trim() != "")
            {
                if (sLista.Contains(";"))
                {
                    if (sLista.Trim().ToUpper() == sItem.Trim().ToUpper())
                    {
                        bOk = true;
                    }
                }
                else
                {
                    foreach (string Item in sLista.Split(';'))
                    {
                        if (Item.Trim().ToUpper() == sItem.Trim().ToUpper())
                        {
                            bOk = true;
                            break;
                        }
                    }
                }
            }

            return bOk;
        }

        public static void FNC_Str_Adicionar(ref string vVariavel, string sValor, string sSeparador)
        {
            if ((sValor.Trim() != ""))
            {
                if ((vVariavel.Trim() != ""))
                {
                    vVariavel = (vVariavel + sSeparador);
                }

                vVariavel = (vVariavel + sValor);
            }

        }

        public static bool FNC_Data_Valida(string data)
        {
            //Regex r = new Regex(@"(\d{2}\/\d{2}\/\d{4} \d{2}:\d{2})");
            //return r.Match(data).Success;

            try
            {
                Convert.ToDateTime(data);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string FNC_Data_Atual_DB()
        {
            string Data = "";

            Data = FNC_Data_DB(DateTime.Now);

            return Data;
        }

        public static string FNC_Data_DB(DateTime Date)
        {
            string Data = "";

            Data = Date.Year.ToString() + "-" +
                   FNC_ZerosEsquerda(Date.Month.ToString(), 2) + "-" +
                   FNC_ZerosEsquerda(Date.Day.ToString(), 2) + " " +
                   FNC_ZerosEsquerda(Date.Hour.ToString(), 2) + ":" +
                   FNC_ZerosEsquerda(Date.Minute.ToString(), 2) + ":" +
                   FNC_ZerosEsquerda(Date.Second.ToString(), 2);

            return Data;
        }
    }

    public class clsCampo
    {
        public string Nome { get; set; }
        public DbType Tipo { get; set; }
        public object Valor { get; set; }
        public System.Data.ParameterDirection Direcao { get; set; }
    }

    public class clsCampo_Retorno
    {
        public clsCampo[] ParametroRetorno;

        public clsCampo Campo(string Nome)
        {
            clsCampo oRetorno = null;

            foreach (clsCampo Parametro in ParametroRetorno)
            {
                if (Parametro.Nome == Nome)
                {
                    oRetorno = Parametro;
                    break;
                }
            }

            return oRetorno;
        }
    }

    public class clsBancoDados
    {
        object oConexao;
        string sTipoBancoDados;
        string[] SqlTexte_Executada = new string[0];

        public clsCampo_Retorno Retorno
        {
            get
            {
                switch (sTipoBancoDados)
                {
                    case Constantes.const_TipoBancoDados_MySql:
                        {
                            return ((clsMySql)oConexao).Retorno;
                        }
                    case Constantes.const_TipoBancoDados_SqlServer:
                        {
                            return ((clsSqlServer)oConexao).Retorno;
                        }
                    default:
                        return null;
                };
            }
        }

        public Boolean DBConectar(string sTpBancoDados, string sStringConexao)
        {
            bool bOk = false;

            sTipoBancoDados = sTpBancoDados;

            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    {
                        oConexao = new clsMySql();

                        bOk = ((clsMySql)oConexao).DBConectar(sStringConexao);

                        break;
                    }
                case Constantes.const_TipoBancoDados_SqlServer:
                    {
                        oConexao = new clsSqlServer();

                        bOk = ((clsSqlServer)oConexao).DBConectar(sStringConexao);

                        break;
                    }
            }

            return bOk;
        }

        public void DBReconectar()
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    {
                        ((clsMySql)oConexao).DBReconectar();

                        break;
                    }
                case Constantes.const_TipoBancoDados_SqlServer:
                    {
                        ((clsSqlServer)oConexao).DBReconectar();

                        break;
                    }
            }
        }


        public String DBStringConexao()
        {
            string sRet = "";

            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    {
                        sRet = ((clsMySql)oConexao).connNivelAcesso.ConnectionString;

                        break;
                    }
                case Constantes.const_TipoBancoDados_SqlServer:
                    {
                        sRet = ((clsSqlServer)oConexao).cnn.ConnectionString;

                        break;
                    }
            }

            return sRet;
        }

        public Boolean DBConectado()
        {
            bool bOk = false;

            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    {
                        bOk = ((clsMySql)oConexao).DBConectado();

                        break;
                    }
                case Constantes.const_TipoBancoDados_SqlServer:
                    {
                        bOk = ((clsSqlServer)oConexao).DBConectado();

                        break;
                    }
            }

            return bOk;
        }

        public void DBDesconectar()
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    {
                        ((clsMySql)oConexao).DBDesconectar();

                        break;
                    }
                case Constantes.const_TipoBancoDados_SqlServer:
                    {
                        ((clsSqlServer)oConexao).DBDesconectar();

                        break;
                    }
            }
        }

        public DbCommand DBComando(string sSqlText)
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    return ((clsMySql)oConexao).DBComando(sSqlText);
                default:
                    return null;
            }
        }

        public DataTable DBQuery(String sSqlText)
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    return ((clsMySql)oConexao).DBQuery(sSqlText);
                case Constantes.const_TipoBancoDados_SqlServer:
                    return ((clsSqlServer)oConexao).DBQuery(sSqlText);
                default:
                    return null;
            }
        }

        public DataRow DBQuery_PrimeiraLinha(String sSqlText)
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    return ((clsMySql)oConexao).DBQuery_PrimeiraLinha(sSqlText);
                default:
                    return null;
            }
        }

        public object DBQuery_ValorUnico(String sSqlText, object ValorPadrao = null, object Campos = null)
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    return ((clsMySql)oConexao).DBQuery_ValorUnico(sSqlText, ValorPadrao, Campos);
                case Constantes.const_TipoBancoDados_SqlServer:
                    return ((clsSqlServer)oConexao).DBQuery_ValorUnico(sSqlText, ValorPadrao, Campos);
                default:
                    return null;
            }
        }

        public DateTime DBDataAtual()
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    return ((clsMySql)oConexao).DBDataAtual();
                case Constantes.const_TipoBancoDados_SqlServer:
                    return ((clsSqlServer)oConexao).DBDataAtual();
                default:
                    return DateTime.Now;
            }
        }

        public string[] DBValidarCampos(string Tabela, string[] Campos, ref int iColunasValidas)
        {
            DataTable oData;
            Boolean bAchou = false;

            iColunasValidas = 0;

            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_SqlServer:
                    oData = ((clsSqlServer)oConexao).DBCampos(Tabela);
                    break;
                default:
                    return null;
            }

            for (int intI = 0; intI < Campos.Length; intI++)
            {
                bAchou = false;

                foreach (DataColumn Coluna in oData.Columns)
                {
                    if (Campos[intI].ToString().Trim().ToUpper() == Coluna.ColumnName.Trim().ToUpper())
                    {
                        iColunasValidas++;
                        bAchou = true;
                        break;
                    }
                }

                if (!bAchou)
                    Campos[intI] = "?";

            }

            oData.Dispose();

            return Campos;
        }

        public object DBParametro_Transformar(clsCampo[] oParametro)
        {
            if (oParametro != null)
            {
                switch (sTipoBancoDados)
                {
                    case Constantes.const_TipoBancoDados_MySql:
                        clsMySql.clsMySql_Campo[] oMySqlParametro_Ret = new clsMySql.clsMySql_Campo[oParametro.Length];

                        MySqlDbType oMySqlTipo = MySqlDbType.VarChar;

                        for (int intI = 0; intI < oParametro.Length; intI++)
                        {
                            if (oParametro[intI].Direcao == 0) { oParametro[intI].Direcao = ParameterDirection.Input; };

                            oMySqlTipo = MySqlDbType.VarChar;

                            if (oParametro[intI].Tipo == DbType.Decimal)
                                oMySqlTipo = MySqlDbType.Decimal;
                            else if (oParametro[intI].Tipo == DbType.Int32)
                                oMySqlTipo = MySqlDbType.Int32;
                            else if (oParametro[intI].Tipo == DbType.Double)
                                oMySqlTipo = MySqlDbType.Double;
                            else if (oParametro[intI].Tipo == DbType.Date)
                                oMySqlTipo = MySqlDbType.Date;
                            else if (oParametro[intI].Tipo == DbType.Time)
                                oMySqlTipo = MySqlDbType.Time;
                            else if (oParametro[intI].Tipo == DbType.DateTime)
                                oMySqlTipo = MySqlDbType.DateTime;
                            else if (oParametro[intI].Tipo == DbType.String)
                                oMySqlTipo = MySqlDbType.VarChar;

                            oParametro[intI].Nome = oParametro[intI].Nome.Replace("#", "?");

                            oMySqlParametro_Ret[intI] = new clsMySql.clsMySql_Campo
                            {
                                Nome = oParametro[intI].Nome,
                                Tipo = oMySqlTipo,
                                Valor = oParametro[intI].Valor,
                                Direcao = oParametro[intI].Direcao
                            };
                        }

                        return oMySqlParametro_Ret;
                    case Constantes.const_TipoBancoDados_SqlServer:
                        clsSqlServer.clsCampo[] oSqlServerParametro_Ret = new clsSqlServer.clsCampo[oParametro.Length];

                        SqlDbType oSqlServerTipo = SqlDbType.VarChar;

                        for (int intI = 0; intI < oParametro.Length; intI++)
                        {
                            if (oParametro[intI].Direcao == 0) { oParametro[intI].Direcao = ParameterDirection.Input; };

                            oSqlServerTipo = SqlDbType.VarChar;

                            if (oParametro[intI].Tipo == DbType.Decimal)
                                oSqlServerTipo = SqlDbType.Decimal;
                            else if (oParametro[intI].Tipo == DbType.Int32)
                                oSqlServerTipo = SqlDbType.Int;
                            else if (oParametro[intI].Tipo == DbType.Double)
                                oSqlServerTipo = SqlDbType.Float;
                            else if (oParametro[intI].Tipo == DbType.Date)
                                oSqlServerTipo = SqlDbType.Date;
                            else if (oParametro[intI].Tipo == DbType.Time)
                                oSqlServerTipo = SqlDbType.Time;
                            else if (oParametro[intI].Tipo == DbType.DateTime)
                                oSqlServerTipo = SqlDbType.DateTime;
                            else if (oParametro[intI].Tipo == DbType.String)
                                oSqlServerTipo = SqlDbType.VarChar;

                            oParametro[intI].Nome = oParametro[intI].Nome.Replace("#", "@");

                            oSqlServerParametro_Ret[intI] = new clsSqlServer.clsCampo
                            {
                                Nome = oParametro[intI].Nome,
                                Tipo = oSqlServerTipo,
                                Valor = oParametro[intI].Valor,
                                Direcao = oParametro[intI].Direcao
                            };
                        }

                        return oSqlServerParametro_Ret;
                    default:
                        return null;
                }
            }
            else
                return null;
        }

        public Boolean DBExecutar(string sTexto,
                                  clsCampo[] oParametro = null,
                                  Boolean NaoExecutado = false)
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    sTexto = sTexto.Replace("#", "?");
                    return ((clsMySql)oConexao).DBSQL_Executar(sTexto, (clsMySql.clsMySql_Campo[])DBParametro_Transformar(oParametro));
                case Constantes.const_TipoBancoDados_SqlServer:
                    sTexto = sTexto.Replace("#", "@");
                    return ((clsSqlServer)oConexao).DBSQL_Executar(sTexto, (clsSqlServer.clsCampo[])DBParametro_Transformar(oParametro));
                default:
                    return false;
            }
        }

        public Boolean DBProcedure(string sTexto,
                                   clsCampo[] oParametro = null)
        {
            switch (sTipoBancoDados)
            {
                case Constantes.const_TipoBancoDados_MySql:
                    return ((clsMySql)oConexao).DBSQL_Procedure(sTexto, (clsMySql.clsMySql_Campo[])DBParametro_Transformar(oParametro));
                case Constantes.const_TipoBancoDados_SqlServer:
                    return ((clsSqlServer)oConexao).DBSQL_Procedure(sTexto, (clsSqlServer.clsCampo[])DBParametro_Transformar(oParametro));
                default:
                    return false;
            }
        }

        public Boolean DBSQL_Log_Gravar(int iIdEmpresa,
                                        string sDsRotina,
                                        string sTpMessage,
                                        string sDs_log)
        {
            Boolean bOk = false;

            try
            {
                string sSql = "INSERT INTO tb_log (idEmpresa,dsRotina,tpMessage,DsLog)" +
                              " VALUES(#idEmpresa,#dsRotina,#tpMessage,#DsLog)";

                DBExecutar(sSql, new clsCampo[] {new clsCampo {Nome = "idEmpresa", Tipo = DbType.Int32 , Valor = iIdEmpresa},
                                                 new clsCampo {Nome = "dsRotina", Tipo = DbType.String, Valor = sDsRotina},
                                                 new clsCampo {Nome = "tpMessage", Tipo = DbType.String, Valor = sTpMessage},
                                                 new clsCampo {Nome = "DsLog", Tipo = DbType.String, Valor = sDs_log}});

                bOk = true;
            }
            catch (Exception Ex)
            {
                //tr.Rollback();
                //DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "");
            }

            return bOk;
        }
    }

    public class clsSqlServer
    {
        public class clsCampo
        {
            public string Nome { get; set; }
            public SqlDbType Tipo { get; set; }
            public object Valor { get; set; }
            public ParameterDirection Direcao { get; set; }
        }

        public clsCampo_Retorno Retorno;
        public long UltimoIdInserido;

        public SqlConnection cnn;

        public Boolean DBConectado()
        {
            try
            {
                return (cnn.State == ConnectionState.Open);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void DBReconectar()
        {
            cnn.Close();
            cnn.Open();
        }

        public void DBDesconectar()
        {
            cnn.Close();
        }

        public Boolean DBConectar(string sConnetionString = null)
        {
            Boolean bOk = false;

            cnn = new SqlConnection(sConnetionString);

            try
            {
                cnn.Open();
                bOk = true;
            }
            catch (Exception ex)
            {
                //Funcoes.FNC_Mensagem("Não foi possível conectar ao banco de dados - " + ex.Message, Funcoes.Mensasagem_Tipo.Informacao);
            }

            return bOk;
        }

        public DataTable DBCampos(string sTabela)
        {
            sTabela = "SELECT TOP 0 * FROM " + sTabela;

            return DBQuery(sTabela);
        }

        public DateTime DBDataAtual()
        {
            return Convert.ToDateTime(DBQuery_ValorUnico("select getdate() from tb_tipo_empresa"));
        }

        public SqlCommand DBComando(string sSqlText)
        {
            return new SqlCommand(sSqlText, cnn);
        }

        public int DBRetornarInt(string sSqlText)
        {
            int ID = 0;

            SqlCommand cmd = new SqlCommand(sSqlText, cnn);

            //instância do leitor 
            SqlDataReader ret = cmd.ExecuteReader();

            //enquanto leitor lê 
            while (ret.Read())
            {
                ID = Convert.ToInt32(ret[0].ToString());
            }

            ret.Close();
            ret = null;

            cmd.Dispose();
            cmd = null;

            return ID;
        }

        public object DBQuery_ValorUnico(String sSqlText, object ValorPadrao = null, object Campos = null)
        {
            DataTable oRet = null;
            object iRet = 0;

            try
            {
                oRet = DBQuery(sSqlText);

                if (oRet.Rows.Count > 0)
                {
                    if (Campos == null)
                    {
                        iRet = oRet.Rows[0][Convert.ToInt32(Campos)];
                    }
                }
                else
                {
                    iRet = ValorPadrao;
                }

                oRet.Dispose();
                oRet = null;
            }
            catch (MySqlException ex)
            {
                //Erro_Setar(ex.Message);
            }

            return iRet;
        }

        public DataTable DBQuery(string sSqlText)
        {
            DataTable oData = new DataTable();

            try
            {
                SqlCommand oCmd = new SqlCommand(sSqlText, cnn);

                //instância do leitor 
                SqlDataReader oReader = oCmd.ExecuteReader();

                oData.Load(oReader);

                oReader.Close();
                oReader = null;

                oCmd.Dispose();
                oCmd = null;
            }
            catch (Exception Ex)
            {
                DBSQL_Log_Gravar(0, "DBSQL_Executar", "B", Ex.Message);
            }

            return oData;
        }

        public Boolean DBSQL_Executar(string sTexto,
                                      clsCampo[] oParametro = null)
        {
            return DBSQL_Executar(sTexto, System.Data.CommandType.Text, oParametro);
        }

        public Boolean DBSQL_Procedure(string sTexto,
                                      clsCampo[] oParametro = null)
        {
            return DBSQL_Executar(sTexto, System.Data.CommandType.StoredProcedure, oParametro);
        }

        private Boolean DBSQL_Executar(string sTexto,
                                       System.Data.CommandType osCommandType,
                                       clsCampo[] oParametro = null)
        {
            Boolean bOk = false;
            Boolean bRetornaValor = false;

            //SqlTransaction oSqlTransaction;

            try
            {
                cnn.Close();
                cnn.Open();
            }
            catch (Exception)
            {
                throw;
            }

            //oSqlTransaction = cnn.BeginTransaction();

            try
            {
                //Basic command and connection initialization 
                SqlCommand cmd = new SqlCommand(sTexto, cnn);
                cmd.CommandType = osCommandType;
                //cmd.Transaction = oSqlTransaction;

                if (oParametro != null)
                {
                    foreach (clsCampo Parametro in oParametro)
                    {
                        if (Parametro.Direcao == 0)
                            Parametro.Direcao = ParameterDirection.Input;

                        if (Parametro.Direcao == ParameterDirection.InputOutput || Parametro.Direcao == ParameterDirection.Output)
                            bRetornaValor = true;

                        DBParametro(cmd, "@" + Parametro.Nome, Parametro.Valor, Parametro.Tipo, Parametro.Direcao);
                    }
                }

                //Open connection and Execute 
                if (bRetornaValor)
                {
                    Retorno = new clsCampo_Retorno();
                    Retorno.ParametroRetorno = new Integradores.clsCampo[cmd.Parameters.Count];
                    int iCont = 0;

                    cmd.ExecuteReader();

                    foreach (SqlParameter Parametro in cmd.Parameters)
                    {
                        Retorno.ParametroRetorno[iCont] = new Integradores.clsCampo();
                        Retorno.ParametroRetorno[iCont].Nome = "";

                        if (Parametro.Direction == ParameterDirection.InputOutput || Parametro.Direction == ParameterDirection.Output)
                        {
                            Retorno.ParametroRetorno[iCont].Nome = Parametro.ParameterName.Substring(1);
                            Retorno.ParametroRetorno[iCont].Valor = Parametro.Value;
                        }
                        iCont++;
                    }

                    cmd.Dispose();
                    cmd = null;
                }
                else
                {
                    UltimoIdInserido = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                    //UltimoIdInserido = cmd.;
                }

                //oSqlTransaction.Commit();

                bOk = true;
            }
            catch (Exception Ex)
            {
                //oSqlTransaction.Rollback();

                if (Declaracao.ErroMensagem == "Gravar")
                    Declaracao.ErroMensagem = Ex.Message;

                DBSQL_Log_Gravar(0, "DBSQL_Executar", "B", Ex.Message);
                bOk = false;
            }

            //oSqlTransaction.Dispose();
            //oSqlTransaction = null;

            return bOk;
        }

        private void DBParametro(SqlCommand cmd,
                                 string sNome,
                                 object Valor,
                                 SqlDbType oTipo = SqlDbType.VarChar,
                                 ParameterDirection oDirecao = ParameterDirection.Input)
        {
            cmd.Parameters.Add(new SqlParameter(sNome, oTipo));
            cmd.Parameters[sNome].Direction = oDirecao;
            cmd.Parameters[sNome].Value = Valor;
            if (Valor == null)
            {
                cmd.Parameters[sNome].Size = 100;
            }
            else
            {
                if (oTipo == SqlDbType.VarChar) { cmd.Parameters[sNome].Size = Valor.ToString().Length; } else { cmd.Parameters[sNome].Size = 100; }
            }
        }

        private Boolean DBSQL_Log_Gravar(int iIdEmpresa,
                                         string sDsRotina,
                                         string sTpMessage,
                                         string sDs_log)
        {
            Boolean bOk = false;

            try
            {
                string sSql = "INSERT INTO tb_log (idEmpresa,dsRotina,tpMessage,DsLog,DataLog)" +
                              " VALUES(@idEmpresa,@dsRotina,@tpMessage,@DsLog,@DataLog)";

                DBSQL_Executar(sSql, new clsCampo[] {new clsCampo {Nome = "idEmpresa", Tipo = SqlDbType.Int, Valor = iIdEmpresa},
                                                     new clsCampo {Nome = "dsRotina", Tipo = SqlDbType.VarChar, Valor = sDsRotina},
                                                     new clsCampo {Nome = "tpMessage", Tipo = SqlDbType.VarChar, Valor = sTpMessage},
                                                     new clsCampo {Nome = "DsLog", Tipo = SqlDbType.VarChar, Valor = sDs_log},
                                                     new clsCampo {Nome = "DataLog", Tipo = SqlDbType.DateTime2, Valor = DateTime.Now }});

                bOk = true;
            }
            catch (Exception Ex)
            {
                //tr.Rollback();
                //DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "");
            }

            return bOk;
        }
    }

    public class clsMySql
    {
        public class clsMySql_Campo
        {
            public string Nome { get; set; }
            public MySqlDbType Tipo { get; set; }
            public object Valor { get; set; }
            public ParameterDirection Direcao { get; set; }
        }

        public Boolean bErro;
        public string sErro;
        public string sBancoDados = "";
        public clsCampo_Retorno Retorno;
        public long UltimoIdInserido;

        public MySqlConnection connNivelAcesso = new MySqlConnection();

        public Boolean DBConectado()
        {
            try
            {
                return (connNivelAcesso.State == System.Data.ConnectionState.Open);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Boolean DBConectar(string sStringConexao = "")
        {
            Boolean bOk = false;

            Erro_Limpar();

            try
            {
                if (sStringConexao.Trim() != "")
                    connNivelAcesso.ConnectionString = sStringConexao;

                connNivelAcesso.Open();

                bOk = true;
            }
            catch (Exception ex)
            {
                DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, ex.Message, "");
                Erro_Setar(ex.Message);

                bOk = false;
            }

            return bOk;
        }

        public void DBReconectar()
        {
            connNivelAcesso.Close();
            connNivelAcesso.Open();
        }

        public void DBDesconectar()
        {
            connNivelAcesso.Close();
        }

        public DataTable DBQuery(String sSqlText)
        {
            DataTable oRet = new DataTable();

            Erro_Limpar();

            try
            {
                MySqlCommand cmd = new MySqlCommand(sSqlText, connNivelAcesso);

                MySqlDataReader reader = cmd.ExecuteReader();

                oRet.Load(reader);

                reader.Close();
                reader.Dispose();
            }
            catch (MySqlException ex)
            {
                Erro_Setar(ex.Message);
            }

            return oRet;
        }

        public DataRow DBQuery_PrimeiraLinha(String sSqlText)
        {
            DataTable oData = new DataTable();
            DataRow oRow;

            oData = DBQuery(sSqlText);

            oRow = oData.Rows[0];

            return oRow;
        }

        public DateTime DBDataAtual()
        {
            return Convert.ToDateTime(DBQuery_ValorUnico("SELECT CURRENT_TIMESTAMP"));
        }

        public object DBQuery_ValorUnico(String sSqlText, object ValorPadrao = null, object Campos = null)
        {
            DataTable oRet = null;
            object iRet = 0;

            Erro_Limpar();

            try
            {
                oRet = DBQuery(sSqlText);

                if (oRet.Rows.Count > 0)
                {
                    if (Campos == null)
                    {
                        iRet = oRet.Rows[0][Convert.ToInt32(Campos)];
                    }
                }
                else
                {
                    iRet = ValorPadrao;
                }
            }
            catch (MySqlException ex)
            {
                Erro_Setar(ex.Message);
            }

            return iRet;
        }

        public string DBTabela(string sNome)
        {
            string sRet;

            sNome = sNome.ToLower().ToString();

            if (sBancoDados.Trim() == "")
            {
                sRet = sNome;
            }
            else
            {
                sRet = sBancoDados.Trim() + "." + sNome.Trim();
            }

            return sRet;
        }

        private void DBParametro(ref MySqlCommand cmd,
                                 string sNome,
                                 object Valor,
                                 MySqlDbType oTipo = MySqlDbType.VarChar,
                                 ParameterDirection oDirecao = ParameterDirection.Input)
        {
            cmd.Parameters.Add(new MySqlParameter(sNome, oTipo));
            cmd.Parameters[sNome].Direction = oDirecao;
            cmd.Parameters[sNome].Value = Valor;
        }

        private void Erro_Limpar()
        {
            bErro = false;
            sErro = "";
        }

        private void Erro_Setar(string sMensagem)
        {
            bErro = true;
            sErro = sMensagem;
        }

        public MySqlCommand DBComando(string sSqlText)
        {
            return new MySqlCommand(sSqlText, connNivelAcesso);
        }

        public Boolean DBSQL_Executar(string sTexto,
                                      clsMySql_Campo[] oParametro = null)
        {
            return DBSQL_Executar(sTexto, System.Data.CommandType.Text, oParametro);
        }

        public Boolean DBSQL_Procedure(string sTexto,
                                      clsMySql_Campo[] oParametro = null)
        {
            return DBSQL_Executar(sTexto, System.Data.CommandType.StoredProcedure, oParametro);
        }

        private Boolean DBSQL_Executar(string sTexto,
                                       System.Data.CommandType osCommandType,
                                       clsMySql_Campo[] oParametro = null)
        {

            Boolean bOk = false;
            Boolean bRetornaValor = false;

            try
            {
                //Basic command and connection initialization 
                MySqlCommand cmd = new MySqlCommand(sTexto, connNivelAcesso);
                cmd.CommandType = osCommandType;

                if (oParametro != null)
                {
                    foreach (clsMySql_Campo Parametro in oParametro)
                    {
                        if (Parametro.Direcao == 0)
                            Parametro.Direcao = ParameterDirection.Input;

                        if (Parametro.Direcao == ParameterDirection.InputOutput || Parametro.Direcao == ParameterDirection.Output)
                            bRetornaValor = true;

                        DBParametro(ref cmd, "?" + Parametro.Nome, Parametro.Valor, Parametro.Tipo, Parametro.Direcao);
                    }
                }

                //Open connection and Execute 
                if (bRetornaValor)
                {
                    Retorno = new clsCampo_Retorno();
                    Retorno.ParametroRetorno = new clsCampo[cmd.Parameters.Count];
                    int iCont = 0;

                    cmd.ExecuteReader();

                    foreach (MySqlParameter Parametro in cmd.Parameters)
                    {
                        Retorno.ParametroRetorno[iCont] = new clsCampo();
                        Retorno.ParametroRetorno[iCont].Nome = "";

                        if (Parametro.Direction == ParameterDirection.InputOutput || Parametro.Direction == ParameterDirection.Output)
                        {
                            Retorno.ParametroRetorno[iCont].Nome = Parametro.ParameterName.Substring(1);
                            Retorno.ParametroRetorno[iCont].Valor = Parametro.Value;
                        }
                        iCont++;
                    }

                    cmd.Dispose();
                }
                else
                {
                    cmd.ExecuteNonQuery();
                    UltimoIdInserido = cmd.LastInsertedId;
                }

                bOk = true;
            }
            catch (Exception Ex)
            {
                DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message + "(" + sTexto + ")", "");
            }

            return bOk;
        }

        public Boolean DBSQL_Tarefas_AutoAgendar_Gravar(int iId_Tarefa)
        {
            Boolean bOk = false;

            try
            {
                MySqlTransaction tr = connNivelAcesso.BeginTransaction();

                //Basic command and connection initialization 
                MySqlCommand cmd = new MySqlCommand("sp_gertarefas_autoagendar_cad", connNivelAcesso);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Transaction = tr;

                DBParametro(ref cmd, "?p_idTarefa", iId_Tarefa, MySqlDbType.Int32);

                //Open connection and Execute 
                cmd.ExecuteNonQuery();

                tr.Commit();

                bOk = true;
            }
            catch (Exception Ex)
            {
                DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, iId_Tarefa, Ex.Message, "");
            }

            return bOk;
        }

        public Boolean DBSQL_Integrador_Gravar(string sDs_servidor_entrada,
                                               string sDs_servidor_saida,
                                               string sDs_usuario,
                                               string sDs_senha,
                                               string sCd_codigo_copia_arquivo,
                                               string sCd_codigo_empresa,
                                               string sDs_pasta_tabelas,
                                               string sDs_pasta_imagens,
                                               string sDs_pasta_importacao,
                                               string sDs_pasta_exportacao,
                                               string sDs_pasta_lokalizei,
                                               string sDs_separador_campo,
                                               string sDs_separador_colunas,
                                               string sIc_igorar_erro_saldo_estoque,
                                               string sIc_desativar_execucao_automatica,
                                               string sIc_gerar_tabela_preco,
                                               string sIc_re_enviar_imagens,
                                               string sIc_gerar_log_imagens,
                                               string sIc_enviar_pedido_direto,
                                               string sIc_enviar_estoque)
        {
            Boolean bOk = false;

            try
            {
                MySqlTransaction tr = connNivelAcesso.BeginTransaction();

                //Basic command and connection initialization 
                MySqlCommand cmd = new MySqlCommand("pdsuite.sp_integrador_cad", connNivelAcesso);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Transaction = tr;

                DBParametro(ref cmd, "?p_ds_servidor_entrada", sDs_servidor_entrada, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_servidor_saida", sDs_servidor_saida, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_usuario", sDs_usuario, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_senha", sDs_senha, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_cd_codigo_copia_arquivo", sCd_codigo_copia_arquivo, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_cd_codigo_empresa", sCd_codigo_empresa, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_pasta_tabelas", sDs_pasta_tabelas, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_pasta_imagens", sDs_pasta_imagens, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_pasta_importacao", sDs_pasta_importacao, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_pasta_exportacao", sDs_pasta_exportacao, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_pasta_lokalizei", sDs_pasta_lokalizei, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_separador_campo", sDs_separador_campo, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ds_separador_colunas", sDs_separador_colunas, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ic_igorar_erro_saldo_estoque", sIc_igorar_erro_saldo_estoque, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ic_desativar_execucao_automatica", sIc_desativar_execucao_automatica, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ic_gerar_tabela_preco", sIc_gerar_tabela_preco, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ic_re_enviar_imagens", sIc_re_enviar_imagens, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ic_gerar_log_imagens", sIc_gerar_log_imagens, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ic_enviar_pedido_direto", sIc_enviar_pedido_direto, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?p_ic_enviar_estoque", sIc_enviar_estoque, MySqlDbType.VarChar);

                //Open connection and Execute 
                cmd.ExecuteNonQuery();

                tr.Commit();

                bOk = true;
            }
            catch (Exception Ex)
            {
                DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "");
            }

            return bOk;
        }

        public Boolean DBSQL_Integrador_Log_Gravar(int iId_Empresa,
                                                   int iId_Tarefa,
                                                   string sDs_log)
        {
            Boolean bOk = false;

            try
            {
                MySqlTransaction tr = connNivelAcesso.BeginTransaction();

                //Basic command and connection initialization 
                MySqlCommand cmd = new MySqlCommand("sp_integrador_log_cad", connNivelAcesso);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Transaction = tr;

                DBParametro(ref cmd, "?p_id_empresa", iId_Empresa, MySqlDbType.Int32);
                DBParametro(ref cmd, "?p_idTarefa", iId_Tarefa, MySqlDbType.Int32);
                DBParametro(ref cmd, "?p_ds_log", sDs_log, MySqlDbType.VarChar);

                //Open connection and Execute 
                cmd.ExecuteNonQuery();

                tr.Commit();

                bOk = true;
            }
            catch (Exception Ex)
            {
                DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, iId_Tarefa, Ex.Message, "");
            }

            return bOk;
        }

        public Boolean DBSQL_Log_Gravar(int iId_Empresa,
                                        LogTipo idOperacao,
                                        int iIdRegistro,
                                        string sDs_log,
                                        string sNomeArquivo)
        {
            Boolean bOk = false;

            //MySqlTransaction tr = connNivelAcesso.BeginTransaction();

            try
            {
                string sSql = "INSERT INTO tb_log(idEmpresa,idEntidade,idUsuario,idDispositivo,idOperacao,idRegistro,idRotina," +
                                                 "Latitude,Longitude,ComplementoLog,ComplementoAdicional,DataLog)" +
                              " VALUES(?idEmpresa,?idEntidade,?idUsuario,?idDispositivo,?idOperacao,?idRegistro,?idRotina," +
                                      "?Latitude,?Longitude,?ComplementoLog,?ComplementoAdicional,Now())";

                //Basic command and connection initialization 
                MySqlCommand cmd = new MySqlCommand(sSql, connNivelAcesso);
                cmd.CommandType = System.Data.CommandType.Text;
                //cmd.Transaction = tr;

                DBParametro(ref cmd, "?idEmpresa", iId_Empresa, MySqlDbType.Int32);
                DBParametro(ref cmd, "?idEntidade", 0, MySqlDbType.Int32);
                DBParametro(ref cmd, "?idUsuario", 0, MySqlDbType.Int32);
                DBParametro(ref cmd, "?idDispositivo", 0, MySqlDbType.Int32);
                DBParametro(ref cmd, "?idOperacao", idOperacao, MySqlDbType.Int32);
                DBParametro(ref cmd, "?idRegistro", iIdRegistro, MySqlDbType.Int32);
                DBParametro(ref cmd, "?idRotina", 500, MySqlDbType.Int32);
                DBParametro(ref cmd, "?Latitude", 0, MySqlDbType.Int32);
                DBParametro(ref cmd, "?Longitude", 0, MySqlDbType.Int32);
                DBParametro(ref cmd, "?ComplementoLog", sDs_log, MySqlDbType.VarChar);
                DBParametro(ref cmd, "?ComplementoAdicional", sNomeArquivo, MySqlDbType.VarChar);

                //Open connection and Execute 
                cmd.ExecuteNonQuery();

                //tr.Commit();

                bOk = true;
            }
            catch (Exception Ex)
            {
                //tr.Rollback();
                //DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "");
            }

            return bOk;
        }

        public string DataMySql(DateTime Data)
        {
            try
            {
                return Data.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception)
            {

                return null;
            }
        }

        public DateTime DBStringToData(string sData, string sHora = "")
        {
            DateTime dData = new DateTime();

            try
            {
                string sDia = sData.Substring(0, sData.IndexOf("/"));
                string sMes = sData.Substring(sData.IndexOf("/") + 1, sData.IndexOf("/", sData.IndexOf("/") + 1) - sData.IndexOf("/") - 1);
                string sAno = sData.Substring(sData.IndexOf("/", sData.IndexOf("/") + 1) + 1);

                dData = dData.AddDays(Convert.ToInt32(sDia) - 1);
                dData = dData.AddMonths(Convert.ToInt32(sMes) - 1);
                dData = dData.AddYears(Convert.ToInt32(sAno) - 1);

                if (sHora.Trim() != "")
                {
                    dData = dData.AddHours(Convert.ToInt32(sHora.Substring(0, 2)));
                    dData = dData.AddMinutes(Convert.ToInt32(sHora.Substring(3, 2)));
                }
            }
            catch (Exception ex)
            {
            }

            return dData;
        }

        public static string DBDataToString(DateTime dData)
        {
            string sData = "";

            sData = _Funcoes.FNC_ZerosEsquerda(dData.Day.ToString().Trim(), 2) + "/" +
                    _Funcoes.FNC_ZerosEsquerda(dData.Month.ToString().Trim(), 2) + "/" +
                    _Funcoes.FNC_ZerosEsquerda(dData.Year.ToString().Trim(), 4);

            return sData;
        }

        public static string DBDataToStringDB(string sData)
        {
            string sDataRet = "";

            try
            {
                sDataRet = sData.Substring(6, 4) + "-" + sData.Substring(3, 2) + "-" + sData.Substring(0, 2);
            }
            catch (Exception)
            {
            }

            return sDataRet;
        }
    }

    public static class Bot
    {
        public static void Terms_Carregar(ref Mensagem oMensagem, ref Config oConfig, int idServico = 0)
        {
            int idMessageTerms = 0;
            int iCont = -1;
            string sSql;

            DataTable oData_01;

            sSql = "SELECT * FROM vwmessagetermswords where idMessageTerms is not null";

            if (idServico != 0)
            {
                sSql = sSql + " where (id_servico = " + idServico.ToString() + " or idServico is null)";
            }

            sSql = sSql + " order by nrOrdem desc, Cod_Puxada desc, Agente desc, cd_servico";

            oData_01 = oConfig.oBancoDados.DBQuery(sSql);

            try
            {
                for (int intI = 0; intI < oData_01.Rows.Count; intI++)
                {
                    if (idMessageTerms != Convert.ToInt32(oData_01.Rows[intI]["idMessageTerms"]))
                    {
                        iCont++;
                        Array.Resize(ref Declaracao.otbmessageterms.messageterms, iCont + 1);
                        Declaracao.otbmessageterms.messageterms[iCont] = new cls_tbmessageterms.cls_messageterms();
                        Declaracao.otbmessageterms.messageterms[iCont].idMessageTerms = Convert.ToInt32(oData_01.Rows[intI]["idMessageTerms"]);
                        Declaracao.otbmessageterms.messageterms[iCont].idMessageTermsPai = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["idMessageTermsPai"]));
                        Declaracao.otbmessageterms.messageterms[iCont].Cod_Puxada = oData_01.Rows[intI]["Cod_Puxada"].ToString();
                        Declaracao.otbmessageterms.messageterms[iCont].nrOrdem = Convert.ToInt32(oData_01.Rows[intI]["nrOrdem"]);
                        Declaracao.otbmessageterms.messageterms[iCont].Agente = oData_01.Rows[intI]["Agente"].ToString();
                        Declaracao.otbmessageterms.messageterms[iCont].Servico = oData_01.Rows[intI]["cd_Servico"].ToString();
                        Declaracao.otbmessageterms.messageterms[iCont].Command = oData_01.Rows[intI]["Command"].ToString();
                        Declaracao.otbmessageterms.messageterms[iCont].TipoComando = oData_01.Rows[intI]["TipoComando"].ToString();
                        Declaracao.otbmessageterms.messageterms[iCont].dsTerms = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["dsTerms"]).Trim();
                        Declaracao.otbmessageterms.messageterms[iCont].dsTemplateHeader = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["dsTemplateHeader"]);
                        Declaracao.otbmessageterms.messageterms[iCont].dsTemplate = oData_01.Rows[intI]["dsTemplate"].ToString();
                        Declaracao.otbmessageterms.messageterms[iCont].dsTemplateFooter = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["dsTemplateFooter"]);
                        Declaracao.otbmessageterms.messageterms[iCont].dsUrl_Image = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["dsUrl_Image"]);
                        Declaracao.otbmessageterms.messageterms[iCont].StringConexao = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["ds_stringconexao"]);
                        Declaracao.otbmessageterms.messageterms[iCont].TipoBancoDados = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["tp_bancodados"]);
                        Declaracao.otbmessageterms.messageterms[iCont].Query_Tipo = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["Query_Tipo"]);
                        Declaracao.otbmessageterms.messageterms[iCont].Query_String = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["Query_String"]);
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRegistro = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRegistro"]);
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TipoConsulta = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoConsulta"]);
                        Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_JsonParametros"]);
                        Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros = Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros.Replace("WS_TipoConsulta]", Declaracao.otbmessageterms.messageterms[iCont].WS_TipoConsulta);
                        Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros = Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros.Replace("[WS_TipoRegistro]", Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRegistro);
                        Declaracao.otbmessageterms.messageterms[iCont].SepararMensagens = (_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["SepararMensagens"]) == 1);
                        Declaracao.otbmessageterms.messageterms[iCont].WS_ProximaMensagem = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_ProximaMensagem"]));
                        Declaracao.otbmessageterms.messageterms[iCont].WS_IntervaloConsulta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_IntervaloConsulta"]));
                        Declaracao.otbmessageterms.messageterms[iCont].WS_VezesAoDia = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_VezesAoDia"]));
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TempoEspera = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_TempoEspera"]));
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetorno = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetorno"]).Trim();
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoCampo = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoCampo"]).Trim();
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoRespostas = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoRespostas"]).Trim();
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoScript = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoScript"]).Trim();
                        Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoTipo = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoTipo"]).Trim();
                        Declaracao.otbmessageterms.messageterms[iCont].WS_ComprimentoMin = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_ComprimentoMin"]));
                        Declaracao.otbmessageterms.messageterms[iCont].WS_CampoObrigatorio = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_CampoObrigatorio"]));
                        Declaracao.otbmessageterms.messageterms[iCont].WS_MetodoValidarUsuario = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_MetodoValidarUsuario"]).Trim();
                        idMessageTerms = Convert.ToInt32(oData_01.Rows[intI]["idMessageTerms"]);
                    }

                    if (Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Substring(Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Length - 1, 1) == ";")
                        Declaracao.otbmessageterms.messageterms[iCont].dsTerms = Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Substring(0, Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Length - 1);

                    if (oData_01.Rows[intI]["SearchTerm"] != null)
                        Declaracao.otbmessageterms.messageterms[iCont].dsTerms = Declaracao.otbmessageterms.messageterms[iCont].dsTerms + ";" + oData_01.Rows[intI]["SearchTerm"].ToString();
                }
            }
            catch (Exception)
            {
                idMessageTerms = idMessageTerms;
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_BancoDados_Tems, "Carregado");
        }

        public static void Terms_Descarregar()
        {
            Declaracao.otbmessageterms.messageterms = null;
        }

        public static void CarregarTabelas(ref Mensagem oMensagem, ref Config oConfig, bool bLimpar)
        {
            string sSql = "";

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_BancoDados, "Carregando Tabelas");

            if ((oMensagem.oTabela == null) || bLimpar)
            {
                oMensagem.oTabela = new DataTable[Constantes.const_TabelasAuxiliares_Quantidade];
            }

            //Carregar dados do bot
            if (oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Bot] == null)
            {
                if (oMensagem.idBot != 0)
                {
                    oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Bot] = oConfig.oBancoDados.DBQuery("select * from vw_bot where botAtivo = 1 and idBot = " + oMensagem.idBot.ToString());
                }
                else
                {
                    if (oMensagem.botname.Trim().ToUpper() != "")
                    {
                        sSql = "select * from vw_bot where botAtivo = 1 and rtrim(upper(Apelido)) = '" + oMensagem.botname.Trim().ToUpper() + "'";

                        if (_Funcoes.FNC_NuloString(oMensagem.idBotExterno).Trim().ToUpper() != "")
                        {
                            sSql = sSql + " and idBotExterno = '" + oMensagem.idBotExterno.Trim() + "'";
                        }

                        oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Bot] = oConfig.oBancoDados.DBQuery(sSql);
                    }
                }
            }
            else
            {
                if (oMensagem.idBot != 0)
                {
                    if (Convert.ToUInt32(Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idMessage", 0)) != oMensagem.idBot)
                    {
                        sSql = "select * from vw_bot where botAtivo = 1 and idBot = " + oMensagem.idBot.ToString();

                        if (_Funcoes.FNC_NuloString(oMensagem.idBotExterno).Trim().ToUpper() != "")
                        {
                            sSql = sSql + " and idBotExterno = '" + oMensagem.idBotExterno.Trim() + "'";
                        }

                        oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Bot] = oConfig.oBancoDados.DBQuery(sSql);
                    }
                }
            }

            if (Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idBancoDados") != null)
            {
                oConfig.oBancoDadosBot = new clsBancoDados();
                oConfig.oBancoDadosBot.DBConectar(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "tp_bancodados").ToString(),
                                                 Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "ds_stringconexao").ToString());
            }
            else
            {
                oConfig.oBancoDadosBot = oConfig.oBancoDados;
            }

            //carregar dados da oMensagem
            if (oMensagem.idMensagem != 0)
            {
                if (oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Message] == null)
                {
                    oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Message] = oConfig.oBancoDadosBot.DBQuery("select * from tbmessage where idMessage = " + oMensagem.idMensagem.ToString());
                }
                else
                {
                    if (Convert.ToUInt32(Tabelas_BuscarValor(ref oMensagem, "tbmessage", "idMessage", 0)) != oMensagem.idMensagem)
                        oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Message] = oConfig.oBancoDadosBot.DBQuery("select * from tbmessage where idMessage = " + oMensagem.idMensagem.ToString());
                }
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_BancoDados, "Tabelas Carregadas");
        }

        public static void CarregarTabelaEmpresa(ref Config oConfig, ref Mensagem oMensagem, string sCodigoPuxada)
        {
            try
            {
                if (oConfig.tipobancodados == Constantes.const_TipoBancoDados_MySql)
                {
                    //carregar dados da oMensagem
                    oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Empresa] = oConfig.oBancoDados.DBQuery("select * from vw_empresas where CodPuxada = '" + sCodigoPuxada.ToString().Trim() + "' limit 1");
                }
                else
                {
                    //carregar dados da oMensagem
                    oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Empresa] = oConfig.oBancoDados.DBQuery("select top 1 * from vw_empresas where CodPuxada = '" + sCodigoPuxada.ToString().Trim() + "'");
                }
            }
            catch (Exception)
            {
            }
        }

        public static void CarregarTabelaServico(ref Config oConfig, ref Mensagem oMensagem, string sCodigoServico)
        {
            try
            {
                oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Servico] = oConfig.oBancoDados.DBQuery("select * from vw_servicos where upper(cd_Servico) =  '" + sCodigoServico.ToString().Trim().ToUpper() + "'");
            }
            catch (Exception)
            {
            }
        }

        public static void CarregarTabelaUsuario(ref Mensagem oMensagem, clsBancoDados oBancoDados, string sTelefone, string sCodigoPuxada, string sServico)
        {
            try
            {
                string sSql = "";

                if (sCodigoPuxada.Trim() == "")
                {
                    sSql = "select * from TB_USUARIO where TELEFONE = '" + sTelefone + "'";
                }
                else
                {
                    sSql = "select * from TB_USUARIO where TELEFONE = '" + sTelefone + "' and Cod_Puxada = '" + sCodigoPuxada.ToString().Trim() + "'";

                    if (sServico.Trim() != "")
                    {
                        sSql = sSql + " and upper(ltrim(rtrim(LICENCA))) = '" + sServico.Trim().ToUpper() + "'";
                    }
                }

                //carregar dados da oMensagem
                oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Usuario] = oBancoDados.DBQuery(sSql);
            }
            catch (Exception)
            {
            }
        }

        public static void Tabelas_AtualizarValor(ref Mensagem oMensagem, string sTabela, string sCampo, object Valor)
        {
            try
            {
                if (sTabela.ToUpper() == "TB_EMPRESAS")
                {
                    oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Empresa].Rows[0][sCampo] = Valor;
                }
                else if (sTabela.ToUpper() == "TB_USUARIO")
                {
                    oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Usuario].Rows[0][sCampo] = Valor;
                }
                else
                {
                    foreach (DataTable Tabela in oMensagem.oTabela)
                    {
                        if (Tabela != null)
                        {
                            if (Tabela.TableName.ToUpper() == sTabela.ToUpper())
                            {
                                Tabela.Rows[0][sCampo] = Valor;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {

            }
        }

        public static object Tabelas_BuscarValor(ref Mensagem oMensagem, string sTabela, string sCampo, object ValorPadrao = null)
        {
            object Ret = "";

            try
            {

                if (sTabela.ToUpper() == "TB_EMPRESAS")
                {
                    Ret = oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Empresa].Rows[0][sCampo];
                }
                else if (sTabela.ToUpper() == "TB_USUARIO")
                {
                    Ret = oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Usuario].Rows[0][sCampo];
                }
                else if (sTabela.ToUpper() == "VW_SERVICOS")
                {
                    Ret = oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Servico].Rows[0][sCampo];
                }
                else if (sTabela.ToUpper() == "TB_QUERY")
                {
                    Ret = oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Query].Rows[0][sCampo];
                }
                else
                {
                    foreach (DataTable Tabela in oMensagem.oTabela)
                    {
                        if (Tabela != null)
                        {
                            if (Tabela.TableName.ToUpper() == sTabela.ToUpper())
                            {
                                Ret = Tabela.Rows[0][sCampo];
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                if (ValorPadrao != null)
                    Ret = ValorPadrao;
            }

            if (ValorPadrao != null && Ret == DBNull.Value)
                Ret = ValorPadrao;

            return Ret;
        }

        public static string LerTag(ref Mensagem oMensagem, ref Config oConfig, string sTemplate)
        {
            string Ret = "";
            int iAux = 0;
            string sAux = "";

            Ret = sTemplate;

            //Procura TAGs acesso a tabelas
            try
            {
                while (true)
                {
                    if (Ret.IndexOf("|CPO.") > -1)
                    {
                        iAux = Ret.Substring(1, Ret.IndexOf("|CPO.")).LastIndexOf("|TAB.");
                        sAux = Ret.Substring(Ret.Substring(0, iAux + 1).LastIndexOf("["), Ret.Substring(iAux).IndexOf("]") - Ret.Substring(1, iAux).LastIndexOf("[") + iAux);

                        //                        Ret = _Funcoes.Replace(Ret, sAux.Substring(sAux.IndexOf("TAB."), sAux.Length - sAux.IndexOf("TAB.") - 1),
                        //Bot.Tabelas_BuscarValor(sAux.Substring(sAux.IndexOf("TAB.") + ("TAB.").Length, sAux.IndexOf("|CPO.") - sAux.IndexOf("TAB.") - ("TAB.").Length),
                        //sAux.Substring(sAux.IndexOf("CPO.") + ("CPO.").Length, sAux.Length - sAux.IndexOf("CPO.") - ("CPO.").Length - 1)));
                        Ret = _Funcoes.Replace(Ret, sAux,
                                               Bot.Tabelas_BuscarValor(ref oMensagem, sAux.Substring(sAux.IndexOf("TAB.") + ("TAB.").Length, sAux.IndexOf("|CPO.") - sAux.IndexOf("TAB.") - ("TAB.").Length),
                                                                       sAux.Substring(sAux.IndexOf("CPO.") + ("CPO.").Length, sAux.Length - sAux.IndexOf("CPO.") - ("CPO.").Length - 1)));
                        Ret = Ret.Replace(sAux, Bot.Tabelas_BuscarValor(ref oMensagem, sAux.Substring(sAux.IndexOf("TAB.") + ("TAB.").Length, sAux.IndexOf("|CPO.") - sAux.IndexOf("TAB.") - ("TAB.").Length),
                                                                       sAux.Substring(sAux.IndexOf("CPO.") + ("CPO.").Length, sAux.Length - sAux.IndexOf("CPO.") - ("CPO.").Length - 1)).ToString());
                    }
                    else
                        break;
                }
            }
            catch (Exception)
            {
            }

            //Procura TAGs de acesso a outros terms
            iAux = 0;

            while (Ret.IndexOf("[-||-|TERMS.", iAux) > -1)
            {
                sAux = Ret.Substring(Ret.IndexOf("[-||-|TERMS."));
                sAux = sAux.Substring(0, sAux.IndexOf("]") + 1);
                string[] sGrupo = sAux.Substring(1, sAux.Length - 2).Replace("||", "|").Split(new char[] { '|' });
                string[] sFiltro = sGrupo[2].Split(new char[] { '.' });

                iAux = Ret.IndexOf(sAux) + sAux.Length;

                oMensagem.Terms = Declaracao.otbmessageterms.PesquisarPorTermo(ref oConfig, ref oMensagem, sGrupo[3], true, sFiltro[1], sFiltro[3]);

                if (oMensagem.Terms != null)
                {
                    Ret = _Funcoes.Replace(Ret, sGrupo[3], oMensagem.Terms.dsTemplate);
                    oMensagem.Terms.dsTemplate = Ret;
                    Ret = Bot.Terms_Processar(ref oMensagem, ref oConfig)[0];
                    oMensagem.Terms = null;
                    iAux = 0;
                }
                else
                {
                    iAux = iAux + sAux.Length;
                }
            }

            return Ret;
        }

        public static string TratarTexto(ref Config oConfig, ref Mensagem oMensagem, string sTemplate, string sProtocolo = "")
        {
            string Ret = "";

            Ret = sTemplate;

            if (_Funcoes.FNC_NuloString(oMensagem.contactname).Trim() != "")
                Ret = _Funcoes.Replace(Ret, "NOME_CONTATO", oMensagem.contactname);
            if (_Funcoes.FNC_NuloString(sProtocolo).Trim() != "")
                Ret = _Funcoes.Replace(Ret, "NRO_PROTOCOLO", sProtocolo);

            Ret = _Funcoes.Replace(Ret, "DATA_HORA_HOJE", oMensagem.messagemtdd.AddHours(-3));
            Ret = _Funcoes.Replace(Ret, "REMUNERACAO_TOTAL", Declaracao.REMUNERACAO_TOTAL);
            Ret = _Funcoes.Replace(Ret, "DROP_TOTAL", Declaracao.DROP_TOTAL);

            return Ret;
        }

        public static Boolean ValidarRetornoJSon(ref Mensagem oMensagem, int codigo, string mensagem, ref string[] Texto)
        {
            Boolean Ret = false;

            if (codigo == -1 || codigo == 9 || codigo == 998 || codigo == 999)
            {
                Texto = new string[1];

                switch (codigo)
                {
                    case -1:
                        Texto[0] = Constantes.const_Template_Command_OFFLINE;
                        break;
                    case 9:
                        Texto[0] = Constantes.const_Template_Command_SEMACESSO;
                        break;
                    case 998:
                        Texto[0] = Constantes.const_Template_Command_ERRO998;
                        break;
                    case 999:
                        Texto[0] = Constantes.const_Template_Command_SERVICOFORA;
                        break;
                }

                oMensagem.ReprocessarTerms = true;
            }
            else
            {
                Ret = true;
            }

            return Ret;
        }

        public static string[] Terms_Processar(ref Mensagem oMensagem, ref Config oConfig)
        {
            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_BancoDados_Tems, "Processando");

            string[] sMensagem = null;
            cls_tbmessageterms.cls_messageterms NovaTerms = null;

            while (true)
            {
                switch (oMensagem.Terms.TipoComando)
                {
                    case "T":
                        {
                            sMensagem = new string[1];
                            sMensagem[0] = LerTag(ref oMensagem, ref oConfig, oMensagem.Terms.dsTemplate);
                            break;
                        }
                    case "Q":
                        {
                            string[] sParametros = oMensagem.messagebody.Trim().Split(new char[] { '.' });
                            int iCont = 1;

                            foreach (string sParametro in sParametros)
                            {
                                if (sParametro.Substring(0, 1) != "*")
                                {
                                    if (_Funcoes.FNC_IsNumeric(sParametro))
                                    {
                                        oMensagem.Terms.Query_String = oMensagem.Terms.Query_String.Replace("Param" + iCont.ToString(), sParametro);
                                    }
                                    else
                                    {
                                        oMensagem.Terms.Query_String = oMensagem.Terms.Query_String.Replace("Param" + iCont.ToString(), "'" + sParametro.Trim() + "'");
                                    }

                                    iCont = iCont + 1;
                                }
                            }

                            clsBancoDados oBancoDados = new clsBancoDados();
                            oBancoDados.DBConectar(oMensagem.Terms.TipoBancoDados, oMensagem.Terms.StringConexao);
                            oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Query] = oBancoDados.DBQuery(oMensagem.Terms.Query_String);

                            oBancoDados.DBDesconectar();

                            sMensagem = new string[1];
                            sMensagem[0] = LerTag(ref oMensagem, ref oConfig, oMensagem.Terms.dsTemplate);
                            break;
                        }
                    case "W":
                        {
                            switch (oMensagem.Terms.Command)
                            {
                                case "DROPCANAL":
                                    {
                                        sMensagem = FlexXTools.FlexXTools_DadosBRK_CanalRemuneracao(ref oMensagem,
                                                                                                    oConfig.oBancoDados,
                                                                                                    oMensagem.idTarefa,
                                                                                                    oMensagem.Terms.StringConexao,
                                                                                                    _Funcoes.FNC_NuloString(Bot.Tabelas_BuscarValor(ref oMensagem, "TB_USUARIO", "Cod_Puxada")),
                                                                                                    oMensagem.Terms.dsTemplate,
                                                                                                    oMensagem.Terms.SepararMensagens);
                                        break;
                                    }
                                case "DadosDISTSfVenda":
                                    {
                                        sMensagem = FlexXTools.FlexXTools_DadosDISTSfVenda(ref oMensagem,
                                                                                           oConfig.oBancoDados,
                                                                                           oMensagem.idTarefa,
                                                                                           oMensagem.Terms.StringConexao,
                                                                                           oMensagem.Terms.Cod_Puxada,
                                                                                           oMensagem.Terms.WS_TipoRegistro,
                                                                                           _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid),
                                                                                           oMensagem.Terms.WS_TipoConsulta,
                                                                                           oMensagem.Terms.dsTemplate,
                                                                                           "",
                                                                                           oMensagem.Terms.SepararMensagens);
                                        break;
                                    }
                                case "DadosDISTSfCobertura":
                                    {
                                        sMensagem = FlexXTools.FlexXTools_DadosDisSfCobertura(ref oMensagem,
                                                                                              oConfig.oBancoDados,
                                                                                              oMensagem.idTarefa,
                                                                                              oMensagem.Terms.StringConexao,
                                                                                              oMensagem.Terms.Cod_Puxada,
                                                                                              oMensagem.Terms.WS_TipoRegistro,
                                                                                              _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid),
                                                                                              oMensagem.Terms.WS_TipoConsulta,
                                                                                              oMensagem.Terms.dsTemplate,
                                                                                              "", //oMensagem.Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                                              oMensagem.Terms.SepararMensagens);
                                        break;
                                    }
                                case "DadosDISTSfVendas_Grupo":
                                    {
                                        sMensagem = FlexXTools.FlexXTools_DadosDISTSfVendasGrupo(ref oMensagem,
                                                                                                 oConfig.oBancoDados,
                                                                                                 oMensagem.idTarefa,
                                                                                                 oMensagem.Terms.StringConexao,
                                                                                                 oMensagem.Terms.Cod_Puxada,
                                                                                                 oMensagem.Terms.WS_TipoRegistro,
                                                                                                 _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid),
                                                                                                 oMensagem.Terms.WS_TipoConsulta,
                                                                                                 oMensagem.Terms.dsTemplate,
                                                                                                 "", //oMensagem.Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                                                 oMensagem.Terms.SepararMensagens);
                                        break;
                                    }
                                case "DadosDISTSfIV_IAV":
                                    {
                                        sMensagem = FlexXTools.FlexXTools_DadosDisSfIv_Iav(ref oMensagem,
                                                                                           oConfig.oBancoDados,
                                                                                           oMensagem.idTarefa,
                                                                                           oMensagem.Terms.StringConexao,
                                                                                           oMensagem.Terms.Cod_Puxada,
                                                                                           oMensagem.Terms.WS_TipoRegistro,
                                                                                           _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid),
                                                                                           oMensagem.Terms.WS_TipoConsulta,
                                                                                           oMensagem.Terms.dsTemplate,
                                                                                           "", //oMensagem.Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                                           oMensagem.Terms.SepararMensagens);
                                        break;
                                    }
                                case "DadosDISTSfDevolucao":
                                    {
                                        sMensagem = FlexXTools.FlexXTools_DadosDISTSfDevolucao(ref oMensagem,
                                                                                               oConfig.oBancoDados,
                                                                                               oMensagem.idTarefa,
                                                                                               oMensagem.Terms.StringConexao,
                                                                                               oMensagem.Terms.Cod_Puxada,
                                                                                               oMensagem.Terms.WS_TipoRegistro,
                                                                                               _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid),
                                                                                               oMensagem.Terms.WS_TipoConsulta,
                                                                                               oMensagem.Terms.dsTemplate,
                                                                                               "", //oMensagem.Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                                               oMensagem.Terms.SepararMensagens);
                                        break;
                                    }
                                case "BOOMERANGUE_PURINA":
                                    {
                                        sMensagem = FlexXTools.FlexXTools_BOOMERANGUE_PURINA(ref oConfig,
                                                                                             ref oMensagem,
                                                                                             oMensagem.idTarefa,
                                                                                             oMensagem.Terms.StringConexao,
                                                                                             _Funcoes.FNC_FormatarTelefone(oMensagem.ParaLista),
                                                                                             oMensagem.Terms.dsTemplate);

                                        break;
                                    }
                            }

                            break;
                        }
                }

                if (NovaTerms == null)
                    break;
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_BancoDados_Tems, "Processado");

            return sMensagem;
        }

        public static string Custom_uid_Novo(ref Mensagem oMensagem, ref Config oConfig, ref int iId_bot_requisicao)
        {
            string sCustom_uid = "";

            oConfig.oBancoDados.DBProcedure("sp_bot_requisicao_new", new clsCampo[] {
                                                                    new clsCampo {Nome = "idBot", Tipo = DbType.Double, Valor = oMensagem.idBot},
                                                                    new clsCampo {Nome = "id_bot_requisicao", Tipo = DbType.Double, Valor = null, Direcao = ParameterDirection.Output },
                                                                    new clsCampo {Nome = "custom_uid", Tipo = DbType.String, Valor = null, Direcao = ParameterDirection.Output }});

            if (oConfig.oBancoDados.Retorno.ParametroRetorno != null)
            {
                iId_bot_requisicao = Convert.ToInt32(oConfig.oBancoDados.Retorno.Campo("id_bot_requisicao").Valor);
                sCustom_uid = oConfig.oBancoDados.Retorno.Campo("custom_uid").Valor.ToString();
            }

            return sCustom_uid;
        }

        public static void Custom_uid_Atualizar(ref Config oConfig,
                                                int iId_bot_requisicao,
                                                string p_status)
        {
            oConfig.oBancoDados.DBProcedure("sp_bot_requisicao_upd", new clsCampo[] {
                                                                    new clsCampo {Nome = "id_bot_requisicao", Tipo = DbType.Double, Valor = iId_bot_requisicao},
                                                                    new clsCampo {Nome = "status", Tipo = DbType.String, Valor = p_status }});
        }

        public static void MessageSend(ref Config oConfig,
                                       ref Mensagem oMensagem,
                                       string custom_uid,
                                       string message_body,
                                       string message_caption,
                                       string message_response_Custon_uid,
                                       int iIdMessageTerms)
        {
            oConfig.oBancoDadosBot.DBProcedure("sp_messagesend_ins", new clsCampo[] {
                                                                    new clsCampo {Nome = "idMessage", Tipo = DbType.Double, Valor = oMensagem.idMensagem},
                                                                    new clsCampo {Nome = "custom_uid", Tipo = DbType.String, Valor = _Funcoes.FNC_NuloString(custom_uid)},
                                                                    new clsCampo {Nome = "message_body", Tipo = DbType.String, Valor = message_body},
                                                                    new clsCampo {Nome = "message_response_OK", Tipo = DbType.Double, Valor = oMensagem.idStatusMensagem},
                                                                    new clsCampo {Nome = "message_caption", Tipo = DbType.String, Valor = message_caption},
                                                                    new clsCampo {Nome = "message_response_Custon_uid", Tipo = DbType.String, Valor = _Funcoes.FNC_NuloString(custom_uid)},
                                                                    new clsCampo {Nome = "idStatusMensagem", Tipo = DbType.Double, Valor = oMensagem.idStatusMensagem},
                                                                    new clsCampo {Nome = "idMessageTerms", Tipo = DbType.Double, Valor = iIdMessageTerms},
                                                                    new clsCampo {Nome = "Processador", Tipo = DbType.String, Valor = Declaracao.processador}});
        }

        public static Boolean EnviarTexto(ref Mensagem oMensagem,
                                          ref Config oConfig,
                                          string[] sMensagem,
                                          bool bEnviar = true)
        {
            bool bOk = false;
            //ttps://www.waboxapp.com/api/send/chat?token=3850ae881c808e24a5c8281dadf15c2f5bf561a267c02&uid=16288000515&to=557399009349&custom_uid=flag_1811211216_012&text=Hello world!'

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem, "Enviado");

            try
            {
                if (sMensagem.Length == 1)
                {
                    if (sMensagem[0] == Constantes.const_Template_Command_SEMACESSO ||
                        sMensagem[0] == Constantes.const_Template_Command_OFFLINE ||
                        sMensagem[0] == Constantes.const_Template_Command_ERRO998 ||
                        sMensagem[0] == Constantes.const_Template_Command_AJUDAERRO ||
                        sMensagem[0] == Constantes.const_Template_Command_SERVICOFORA)
                    {
                        try
                        {
                            if (oMensagem.Terms != null)
                            {
                                oMensagem.Terms = Declaracao.otbmessageterms.PesquisarPorTermo(ref oConfig, ref oMensagem, sMensagem[0], true);
                                sMensagem[0] = TratarTexto(ref oConfig, ref oMensagem, oMensagem.Terms.dsTemplate, oMensagem.contactname);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                string sAux = "";

                foreach (string sMens in sMensagem)
                {
                    sAux = sMens;

                    if (oMensagem.Terms != null)
                    {
                        if (oMensagem.Terms.dsTemplateHeader.Trim() != "") { sAux = oMensagem.Terms.dsTemplateHeader + Environment.NewLine + sAux; }
                        if (oMensagem.Terms.dsTemplateFooter.Trim() != "") { sAux = sAux + Environment.NewLine + oMensagem.Terms.dsTemplateFooter; }
                    }

                    bOk = EnviarTexto_Provider(ref oMensagem, ref oConfig, sAux, bEnviar);

                    oMensagem.Terms = oMensagem.Terms;
                }
            }
            catch (Exception Ex1)
            {
                oConfig.oBancoDados.DBSQL_Log_Gravar(0, "EnviarTexto", "B", Ex1.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem, "Enviado");

            return bOk;
        }

        public static Boolean EnviarTexto_Provider(ref Mensagem oMensagem, ref Config oConfig, string sMensagem, bool bEnviar = true)
        {
            bool bOk = false;
            string[] sLista = null;

            if (_Funcoes.FNC_NuloString(oMensagem.status) == Constantes.const_Status_PendenteConfirmacao)
            {
                oMensagem.ParaLista = "";
                sMensagem = "Esse comando necessita de confirmação. "
                            + Environment.NewLine + Environment.NewLine +
                            " Você confirmar [SIM] a execução do comando [" + oMensagem.command.Trim() + "]?";
            }

            if (_Funcoes.FNC_NuloString(oMensagem.ParaLista).Trim() != "")
            {
                if (_Funcoes.FNC_NuloString(oMensagem.ParaLista).Trim().Contains(";"))
                {
                    sLista = oMensagem.ParaLista.Split(new char[] { ';' });
                }
                else
                {
                    sLista = new string[] { oMensagem.ParaLista };
                }
            }
            else
            {
                sLista = new string[] { oMensagem.contactuid };
            }

            oMensagem.messagemtd = DateTime.Now;
            oMensagem.messagemtdd = DateTime.Now;

            foreach (string Para in sLista)
            {
                if (oMensagem.idBot == 0)
                {
                    oMensagem.Custom_uid = "";
                }
                else
                {
                    oMensagem.Custom_uid = Bot.Custom_uid_Novo(ref oMensagem, ref oConfig, ref oMensagem.idbot_requisicao);
                }

                sMensagem = Bot.TratarTexto(ref oConfig, ref oMensagem, sMensagem, oMensagem.Custom_uid);
                oMensagem.To = _Funcoes.FNC_FormatarTelefone(Para);

                if (bEnviar)
                {
                    switch (oConfig.Provider.ToUpper().Trim())
                    {
                        case Constantes.const_Provider_Waboxapp:
                            {
                                bOk = EnviarTexto_Waboxapp(ref oConfig, ref oMensagem, sMensagem, oMensagem.Custom_uid);
                                break;
                            }
                        case Constantes.const_Provider_ChartAPI:
                            {
                                bOk = EnviarTexto_ChatAPI(ref oConfig, ref oMensagem, sMensagem);
                                break;
                            }
                        case Constantes.const_Provider_Telegram:
                            {
                                bOk = EnviarTexto_Telegram(ref oMensagem, ref oConfig, sMensagem);
                                break;
                            }
                        case Constantes.const_Provider_BTrive:
                            {
                                bOk = EnviarTexto_BTrive(ref oConfig, ref oMensagem, sMensagem);
                                break;
                            }
                    }
                }
            }

            if (sLista.Length == 1)
                oMensagem.contactuid = sLista[0].ToString();
            else
                oMensagem.contactuid = "*";

            oMensagem.messagebody_response = sMensagem;
            oMensagem.messagedtm = DateTime.Now;
            oMensagem.messagedtmd = DateTime.Now;

            return bOk;
        }

        public static Boolean EnviarTexto_Telegram(ref Mensagem oMensagem, ref Config oConfig, string sMens)
        {
            bool bOk = false;

            try
            {
                TelegramBot.TelegramBot bot = new TelegramBot.TelegramBot(oMensagem.Token);

                TelegramBot.MessageToSend Mens = new MessageToSend();
                Task<TelegramBot.ResponseObjects.MessageResponse> oRet;

                Mens.ChatID = oMensagem.To;
                Mens.Text = sMens;

                oRet = bot.SendMessageAsync(Mens);

                //string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
                //urlString = String.Format(urlString, Mensagem.Token, Mensagem.To, sMens);
                //WebRequest request = WebRequest.Create(urlString);
                //Stream rs = request.GetResponse().GetResponseStream();
                //StreamReader reader = new StreamReader(rs);
                //string line = "";
                //StringBuilder sb = new StringBuilder();
                //while (line != null)
                //{
                //    line = reader.ReadLine();
                //    if (line != null)
                //        sb.Append(line);
                //}
                //string response = sb.ToString();

                bOk = true;
            }
            catch (Exception Ex1)
            {
                oConfig.oBancoDados.DBSQL_Log_Gravar(0, "EnviarTexto_Telegram", "B", Ex1.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem, "Enviado");

            return bOk;
        }

        public static Boolean EnviarTexto_BTrive(ref Config oConfig,
                                                 ref Mensagem oMensagem,
                                                 string sMens)
        {
            bool Ret = false;
            bool EnviarImagem = false;

            if (oMensagem.Terms != null)
            {
                EnviarImagem = (_Funcoes.FNC_NuloString(oMensagem.Terms.dsUrl_Image) != "");
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem_Waboxapp, "Enviando");

            try
            {
                RestClient client;
                RestRequest request;
                string client_id = oMensagem.Token;
                string secret = oMensagem.Token2;
                string type;
                string device_id;
                string recipient;
                string legend;
                IRestResponse response;

                if (EnviarImagem)
                {
                    client = new RestClient(oConfig.ChatImage_Url);
                    request = new RestRequest(Method.POST);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                    type = "image";
                    string file_url = oMensagem.Terms.dsUrl_Image;
                    device_id = oMensagem.idBotExterno;
                    legend = sMens;
                    recipient = _Funcoes.FNC_FormatarTelefone(oMensagem.To, true);
                    request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"client_id\"\r\n\r\n" + client_id + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"secret\"\r\n\r\n" + secret + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"device_id\"\r\n\r\n" + device_id + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"type\"\r\n\r\n" + type + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"legend\"\r\n\r\n" + legend + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file_url\"\r\n\r\n" + file_url + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"recipient\"\r\n\r\n" + recipient + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW", ParameterType.RequestBody);
                    response = client.Execute(request);
                }
                else
                {
                    type = "text";
                    string message = sMens;
                    device_id = oMensagem.idBotExterno;
                    recipient = _Funcoes.FNC_FormatarTelefone(oMensagem.To, true);
                    client = new RestClient(oConfig.Chat_Url + "?client_id=" + client_id + "&secret=" + secret + "&type=" + type + "&message=" + message + "&device_id=" + device_id + "&recipient=" + recipient);
                    request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    response = client.Execute(request);
                }

            }
            catch (Exception Ex)
            {
                oConfig.oBancoDados.DBSQL_Log_Gravar(0, "EnviarTexto_Waboxapp", "B", Ex.Message); ;
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem_Waboxapp, "Enviado");

            return Ret;
        }

        public static Boolean EnviarTexto_Waboxapp(ref Config oConfig,
                                                   ref Mensagem oMensagem,
                                                   string sMens,
                                                   string sCustom_uid)
        {
            bool Ret = false;
            bool EnviarImagem = false;
            string sChat_URL = "";

            if (oMensagem.Terms != null)
            {
                EnviarImagem = (_Funcoes.FNC_NuloString(oMensagem.Terms.dsUrl_Image) != "");
            }

            //if (EnviarImagem)
            //{
            //    sChat_URL = "http://whats.plugthink.com/sms/api?action=send-sms&api_key=TnFFYWxEYXF6eEppeEt2bHFJPUI=&mms=1&unicode=1&to=" + oMensagem.To + "&from=" + oMensagem.Uid + "&sms=" + sMens + "&media_url=" + oMensagem.Terms.dsUrl_Image;
            //}
            //else
            //{
            //    sChat_URL = "http://whats.plugthink.com/sms/api?action=send-sms&api_key=TnFFYWxEYXF6eEppeEt2bHFJPUI=&unicode=1&to=" + oMensagem.To + "&from=" + oMensagem.Uid + "&sms=" + sMens;
            //}

            ////sChat_URL = "http://whats.plugthink.com/sms/api?action=send-sms&api_key=PWZMbmlySmtNSnFoQklHZUVtbEQ=&to=" + oMensagem.To + "&from=551152420209&mms=1&sms=teste&unicode=1&media_url=http%3A%2F%2Folapdv.flagia.com.br%2Fftp_fotos%2FBoomerangue%2Fpurina%2Fbm_purina_0001.png";

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sChat_URL);
            //WaboxApp_WhatsApp WaboxApp_WhatsApp_Root;

            //request.Method = "POST";
            //request.ContentType = "text/xml; charset=utf-8";
            //request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //string content = "";

            //using (var response = (HttpWebResponse)request.GetResponse())
            //{
            //    using (var stream = response.GetResponseStream())
            //    {
            //        using (var sr = new StreamReader(stream))
            //        {
            //            content = sr.ReadToEnd();
            //            WaboxApp_WhatsApp_Root = JsonConvert.DeserializeObject<WaboxApp_WhatsApp>(content);
            //        }
            //    }
            //}

            if (EnviarImagem)
            {
                sChat_URL = oConfig.ChatImage_Url + "?token=" + oMensagem.Token + "&uid=" + oMensagem.Uid + "&to=" + oMensagem.To + "&custom_uid=" + sCustom_uid + "&url=" + oMensagem.Terms.dsUrl_Image + "&caption=" + sMens;
            }
            else
            {
                sChat_URL = oConfig.Chat_Url + "?token=" + oMensagem.Token + "&uid=" + oMensagem.Uid + "&to=" + oMensagem.To + "&custom_uid=" + sCustom_uid + "&text=" + sMens;
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem_Waboxapp, "Enviando");

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sChat_URL);
                WaboxApp_WhatsApp WaboxApp_WhatsApp_Root;

                request.Method = "POST";
                //request.ContentType = "application/json; charset=utf-8";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string content = "";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                            WaboxApp_WhatsApp_Root = JsonConvert.DeserializeObject<WaboxApp_WhatsApp>(content);
                        }
                    }
                }

                if (WaboxApp_WhatsApp_Root != null)
                {
                    oMensagem.idStatusMensagem = 0;

                    if (WaboxApp_WhatsApp_Root.success) { oMensagem.idStatusMensagem = 1; } else { oMensagem.idStatusMensagem = 9; }

                    Ret = WaboxApp_WhatsApp_Root.success;
                }
            }
            catch (Exception Ex)
            {
                oConfig.oBancoDados.DBSQL_Log_Gravar(0, "EnviarTexto_Waboxapp", "B", Ex.Message); ;
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem_Waboxapp, "Enviado");

            return Ret;
        }

        public static Boolean EnviarTexto_ChatAPI(ref Config oConfig,
                                                  ref Mensagem oMensagem,
                                                  string sMens)
        {
            bool bOk = false;
            string sChat_URL = "";
            bool EnviarImagem = false;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem_ChatAPI, "Enviando");

            if (oMensagem.Terms != null)
            {
                oMensagem.dsUrl_Image = _Funcoes.FNC_NuloString(oMensagem.Terms.dsUrl_Image);
            }

            EnviarImagem = (_Funcoes.FNC_NuloString(oMensagem.dsUrl_Image) != "");

            if (EnviarImagem)
            {
                sChat_URL = oConfig.Chat_Url + "/sendFile?token=" + oMensagem.Token;
            }
            else
            {
                sChat_URL = oConfig.Chat_Url + "/message?token=" + oMensagem.Token;
            }

            try
            {
                if (EnviarImagem)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sChat_URL);

                    request.Method = "POST";
                    request.ContentType = "application/json; charset=utf-8";
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        if (oMensagem.To.IndexOf("@") == -1)
                        {
                            if (!oMensagem.ToDirect)
                                oMensagem.To = oMensagem.To + "@c.us";
                        }

                        if (oMensagem.ToDirect)
                        {
                            string json = new JavaScriptSerializer().Serialize(new
                            {
                                phone = oMensagem.To,
                                body = sMens
                            });
                            streamWriter.Write(json);
                        }
                        else
                        {
                            string json = new JavaScriptSerializer().Serialize(new 
                            {
                                chatId = oMensagem.To,
                                caption = sMens,
                                body = oMensagem.dsUrl_Image,
                                filename = _Funcoes.FNC_NomeArquivo(oMensagem.dsUrl_Image, "/")
                            });
                            streamWriter.Write(json);
                        }

                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }
                else
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sChat_URL);

                    request.Method = "POST";
                    request.ContentType = "application/json; charset=utf-8";
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        if (oMensagem.To.IndexOf("@") == -1)
                        {
                            if (!oMensagem.ToDirect)
                                oMensagem.To = oMensagem.To + "@c.us";
                        }

                        if (oMensagem.ToDirect)
                        {
                            string json = new JavaScriptSerializer().Serialize(new
                            {
                                phone = oMensagem.To,
                                body = sMens
                            });
                            streamWriter.Write(json);
                        }
                        else
                        {
                            string json = new JavaScriptSerializer().Serialize(new
                            {
                                chatId = oMensagem.To,
                                body = sMens
                            });
                            streamWriter.Write(json);
                        }

                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }

                bOk = true;
            }
            catch (Exception Ex)
            {
                oConfig.oBancoDados.DBSQL_Log_Gravar(0, "EnviarTexto_ChatAPI", "ERRO", sChat_URL + "[" + oMensagem.To + "] - " + Ex.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_Mensagem_ChatAPI, "Enviado");

            return bOk;
        }
    }

    public static class FlexXTools
    {
        public static string[] FlexXTools_DadosBRK_CanalRemuneracao(ref Mensagem oMensagem,
                                                                    clsBancoDados oBancoDados,
                                                                    long iIdTarefa,
                                                                    string sConexao,
                                                                    string sCOD_PUXADA,
                                                                    string sTemplate,
                                                                    bool Separar)
        {
            string[] sTexto = null;
            int iLinha = -1;
            clsBancoDados oBancoDadosOrigem = new clsBancoDados();
            DataTable oData;
            string sSql;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVenda - Início");

            try
            {
                oMensagem.messagerqt = DateTime.Now;
                oMensagem.messagerqtd = DateTime.Now;

                Declaracao.DROP_TOTAL = 0;
                Declaracao.REMUNERACAO_TOTAL = 0;

                oBancoDadosOrigem.DBConectar(Constantes.const_TipoBancoDados_SqlServer, sConexao);

                sSql = "select CANAL_REMUNERACAO as nome," +
                              "concat('Drop ', QUANTIDADE_DROPS) as descricao," +
                              "QUANTIDADE_DROPS," +
                              "concat(PCT_PARTICIPACAO_TOTAL, ' %') as info," +
                              "REMUNERACAO_DROP as valor" +
                       " from trade2up_dashboard.dbo.FlexxPowerNestleDropsCanal" +
                       " where tipo_empregado = 'empresa'" +
                         " and cod_puxada = '" + sCOD_PUXADA.Trim() + "'" +
                       " order by REMUNERACAO_DROP desc";
                oData = oBancoDadosOrigem.DBQuery(sSql);

                oMensagem.messagerstd = DateTime.Now;

                if (Separar) { sTexto = new string[oData.Rows.Count]; }
                { sTexto = new string[1]; }

                foreach (DataRow oRow in oData.Rows)
                {
                    if (Separar) { iLinha = iLinha + 1; }
                    { iLinha = 0; }

                    if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                    Declaracao.DROP_TOTAL = Declaracao.DROP_TOTAL + Convert.ToDouble(oRow["QUANTIDADE_DROPS"]);
                    Declaracao.REMUNERACAO_TOTAL = Declaracao.REMUNERACAO_TOTAL + Convert.ToDouble(oRow["VALOR"]);

                    sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "NOME", oRow["NOME"]);
                    sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "DESCR", oRow["NOME"]);
                    sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "INFO", oRow["INFO"]);
                    sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "VALOR", oRow["VALOR"]);
                }
            }
            catch (Exception Ex)
            {
                sTexto = new string[1];
                sTexto[0] = Constantes.const_Template_Command_SERVICOFORA;
                oBancoDados.DBSQL_Log_Gravar(0, "FlexXTools_DadosBRK_CanalRemuneracao", "B", Ex.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVenda - Fim");

            return sTexto;
        }

        public static string[] FlexXTools_DadosDISTSfVenda(ref Mensagem oMensagem,
                                                           clsBancoDados oBancoDados,
                                                           long iIdTarefa,
                                                           string sConexao,
                                                           string sCOD_PUXADA,
                                                           string sTIPO_REGISTRO,
                                                           string sTEL_CELULAR,
                                                           string sTIPO_CONSULTA,
                                                           string sTemplate,
                                                           string json = "",
                                                           Boolean Separar = false)
        {
            string[] sTexto = null;
            int iLinha = -1;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVenda - Início");

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                DadosDISTSfVendaRoot DadosDISTSfVenda_Root;

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Constantes.FlexXTools_Usuario + ":" + Constantes.FlexXTools_Senha)));
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (json.Trim() == "")
                    json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                            "\"TEL_CELULAR\":\"" + _Funcoes.FNC_FormatarTelefone(sTEL_CELULAR) + "\"," +
                            "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

                request.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var content = string.Empty;

                oMensagem.messagerqt = DateTime.Now;
                oMensagem.messagerqtd = DateTime.Now;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                            DadosDISTSfVenda_Root = JsonConvert.DeserializeObject<DadosDISTSfVendaRoot>(content);
                        }
                    }
                }
                oMensagem.messagerst = DateTime.Now;
                oMensagem.messagerstd = DateTime.Now;

                if (DadosDISTSfVenda_Root != null)
                {
                    if (Bot.ValidarRetornoJSon(ref oMensagem, DadosDISTSfVenda_Root.codigo, DadosDISTSfVenda_Root.mensagem, ref sTexto))
                    {
                        if (DadosDISTSfVenda_Root.DadosDISTSfVenda.Count > 0)
                        {
                            if (Separar) { sTexto = new string[DadosDISTSfVenda_Root.DadosDISTSfVenda.Count]; }
                            { sTexto = new string[1]; }

                            foreach (DadosDISTSfVenda Item in DadosDISTSfVenda_Root.DadosDISTSfVenda)
                            {
                                if (Separar) { iLinha = iLinha + 1; }
                                { iLinha = 0; }

                                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "VALOR_HECTO", Item.VALOR_HECTO);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "VALOR_VENDA", Item.VALOR_VENDA);
                            }
                        }
                    }
                }

                request = null;
            }
            catch (Exception Ex)
            {
                sTexto = new string[1];
                sTexto[0] = Constantes.const_Template_Command_SERVICOFORA;
                oBancoDados.DBSQL_Log_Gravar(0, "FlexXTools_DadosDISTSfVenda", "B", Ex.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVenda - Fim");

            return sTexto;
        }

        public static String FlexXTools_DataTable1(string sConexao,
                                                   string json,
                                                   string sFlexXTools_Usuario = "",
                                                   string sFlexXTools_Senha = "",
                                                   string sFlexXTools_Basic = "")
        {
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(sConexao); request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            // turn our request string into a byte stream
            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            // this is important - make sure you specify type this way
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;
            if (sFlexXTools_Basic.Trim() == "")
            { request.Headers.Add("Authorization", "Basic " + sFlexXTools_Basic ); }
            else
            { request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sFlexXTools_Usuario + ":" + sFlexXTools_Senha))); }            
            Stream requestStream = request.GetRequestStream();

            // now send it
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            // grab te response and print it out to the console along with the status code
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result;
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }

            return result;
        }

        public static DataTable FlexXTools_DataTable(long iIdTarefa,
                                                     string sChave,
                                                     string sConexao,
                                                     string sCOD_PUXADA,
                                                     string sTEL_CELULAR,
                                                     string sTIPO_REGISTRO,
                                                     string sTIPO_CONSULTA,
                                                     string sVISAO_FATURAMENTO,
                                                     string json = "",
                                                     string sFlexXTools_Usuario = "",
                                                     string sFlexXTools_Senha = "",
                                                     bool SemTts = false)
        {
            DataTable oDataTable = null;
            DateTime dUtil = DateTime.Now;

            if (SemTts)
            {
            }
            else
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Ssl3;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

            try
            {
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                if (sFlexXTools_Usuario.Trim() != "")
                {
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sFlexXTools_Usuario + ":" + sFlexXTools_Senha)));
                }
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (json.Trim() == "" && sCOD_PUXADA.Trim() != "")
                {
                    if (json.Trim() == "" && sTIPO_REGISTRO.Trim() != "")
                    {
                        json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                                "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                                "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                                "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";
                    }
                    else
                    {
                        json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"}";
                    }
                }

               // now send it
                try
                {
                    request.ContentLength = json.Length;

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                catch (Exception)
                {
                    byte[] postBytes = Encoding.UTF8.GetBytes(json);

                    Stream streamWriter = request.GetRequestStream();

                    streamWriter.Write(postBytes, 0, postBytes.Length);
                    streamWriter.Close();
                }

                var content = string.Empty;

                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();

                            object my_obj;

                            try
                            {
                                my_obj = JsonConvert.DeserializeObject<JObject>(content.ToString());
                                Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

                                foreach (KeyValuePair<string, JToken> Item in dict)
                                {
                                    if (Item.Key == sChave)
                                    {
                                        oDataTable = JsonConvert.DeserializeObject<DataTable>(Item.Value.ToString());
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                oDataTable = toDataTable(content.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
            }
            finally
            {
                request = null;
            }

            TimeSpan ts = DateTime.Now - dUtil;

            return oDataTable;
        }
        private static DataTable toDataTable(string json)
        {
            var result = new DataTable();
            var jArray = JArray.Parse(json);
            //Initialize the columns, If you know the row type, replace this   
            foreach (var row in jArray)
            {
                foreach (var jToken in row)
                {
                    var jproperty = jToken as JProperty;
                    if (jproperty == null) continue;
                    if (result.Columns[jproperty.Name] == null)
                        result.Columns.Add(jproperty.Name, typeof(string));
                }
            }
            foreach (var row in jArray)
            {
                var datarow = result.NewRow();
                foreach (var jToken in row)
                {
                    var jProperty = jToken as JProperty;
                    if (jProperty == null) continue;
                    datarow[jProperty.Name] = jProperty.Value.ToString();
                }
                result.Rows.Add(datarow);
            }

            return result;
        }

        public static string FlexXTools_AtivarTelefone(string sCOD_PUXADA,
                                                              string sTIPO_VISAO,
                                                              string sTEL_CELULAR,
                                                              string sNOME,
                                                              string sSENHA,
                                                              string sCOD_EMPREGADO)
        {
            string[] sTexto = null;
            string sStatus = "";
            string json;

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.flexxtools.com.br/Flag.WS.Whatsapp/WSMovimenta.svc/AtivarTelefone");

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic ZmxhZ3doYXRzNDI4MGVuOk9kZm9mb3Q1OWZsYWcyMzQ0OTU5");
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                json = "{\"TIPO_VISAO\":\"" + sTIPO_VISAO + "\"," +
                        "\"TEL_CELULAR\":\"" + _Funcoes.FNC_FormatarTelefone(sTEL_CELULAR) + "\"," +
                        "\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                        "\"NOME\":\"" + sNOME + "\"," +
                        "\"SENHA\":\"" + sSENHA + "\"," +
                        "\"COD_EMPREGADO\":\"" + sCOD_EMPREGADO + "\"," +
                        "\"EMAIL\":\"" + "flag@flag.com.br" + "\"," +
                        "\"CNPJ\":\"" + "00000000000000" + "\"," +
                        "\"SOFIA_ATIVO\":\"" + "1" + "\"," +
                        "\"FLEXXPOWER_ATIVO\":\"" + "0" + "\"," +
                        "\"IND_MASTER_TI\":\"" + "0" + "\"," +
                        "\"IND_CONTROLLER_EMPRESA\":\"" + "0" + "\"}";

                request.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var content = string.Empty;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();

                            object my_obj;

                            try
                            {
                                my_obj = JsonConvert.DeserializeObject<JObject>(content.ToString());
                                Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

                                foreach (KeyValuePair<string, JToken> Item in dict)
                                {
                                    if (Item.Key.ToUpper() == "CODIGO")
                                    {
                                        sStatus = Item.Value.ToString();

                                        switch (sStatus)
                                        {
                                            case "0":
                                                sStatus = sStatus + " telefone ativado com sucesso";
                                                break;
                                            case "1":
                                                sStatus = sStatus + " código de puxada não existe";
                                                break;
                                            case "2":
                                                sStatus = sStatus + " telefone já cadastrado";
                                                break;
                                            case "3":
                                                sStatus = sStatus + " telefone não inserido";
                                                break;
                                            case "999":
                                                sStatus = sStatus + " erro inesperado/não tratado";
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                request = null;
            }
            catch (Exception Ex)
            {
                sTexto = new string[1];
                sTexto[0] = Constantes.const_Template_Command_SERVICOFORA;
            }

            return sStatus;
        }

        public static string[] FlexXTools_DadosDisSfCobertura(ref Mensagem oMensagem,
                                                              clsBancoDados oBancoDados,
                                                              long iIdTarefa,
                                                              string sConexao,
                                                              string sCOD_PUXADA,
                                                              string sTIPO_REGISTRO,
                                                              string sTEL_CELULAR,
                                                              string sTIPO_CONSULTA,
                                                              string sTemplate,
                                                              string json = "",
                                                              Boolean Separar = false)
        {
            string[] sTexto = null;
            int iLinha = -1;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfCobertura - Início");

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                DadosDISTSfCoberturaRoot DadosDISTSfCobertura_Root;

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Constantes.FlexXTools_Usuario + ":" + Constantes.FlexXTools_Senha)));
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (json.Trim() == "")
                    json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                            "\"TEL_CELULAR\":\"" + _Funcoes.FNC_FormatarTelefone(sTEL_CELULAR) + "\"," +
                            "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

                request.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var content = string.Empty;

                oMensagem.messagerqt = DateTime.Now;
                oMensagem.messagerqtd = DateTime.Now;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                            DadosDISTSfCobertura_Root = JsonConvert.DeserializeObject<DadosDISTSfCoberturaRoot>(content);
                        }
                    }
                }
                oMensagem.messagerst = DateTime.Now;
                oMensagem.messagerstd = DateTime.Now;

                if (DadosDISTSfCobertura_Root != null)
                {
                    if (Bot.ValidarRetornoJSon(ref oMensagem, DadosDISTSfCobertura_Root.codigo, DadosDISTSfCobertura_Root.mensagem, ref sTexto))
                    {
                        if (DadosDISTSfCobertura_Root.DadosDISTSfCobertura.Count > 0)
                        {
                            if (Separar) { sTexto = new string[DadosDISTSfCobertura_Root.DadosDISTSfCobertura.Count]; }
                            { sTexto = new string[1]; }

                            foreach (DadosDISTSfCobertura Item in DadosDISTSfCobertura_Root.DadosDISTSfCobertura)
                            {
                                if (Separar) { iLinha = iLinha + 1; }
                                { iLinha = 0; }

                                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "DSC_GRUPO", Item.DSC_GRUPO);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_COBERTURA", Item.QTDE_COBERTURA);

                            }
                        }
                    }
                }

                request = null;
            }
            catch (Exception Ex)
            {
                sTexto = new string[1];
                sTexto[0] = Constantes.const_Template_Command_SERVICOFORA;
                oBancoDados.DBSQL_Log_Gravar(0, "FlexXTools_DadosDisSfCobertura", "B", Ex.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfCobertura - Fim");

            return sTexto;
        }

        public static string[] FlexXTools_DadosDISTSfVendasGrupo(ref Mensagem oMensagem,
                                                                 clsBancoDados oBancoDados,
                                                                 long iIdTarefa,
                                                                 string sConexao,
                                                                 string sCOD_PUXADA,
                                                                 string sTIPO_REGISTRO,
                                                                 string sTEL_CELULAR,
                                                                 string sTIPO_CONSULTA,
                                                                 string sTemplate,
                                                                 string json = "",
                                                                 Boolean Separar = false)
        {
            string[] sTexto = null;
            int iLinha = -1;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVendasGrupo - Início");

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                DadosDISTSfVendasGrupoRoot DadosDISTSfVendasGrupo_Root;

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Constantes.FlexXTools_Usuario + ":" + Constantes.FlexXTools_Senha)));
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (json.Trim() == "")
                    json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                            "\"TEL_CELULAR\":\"" + _Funcoes.FNC_FormatarTelefone(sTEL_CELULAR) + "\"," +
                            "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

                request.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var content = string.Empty;

                oMensagem.messagerqt = DateTime.Now;
                oMensagem.messagerqtd = DateTime.Now;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                            DadosDISTSfVendasGrupo_Root = JsonConvert.DeserializeObject<DadosDISTSfVendasGrupoRoot>(content);
                        }
                    }
                }
                oMensagem.messagerst = DateTime.Now;
                oMensagem.messagerstd = DateTime.Now;

                if (DadosDISTSfVendasGrupo_Root != null)
                {
                    if (Bot.ValidarRetornoJSon(ref oMensagem, DadosDISTSfVendasGrupo_Root.codigo, DadosDISTSfVendasGrupo_Root.mensagem, ref sTexto))
                    {
                        if (DadosDISTSfVendasGrupo_Root.DadosDISTSfVendas_Grupo.Count > 0)
                        {
                            if (Separar) { sTexto = new string[DadosDISTSfVendasGrupo_Root.DadosDISTSfVendas_Grupo.Count]; }
                            { sTexto = new string[1]; }

                            foreach (DadosDISTSfVendasGrupo Item in DadosDISTSfVendasGrupo_Root.DadosDISTSfVendas_Grupo)
                            {
                                if (Separar) { iLinha = iLinha + 1; }
                                { iLinha = 0; }

                                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "DSC_GRUPO", Item.DSC_GRUPO);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "VALOR_HECTO", Item.VALOR_HECTO);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_VOLUME", Item.QTDE_VOLUME);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "VALOR_VENDA", Item.VALOR_VENDA);
                            }
                        }
                    }
                }

                request = null;
            }
            catch (Exception Ex)
            {
                sTexto = new string[1];
                sTexto[0] = Constantes.const_Template_Command_SERVICOFORA;
                oBancoDados.DBSQL_Log_Gravar(0, "FlexXTools_DadosDISTSfVendasGrupo", "B", Ex.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVendasGrupo - Fim");

            return sTexto;
        }

        public static string[] FlexXTools_DadosDisSfIv_Iav(ref Mensagem oMensagem,
                                                           clsBancoDados oBancoDados,
                                                           long iIdTarefa,
                                                           string sConexao,
                                                           string sCOD_PUXADA,
                                                           string sTIPO_REGISTRO,
                                                           string sTEL_CELULAR,
                                                           string sTIPO_CONSULTA,
                                                           string sTemplate,
                                                           string json = "",
                                                           Boolean Separar = false)
        {
            string[] sTexto = null;
            int iLinha = -1;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfIv_Iav - Início");

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                DadosDISTSfIVIAVRoot DadosDISTSfIVIAV_Root;

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Constantes.FlexXTools_Usuario + ":" + Constantes.FlexXTools_Senha)));
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (json.Trim() == "")
                    json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                            "\"TEL_CELULAR\":\"" + _Funcoes.FNC_FormatarTelefone(sTEL_CELULAR) + "\"," +
                            "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

                request.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var content = string.Empty;

                oMensagem.messagerqt = DateTime.Now;
                oMensagem.messagerqtd = DateTime.Now;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                            DadosDISTSfIVIAV_Root = JsonConvert.DeserializeObject<DadosDISTSfIVIAVRoot>(content);
                        }
                    }
                }
                oMensagem.messagerst = DateTime.Now;
                oMensagem.messagerstd = DateTime.Now;

                if (DadosDISTSfIVIAV_Root != null)
                {
                    if (Bot.ValidarRetornoJSon(ref oMensagem, DadosDISTSfIVIAV_Root.codigo, DadosDISTSfIVIAV_Root.mensagem, ref sTexto))
                    {
                        if (DadosDISTSfIVIAV_Root.DadosDISTSfIV_IAV.Count > 0)
                        {
                            if (Separar) { sTexto = new string[DadosDISTSfIVIAV_Root.DadosDISTSfIV_IAV.Count]; }
                            { sTexto = new string[1]; }

                            foreach (DadosDISTSfIVIAV Item in DadosDISTSfIVIAV_Root.DadosDISTSfIV_IAV)
                            {
                                if (Separar) { iLinha = iLinha + 1; }
                                { iLinha = 0; }

                                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "QTDE_VISITA_PREVISTA", Item.QTDE_VISITA_PREVISTA);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_IV", Item.QTDE_IV);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_IAV", Item.QTDE_IAV);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "PERCENTUAL_IV", Item.PERCENTUAL_IV);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "PERCENTUAL_IAV", Item.PERCENTUAL_IAV);
                            }
                        }
                    }
                }

                request = null;
            }
            catch (Exception Ex)
            {
                sTexto = new string[1];
                sTexto[0] = Constantes.const_Template_Command_SERVICOFORA;
                oBancoDados.DBSQL_Log_Gravar(0, "FlexXTools_DadosDisSfIv_Iav", "B", Ex.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfIv_Iav - Fim");

            return sTexto;
        }

        public static string[] FlexXTools_DadosDISTSfDevolucao(ref Mensagem oMensagem,
                                                               clsBancoDados oBancoDados,
                                                               long iIdTarefa,
                                                               string sConexao,
                                                               string sCOD_PUXADA,
                                                               string sTIPO_REGISTRO,
                                                               string sTEL_CELULAR,
                                                               string sTIPO_CONSULTA,
                                                               string sTemplate,
                                                               string json = "",
                                                               Boolean Separar = false)
        {
            string[] sTexto = null;
            int iLinha = -1;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfDevolucao - Início");

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                DadosDISTSfDevolucaoRoot DadosDISTSfDevolucao_Root;

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Constantes.FlexXTools_Usuario + ":" + Constantes.FlexXTools_Senha)));
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (json.Trim() == "")
                    json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                            "\"TEL_CELULAR\":\"" + _Funcoes.FNC_FormatarTelefone(sTEL_CELULAR) + "\"," +
                            "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

                request.ContentLength = json.Length;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var content = string.Empty;

                oMensagem.messagerqt = DateTime.Now;
                oMensagem.messagerqtd = DateTime.Now;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                            DadosDISTSfDevolucao_Root = JsonConvert.DeserializeObject<DadosDISTSfDevolucaoRoot>(content);
                        }
                    }
                }
                oMensagem.messagerst = DateTime.Now;
                oMensagem.messagerstd = DateTime.Now;

                if (DadosDISTSfDevolucao_Root != null)
                {
                    if (Bot.ValidarRetornoJSon(ref oMensagem, DadosDISTSfDevolucao_Root.codigo, DadosDISTSfDevolucao_Root.mensagem, ref sTexto))
                    {
                        if (DadosDISTSfDevolucao_Root.DadosDISTSfDevolucao.Count > 0)
                        {
                            if (Separar) { sTexto = new string[DadosDISTSfDevolucao_Root.DadosDISTSfDevolucao.Count]; }
                            { sTexto = new string[1]; }

                            foreach (DadosDISTSfDevolucao Item in DadosDISTSfDevolucao_Root.DadosDISTSfDevolucao)
                            {
                                if (Separar) { iLinha = iLinha + 1; }
                                { iLinha = 0; }

                                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "VALOR_DEVOLUCAO", Item.VALOR_DEVOLUCAO);
                                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "PERCENTUAL_DEVOLUCAO", Item.PERCENTUAL_DEVOLUCAO);
                            }
                        }
                    }
                }

                request = null;
            }
            catch (Exception Ex)
            {
                sTexto = new string[1];
                sTexto[0] = Constantes.const_Template_Command_SERVICOFORA;
                oBancoDados.DBSQL_Log_Gravar(0, "FlexXTools_DadosDISTSfDevolucao", "B", Ex.Message);
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfDevolucao - Fim");

            return sTexto;
        }

        public static string FlexxTools_BOOMERAGNGUE_PURINA_PEDIDO_FLAG(ref Config oConfig,
                                                                        ref Mensagem oMensagem,
                                                                        string sTEL_CELULAR,
                                                                        string sChaveEDI_Cliente,
                                                                        string sStatus)
        {
            string sRet = "";

            try
            {
                string sAPI = "http://clientes.flag.com.br/Flag.Boomerangue.WebApi/api/SetStatusPedido";

                //codigo descrição.
                //106       ERRO WS NÃO EXISTE
                //107       ERRO CLIENTES BLOQUEOU WS
                //108       ENVIO OK PARA O WS
                //109       CLIENTE LEU MSG WS
                //116       CLIENTES APROVOU PEDIDO

                var obj = new { id_pedido = sChaveEDI_Cliente, id_status = sStatus, TIPmessageO_CONSULTA = "Este é o primeiro teste de api boomerangue..." };

                JavaScriptSerializer js = new JavaScriptSerializer();
                string strJson = js.Serialize(obj);

                FlexXTools_DataTable1(sAPI, strJson, "", "", "d3NCb29tZXJhbmd1ZWZsYWdYaW0yMDoyZjAzYmM1OC0xZGJlLTQxNjAtOTA1NS0wOTk3Mjg1YzI2MmI=");
                //DataTable oData = FlexXTools_DataTable(0, "", sAPI, "", "", "", "", "", strJson, "wsBoomerangueflagXim20", "2f03bc58-1dbe-4160-9055-0997285c262b", true);

                try
                {
                    //sRet = oData.Rows[0]["retorno"].ToString();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception Ex)
            {
                oMensagem.InformarErro(Mensagem.const_Erro_ErroBoomeangue, Ex.Message);
            }

            return sRet;
        }

        public static void FlexXTools_BOOMERANGUE_PURINA_ACEITE(ref Config oConfig,
                                                                ref Mensagem oMensagem,
                                                                string sTEL_CELULAR,
                                                                string sPROTOCOLO,
                                                                double idMessage)
        {
            string sSql = "";
            DataTable oData;
            string sChaveEDI_Cliente = "";

            sPROTOCOLO = _Funcoes.FNC_Right("00000000" + sPROTOCOLO.Trim(), 6);

            sSql = "SELECT * FROM tb_Usuario_Boomerangue WHERE TELEFONE = '" + sTEL_CELULAR + "' AND DATA_ACEITE IS NULL AND DATA_CANCELADO IS NULL";
            oData = oConfig.oBancoDados.DBQuery(sSql);

            try
            {
                sChaveEDI_Cliente = oData.Rows[0]["ChaveEDI_Cliente"].ToString();
            }
            catch (Exception)
            {
            }

            sSql = "http://api02.plugthink.com/api/v2/boomerangue/_proc/sp_bot_bmWhats_acoes?sApp=boomerangue&sPartner=Flag&sToken=BM00015" +
                    "&sNomeBot=PURINA&sTelCliente=" + sTEL_CELULAR + "&sChaveBM=" + sPROTOCOLO + "&sAcao=VDC&sComplemento1=" + idMessage.ToString() + "&sComplemento2=&" +
                    "api_key=e14cb89ad01262944ba5e2780a1e2daa205d706a68c75d4cf73ce9746799a512";

            oData = FlexXTools_DataTable(0, "", sSql, "", sTEL_CELULAR, "", "", "");

            oMensagem.messagebody = oData.Rows[0]["Mensagem"].ToString();
            oMensagem.EnviarSemTratar = true;

            oConfig.oBancoDados.DBProcedure("SP_Usuario_Boomerangue_Aceite", new clsCampo[] {new clsCampo {Nome = "TELEFONE", Tipo = DbType.String, Valor = sTEL_CELULAR },
                                                                                             new clsCampo {Nome = "PROTOCOLO", Tipo = DbType.String, Valor = sPROTOCOLO}});

            FlexxTools_BOOMERAGNGUE_PURINA_PEDIDO_FLAG(ref oConfig, ref oMensagem, sTEL_CELULAR, sChaveEDI_Cliente, "116");
        }

        public static void FlexxTolls_BOOMERANGUE_PURINA_ENVIADO(ref Config oConfig,
                                                                 ref Mensagem oMensagem,
                                                                 string sTEL_CELULAR,
                                                                 string sPROTOCOLO,
                                                                 long idMessage,
                                                                 object sURLENCURTADA)
        {
            string sLink = "";
            DataTable oData;

            sPROTOCOLO = _Funcoes.FNC_Right("00000000" + sPROTOCOLO.Trim(), 6);

            sLink = "http://api02.plugthink.com/api/v2/boomerangue/_proc/sp_bot_bmWhats_acoes?sApp=boomerangue&sPartner=Flag&sToken=BM00015" +
                    "&sNomeBot=PURINA&sTelCliente=" + sTEL_CELULAR + "&sChaveBM=" + sPROTOCOLO + "&sAcao=EZP&sComplemento1=" + sURLENCURTADA.ToString() + "&sComplemento2=" + idMessage.ToString() + "&" +
                    "api_key=e14cb89ad01262944ba5e2780a1e2daa205d706a68c75d4cf73ce9746799a512";

            oData = FlexXTools_DataTable(0, "", sLink, "", sTEL_CELULAR, "", "", "");
        }

        public static string[] FlexXTools_BOOMERANGUE_PURINA(ref Config oConfig,
                                                             ref Mensagem oMensagem,
                                                             long iIdTarefa,
                                                             string sConexao,
                                                             string sTEL_CELULAR,
                                                             string sTemplate)
        {
            string[] sTexto = null;

            string sLink = null;
            string sProduto = "";
            DataTable oData01 = null;
            DataTable oData02 = null;

            string sPROTOCOLO = "";
            string sURL_ENCUTARDA = "";
            string sChaveEDI_Cliente = "";

            bool bSemPedido = false;
            bool bEnviarLink = false;

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "BOOMERANGUE_PURINA - Início");

            try
            {
                sTexto = new string[1];
                sConexao = sConexao.Replace("[TEL_CELULAR]", sTEL_CELULAR);

                oData01 = FlexXTools_DataTable(iIdTarefa, "", sConexao, "", sTEL_CELULAR, "", "", "");

                try
                {
                    foreach (DataRow oRow in oData01.Rows)
                    {
                        if (oRow["idListaOferta"] == null || oRow["idListaOferta"].ToString() == "")
                        {
                            bSemPedido = true;
                            break;
                        }
                        else
                        {
                            sTemplate = sTemplate.Replace("<<<Nome do Cliente>>>", oRow["Entidade"].ToString());
                            sTemplate = sTemplate.Replace("<<<CNPJ>>>", oRow["CNPJ"].ToString());
                            sTemplate = sTemplate.Replace("<<<mensagem_oferta>>>", oRow["ListaOferta"].ToString());
                            sTemplate = sTemplate.Replace("<<<desconto_pedido>>>", oRow["desconto_pedido"].ToString() + " %");
                            sTemplate = sTemplate.Replace("<<<Preco Original>>>", oRow["Preco_Original"].ToString());
                            sTemplate = sTemplate.Replace("<<<Preco Promocional>>>>", oRow["Preco_Promocional"].ToString());
                            sTemplate = sTemplate.Replace("<<<Condicao pgto>>>", oRow["condicoes_pagamento"].ToString());
                            sTemplate = sTemplate.Replace("<<<Nome Broker>>>", oRow["Nome_Broker"].ToString());
                            sTemplate = sTemplate.Replace("<<<CHAVE_Boomerangue>>>", oRow["ChaveBoomerangue"].ToString());
                            sTemplate = sTemplate.Replace("<<<Vendedor>>>", oRow["sVendedor"].ToString());
                            sPROTOCOLO = oRow["ChaveBoomerangue"].ToString();
                            sChaveEDI_Cliente = oRow["ChaveEDI_Cliente"].ToString();
                            bEnviarLink = (oRow["EnviarLink"].ToString() == "S");

                            sLink = "http://clickaqui.link/api/?key=HwAQ7nyujOpU&url=<<<url_bm>>>&custom=<<<ChaveEncurtador>>>";
                            sLink = sLink.Replace("<<<url_bm>>>", oRow["LinkBoomerangue"].ToString());
                            sLink = sLink.Replace("<<<ChaveEncurtador>>>", oRow["ChaveEncurtador"].ToString());
                            sURL_ENCUTARDA = "https://clickaqui.link/" + oRow["ChaveEncurtador"].ToString();

                            if (sProduto != "")
                            {
                                sProduto = sProduto + Environment.NewLine;
                            }

                            sProduto = sProduto + "*" + oRow["QuantidadeProduto"].ToString() + "* " + oRow["UnidadeMedida"].ToString().ToUpper().Trim() + "  _" + oRow["Descricao_Produto"].ToString() + "_";
                        }
                    }

                    if (bSemPedido)
                    {
                        sTexto[0] = "Não existe pedido para o número " + oMensagem.ParaLista.ToString();
                        oMensagem.ParaLista = oMensagem.contactuid;

                        oMensagem.InformarErro(Mensagem.const_Erro_SemBoomeangue, sTexto[0]);
                    }
                    else
                    {
                        string sLista = oMensagem.ParaLista;

                        oMensagem.ParaLista = oMensagem.contactuid;
                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, "Pesquisa Enviada " + oMensagem.Termo + ", " + sLista + ", BM: " + sPROTOCOLO, true);

                        oMensagem.ParaLista = sLista;

                        oData02 = FlexXTools_DataTable(iIdTarefa, "", sLink, "", "", "", "", "");

                        try
                        {
                            sURL_ENCUTARDA = oData02.Rows[0][0].ToString();
                        }
                        catch (Exception)
                        {
                        }

                        if (bEnviarLink) 
                        {
                            sTemplate = sTemplate.Replace("<<<LINK_BOOMERANGUE>>>", " ou se preferir acesse o link abaixo: " + sURL_ENCUTARDA);
                        }
                        else
                        {
                            sTemplate = sTemplate.Replace("<<<LINK_BOOMERANGUE>>>", " ");
                        }

                        sTemplate = sTemplate.Replace("<<<Produto>>>", sProduto);
                        sTemplate = sTemplate.Replace("&", "");
                        sTexto[0] = sTemplate;

                        oConfig.oBancoDados.DBProcedure("SP_Usuario_Boomerangue_INS", new clsCampo[] {new clsCampo {Nome = "TELEFONE", Tipo = DbType.String, Valor = sTEL_CELULAR },
                                                                                                      new clsCampo {Nome = "PROTOCOLO", Tipo = DbType.String, Valor = Convert.ToInt32(sPROTOCOLO).ToString()},
                                                                                                      new clsCampo {Nome = "URL_ENCUTARDA", Tipo = DbType.String, Valor = sURL_ENCUTARDA},
                                                                                                      new clsCampo {Nome = "ChaveEDI_Cliente", Tipo = DbType.String, Valor = sChaveEDI_Cliente}});

                        oMensagem.TipoAcao = Constantes.const_Acao_Boomerangue_ConfirmarEnvio;
                        oMensagem.CampoAuxiliar_01 = sURL_ENCUTARDA;
                        oMensagem.CampoAuxiliar_02 = Convert.ToInt32(sPROTOCOLO).ToString();

                        FlexxTools_BOOMERAGNGUE_PURINA_PEDIDO_FLAG(ref oConfig, ref oMensagem, sTEL_CELULAR, sChaveEDI_Cliente, "108");

                        oMensagem.InformarErro(Mensagem.const_Erro_BoomeangueEnviado, "Pesquisa Enviada " + oMensagem.Termo + ", " + sLista + ", BM: " + sPROTOCOLO);
                    }
                }
                catch (Exception)
                {
                    sTexto[0] = oData01.Rows[0]["Mensagem"].ToString();
                    oMensagem.ParaLista = oMensagem.contactuid;
                    oMensagem.InformarErro(Mensagem.const_Erro_SemBoomeangue, sTexto[0]);
                }
            }
            catch (Exception Ex)
            {
                oConfig.oBancoDados.DBSQL_Log_Gravar(0, "FlexXTools_BOOMERANGUE_PURINA", "B", Ex.Message);

                oMensagem.InformarErro(Mensagem.const_Erro_ErroBoomeangue, Ex.Message);
            }

            try
            {
                oData01.Dispose();
                oData02.Dispose();
            }
            catch (Exception)
            {
            }

            Declaracao.oMensagem_log.Adicionar(ref oMensagem, Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfDevolucao - Fim");

            return sTexto;
        }
    }
}
