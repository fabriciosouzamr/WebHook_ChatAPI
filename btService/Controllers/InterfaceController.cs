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
using System.Text;
using System.Web;
using System.Web.Http;
using static btService.Models.Repositorio;
using static btService.Modules.Funcoes;

namespace btService.Controllers
{
    public class InterfaceController : ApiController
    {
        class Retorno_Empresa_DadosAtivacao
        {
            public string Status { get; set; }
            public string ChaveAtivacao { get; set; }
            public string Token { get; set; }
        }

        class Retorno_Geral
        {
            public string Status { get; set; }
            public string Mensagem { get; set; }
        }

        [HttpGet]
        [Route("api/Interface/Processar")]
        [Trade2UPAuthentication]
        public HttpResponseMessage Interface_Processar(string Empresa = "", string Servico = "", string IntegracaoEmpresa = "")
        {
            try
            {
                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";
                int iEmpresa = 0;
                int iServico = 0;
                int iTipoIntegracao = 0;
                string sSql = "";
                string sMensagem = "";
                bool bExecutar = true;
                DataTable oData;
                Config oConfig = new Config();

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, "");

                oConfig.oBancoDados = new clsBancoDados();
                oConfig.oBancoDados.DBConectar(oConfig.tipobancodados, oConfig.dbconstring);

                if ((Empresa != "") && (bExecutar))
                {
                    sSql = "select * from tb_empresas where CodPuxada = '" + Empresa.Trim() + "'";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                        bExecutar = false;
                        sMensagem = "Código de puxada não identificado";
                    }
                    else
                    {
                        iEmpresa = Convert.ToInt32(oData.Rows[0]["idEmpresa"]);
                    }
                }

                if ((Servico != "") && (bExecutar))
                {
                    if (iEmpresa == 0)
                    {
                        sSql = "select sv.id_Servico" +
                               " from tb_servicos sv" +
                               " where cd_Servico = '" + Servico.Trim() + "'";
                    }
                    else
                    {
                        sSql = "select sv.id_Servico" +
                               " from tb_servicos sv" +
                                " inner join tb_empresas_servicos es on es.id_Servico = sv.id_Servico" +
                                                                  " and es.id_Empresa = " + iEmpresa.ToString() +
                               " where cd_Servico = '" + Servico.Trim() + "'";
                    }

                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                        bExecutar = false;
                        if (iEmpresa == 0)
                        {
                            sMensagem = "Serviço não identificado";
                        }
                        else
                        {
                            sMensagem = "Serviço não identificado ou não associado a empresa informada";
                        }
                    }
                    else
                    {
                        iServico = Convert.ToInt32(oData.Rows[0]["id_Servico"]);
                    }
                }

                if (IntegracaoEmpresa.Trim() != "")
                {
                    sSql = "select ti.id_tipo_integracao" +
                          " from tb_tipo_integracao ti" +
                           "  where upper(ti.cd_tipo_integracao) = '" + IntegracaoEmpresa.Trim().ToUpper() + "'";

                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                        bExecutar = false;
                        sMensagem = "Integração não encontrada";
                    }
                    else
                    {
                        iTipoIntegracao = Convert.ToInt32(oData.Rows[0]["id_tipo_integracao"]);
                    }
                }

                if (bExecutar)
                {
                    oConfig.oBancoDados.DBProcedure("sp_tarefas_processar", new clsCampo[] {new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = "5573991553135"},
                                                                                            new clsCampo {Nome = "id_servico", Tipo = DbType.Int16, Valor = iServico},
                                                                                            new clsCampo {Nome = "id_empresa", Tipo = DbType.Int16, Valor = iEmpresa },
                                                                                            new clsCampo {Nome = "idTipoIntegracao", Tipo = DbType.Int16, Valor = iTipoIntegracao }});

                    sMensagem = "Ok";
                }

                oConfig = null;

                return Request.CreateResponse(HttpStatusCode.BadRequest, sMensagem);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Interface/Empresa_DadosAtivacao")]
        [BasicAuthentication]
        public HttpResponseMessage Interface_Empresa_DadosAtivacao(string sCodPuxada, string sPartner)
        {
            Retorno_Empresa_DadosAtivacao oRetorno = new Retorno_Empresa_DadosAtivacao();

            oRetorno.Status = "";
            oRetorno.ChaveAtivacao = "";
            oRetorno.Token = "";

            try
            {
                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";
                string sSql = "";
                DataTable oData;
                Config oConfig = new Config();

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, "");

                oConfig.oBancoDados = new clsBancoDados();
                oConfig.oBancoDados.DBConectar(oConfig.tipobancodados, oConfig.dbconstring);

                sSql = "select * from vw_empresas where CodPuxada = '" + sCodPuxada.ToUpper().Trim() + "' and upper(rtrim(isnull(Sigla_Partner, ''))) = '" + sPartner.ToUpper().Trim() + "'";
                oData = oConfig.oBancoDados.DBQuery(sSql);

                if (oData.Rows.Count == 0)
                {
                    oRetorno.Status = "999"; //CodPuxada Nao Encontrado
                    oRetorno.ChaveAtivacao = "";
                    oRetorno.Token = "-";
                }
                else
                {
                    oRetorno.Status = "100"; //OK;
                    oRetorno.ChaveAtivacao = oData.Rows[0]["cod_ativacao_dash"].ToString();
                    oRetorno.Token = "-";
                }
            }
            catch (Exception)
            {
                oRetorno.Status = "999"; //CodPuxada Nao Encontrado
                oRetorno.ChaveAtivacao = "";
                oRetorno.Token = "-";
            }

            return Request.CreateResponse(HttpStatusCode.OK, oRetorno);
        }

        [HttpGet]
        [Route("api/Interface/Usuario_Ativacao")]
        [BasicAuthentication]
        public HttpResponseMessage Interface_Usuario_Ativacao(string Aplicativo, string CodPuxada, string TelefoneCadastro, string Servico, string Cargo, string Telefone, string Nome, string Codigo)
        {
            Retorno_Geral oRetorno = new Retorno_Geral();
            string sSql;
            string sMensagem;
            DataTable oData;

            try
            {
                string root = HttpContext.Current.Server.MapPath("~/") + "Config.json";
                Config oConfig = new Config();

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                oConfig.Carregar(Json, "");

                oConfig.oBancoDados = new clsBancoDados();
                oConfig.oBancoDados.DBConectar(oConfig.tipobancodados, oConfig.dbconstring);

                Telefone = FNC_FormatarTelefone(Telefone);
                TelefoneCadastro = FNC_FormatarTelefone(TelefoneCadastro);

                //sMensagem = "/ativar.flexxpower.BRK.10088885.EMPRESA.5573999009349.Fabricio Moreira.1000";
                sMensagem = "/ativar." + Aplicativo + "." + Servico + "." + CodPuxada + "." + Cargo + "." + Telefone + "." + Nome + "." + Codigo;

                oRetorno.Status = Webook_Util(1, Servico, "", sMensagem, "ZP", Nome, TelefoneCadastro, "Sofia");

                if (oRetorno.Status.Trim().ToUpper() == "OK")
                {
                    oRetorno.Mensagem = oRetorno.Status;
                    oRetorno.Status = "1";

                    sSql = "select * from vw_produto where rtrim(upper(no_produto)) = '" + Aplicativo.Trim().ToUpper() + "'";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    oConfig.oBancoDadosBot = new clsBancoDados();
                    oConfig.oBancoDadosBot.DBConectar(oData.Rows[0]["tp_bancodados"].ToString(), oData.Rows[0]["ds_stringconexao"].ToString());

                    sSql = "select count(*)" +
                           " from tb_Usuario" +
                           " where TELEFONE = '" + Telefone.Trim() + "'" +
                             " and Cod_Puxada = '" + CodPuxada + "'" +
                             " and LICENCA = '" + Servico.Trim().ToUpper() + "'";
                    if (Convert.ToInt32(oConfig.oBancoDadosBot.DBQuery_ValorUnico(sSql)) == 0)
                    {
                        oRetorno.Mensagem = "erro inesperado/não tratado";
                        oRetorno.Status = "999";
                    }
                    else
                    {
                        oRetorno.Mensagem = "Usuário Incluído ou aviso enviado.";
                        oRetorno.Status = "1";
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, oRetorno);
                }
                else
                {
                    oRetorno.Mensagem = oRetorno.Status;
                    oRetorno.Status = "999";

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, oRetorno);
                }
            }
            catch (Exception Ex)
            {
                oRetorno.Status = "999";
                oRetorno.Mensagem = Ex.Message;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, oRetorno);
            }
        }
    }
}
