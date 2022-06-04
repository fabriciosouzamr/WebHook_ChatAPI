using Microsoft.VisualBasic;
using System;
using System.Data;

namespace Integradores
{
    public static class Messengers
    {
        const string const_Propriedade_Notificacao_EMail_Ativacao = "0\\0\\0\\0\\0\\0\\#Notificacao\\#EMail_Ativacao";
        const string const_Propriedade_Notificacao_WebServiceAtivacao = "0\\0\\0\\0\\0\\0\\#Notificacao\\#WebServiceAtivacao";
        const string const_Propriedade_EMail_Host = "0\\0\\0\\0\\0\\0\\#EMail\\#Host";
        const string const_Propriedade_EMail_Port = "0\\0\\0\\0\\0\\0\\#EMail\\#Port";
        const string const_Propriedade_EMail_EnableSsl = "0\\0\\0\\0\\0\\0\\#EMail\\#EnableSsl";
        const string const_Propriedade_EMail_UseDefaultCredentials = "0\\0\\0\\0\\0\\0\\#EMail\\#UseDefaultCredentials";
        const string const_Propriedade_EMail_UserName = "0\\0\\0\\0\\0\\0\\#EMail\\#UserName";
        const string const_Propriedade_EMail_Password = "0\\0\\0\\0\\0\\0\\#EMail\\#Password";

        const int const_TipoPergunta_SimNao = 1;
        const int const_TipoPergunta_Texto = 2;
        const int const_TipoPergunta_Email = 3;
        const int const_TipoPergunta_Numero = 4;
        const int const_TipoPergunta_CPF = 5;
        const int const_TipoPergunta_CNPJ = 6;
        const int const_TipoPergunta_FaixaNumeros = 7;
        const int const_TipoPergunta_Data = 8;
        const int const_TipoPergunta_MultiplaEscolha = 9;
        const int const_TipoPergunta_PerguntaPersona = 10;

        public class Propriedade
        {
            public string Nome = "";
            public string Secao = "";
            public string Campo = "";
            public object Valor = null;
        }

        public static Propriedade[] Propriedades;

        public static void Propriedade_Carregar(ref Config oConfig)
        {
            DataTable oData;
            string sSql = "";

            sSql = "select Propriedade," +
                          "Secao," +
                          "Campo," +
                          "Valor" +
                   " from [dbo].[tb_propriedade]" +
                   " where idEmpresa = 0" +
                     " and idParceiro = 0" +
                     " and idProduto = 0" +
                     " and idServico = 0" +
                     " and idLicenca = 0" +
                     " and idUsuario = 0";
            oData = oConfig.oBancoDados.DBQuery(sSql);

            foreach (DataRow oRow in oData.Rows)
            {
                if (Propriedades == null)
                {
                    Array.Resize(ref Propriedades, 1);
                }
                else
                {
                    Array.Resize(ref Propriedades, Propriedades.Length + 1);
                }

                Propriedades[Propriedades.Length - 1] = new Propriedade();
                Propriedades[Propriedades.Length - 1].Nome = oRow["Propriedade"].ToString();
                Propriedades[Propriedades.Length - 1].Secao = oRow["Secao"].ToString();
                Propriedades[Propriedades.Length - 1].Campo = oRow["Campo"].ToString();
                Propriedades[Propriedades.Length - 1].Valor = oRow["Valor"];
            }
        }

        public static object Propriedade_Ler(string Nome)
        {
            object Ret = null;

            try
            {
                foreach (Propriedade Item in Propriedades)
                {
                    if (Item.Nome == Nome)
                    {
                        Ret = Item.Valor;
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }

            return Ret;
        }

        private static Boolean EnviarMensagem_Comando_Bot_UserAdmin(ref Config oConfig, ref Mensagem oMensagem, string sCodPuxada = "", int iEmpresa_Gestora = 0)
        {
            Boolean bOk = false;
            string sSql = "";
            string sEmpresa = "";
            DataTable oData = null;

            if (oConfig.UsuarioAdminValido)
            {
                bOk = true;
            }
            else
            {
                if (sCodPuxada.Trim() != "")
                {
                    sSql = "select idEmpresa_Gestora, idEmpresa from tb_empresas where CodPuxada = '" + sCodPuxada.Trim() + "'";

                    if (iEmpresa_Gestora != 0)
                    {
                        sSql = sSql = " and idEmpresa_Gestora " + iEmpresa_Gestora.ToString();
                    }

                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count != 0)
                    {
                        sEmpresa = "(" + oData.Rows[0]["idEmpresa_Gestora"].ToString() + "," + oData.Rows[0]["idEmpresa"].ToString() + ")";
                    }
                    else
                    {
                        sEmpresa = "(0)";
                    }
                }

                sSql = "select count(*) from tb_usuario" +
                       " where (Telefone = '" + oMensagem.contactuid.Trim() + "' or " +
                               "Telefone = '" + _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid.Trim()) + "')" +
                         " and Admin = 'S'";

                if (sEmpresa.Trim() != "")
                {
                    sSql = sSql + " and idEmpresa in (" + sEmpresa + ")";
                }

                bOk = (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) != 0);
            }

            return bOk;
        }

        private static void EnviarMensagem_Comando_BotCadastrar(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            string sMensagemEnviar = "";
            int id_plugzapGateway = 0;
            int id_plugzapUser = 0;

            // /bot.cadastrar.provider.bot.parceiro.servico.numerotelefone.token

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 8) && (sMensagem.Length != 10))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/bot.cadastrar.provider.bot.parceiro.servico.numerotelefone.token] ou [/bot.cadastrar.provider.bot.parceiro.servico.numerotelefone.token.plugzapGateway.plugzapUser]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sSql = "select count(*)" +
                           " from tb_bot bot" +
                            " inner join tb_bot_nroOrigem bto on bto.idBot = bot.idBot" +
                            " inner join tb_servicos srv on srv.id_Servico = bto.idServico" +
                           " where upper(trim(bot.Apelido)) = upper(trim('" + sMensagem[3] + "'))" +
                             " and bto.nroOrigem = '" + sMensagem[6] + "'" +
                             " and upper(trim(srv.cd_Servico)) = upper(trim('" + sMensagem[5] + "'))";

                    if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                    {
                        if (sMensagem.Length == 10)
                        {
                            id_plugzapGateway = Convert.ToInt32(sMensagem[8]);
                            id_plugzapUser = Convert.ToInt32(sMensagem[9]);
                        }

                        oConfig.oBancoDados.DBProcedure("sp_bot_nroOrigem_ins", new clsCampo[] {new clsCampo {Nome = "ds_empresa", Tipo = DbType.String, Valor = sMensagem[4]},
                                                                                               new clsCampo {Nome = "tp_ProviderWhatsApp", Tipo = DbType.String, Valor = sMensagem[2]},
                                                                                               new clsCampo {Nome = "ds_bot", Tipo = DbType.String, Valor = sMensagem[3]},
                                                                                               new clsCampo {Nome = "idBotExterno", Tipo = DbType.String, Valor = sMensagem[7]},
                                                                                               new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = sMensagem[6]},
                                                                                               new clsCampo {Nome = "licenca", Tipo = DbType.String, Valor = sMensagem[5].ToUpper()},
                                                                                               new clsCampo {Nome = "id_plugzapGateway", Tipo = DbType.Int32, Valor = id_plugzapGateway},
                                                                                               new clsCampo {Nome = "id_plugzapUser", Tipo = DbType.Int32, Valor = id_plugzapUser}});
                        sMensagemEnviar = "Número " + sMensagem[6] + " cadastrado";

                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagemEnviar, true);

                        oMensagem.ParaLista = sMensagem[2].ToString();
                        oMensagem.Termo = sMensagem[7];
                        oMensagem.status = "";
                        oMensagem.EnviarSemTratar = false;
                        oConfig.NaoValidarLicenca = true;
                    }
                    else
                    {
                        oMensagem.Termo = "";
                        oMensagem.messagebody = "Número " + sMensagem[6] + " já está cadastrado";
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_BotDescadastrar(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";

            // /bot.descadastrar.parceiro.bot.servico.numerotelefone

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 6))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/bot.descadastrar.parceiro.bot.servico.numerotelefone]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sSql = "select count(*)" +
                           " from tb_bot bot" +
                            " inner join tb_bot_nroOrigem bto on bto.idBot = bot.idBot" +
                            " inner join tb_servicos srv on srv.id_Servico = bto.idServico" +
                           " where upper(trim(bot.Apelido)) = upper(trim('" + sMensagem[3] + "'))" +
                             " and bto.nroOrigem = '" + sMensagem[5] + "'" +
                             " and upper(trim(srv.cd_Servico)) = upper(trim('" + sMensagem[4] + "'))";

                    if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                    {
                        oMensagem.Termo = "";
                        oMensagem.messagebody = "Número " + sMensagem[5] + " não está cadastrado";
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                    else
                    {
                        oConfig.oBancoDados.DBProcedure("sp_bot_nroOrigem_del", new clsCampo[] {new clsCampo {Nome = "ds_empresa", Tipo = DbType.String, Valor = sMensagem[2]},
                                                                                               new clsCampo {Nome = "ds_bot", Tipo = DbType.String, Valor = sMensagem[3]},
                                                                                               new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = sMensagem[5]},
                                                                                               new clsCampo {Nome = "licenca", Tipo = DbType.String, Valor = sMensagem[4].ToUpper()}});
                        oMensagem.ParaLista = sMensagem[2].ToString();
                        oMensagem.Termo = "";
                        oMensagem.messagebody = "Número " + sMensagem[5] + " descadastrado";
                        oMensagem.status = "";
                        oMensagem.EnviarSemTratar = false;
                        oConfig.NaoValidarLicenca = true;
                        /// bot.ativar.55319875622237.marino.mrsoft.alice.hnk.palavra_chave_boas_vindas
                    }
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_BotListarServicos(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            string sMensagemEnviar = "";
            DataTable oData;

            // /bot.ListarServicos.bot

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 3))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/bot.ListarServicos.bot]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sSql = "select srv.no_Servico, srv.cd_Servico, bto.nroOrigem" +
                           " from tb_bot bot" +
                            " inner join tb_bot_nroOrigem bto on bto.idBot = bot.idBot" +
                            " inner join tb_servicos srv on srv.id_Servico = bto.idServico" +
                           " where upper(trim(bot.Apelido)) = upper(trim('" + sMensagem[2] + "'))" +
                           " order by srv.no_Servico, srv.cd_Servico, bto.nroOrigem";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                        oMensagem.Termo = "";
                        oMensagem.messagebody = "Nenhum serviço vinculado a esse bot";
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                    else
                    {
                        foreach (DataRow oRow in oData.Rows)
                        {
                            if (sMensagemEnviar.Trim() != "")
                            { sMensagemEnviar = sMensagemEnviar + Environment.NewLine; }

                            sMensagemEnviar = sMensagemEnviar + oRow["no_Servico"].ToString() + " (" + oRow["cd_Servico"].ToString() + ") - " + oRow["nroOrigem"].ToString();
                        }

                        oMensagem.Termo = "";
                        oMensagem.messagebody = sMensagemEnviar;
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                        /// bot.ativar.55319875622237.marino.mrsoft.alice.hnk.palavra_chave_boas_vindas
                    }
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_BotConsultarCliente(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            string sMensagemEnviar = "";
            DataTable oData;

            // /bot.ConsultarCliente.Parceiro.NumeroTelefone

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 3))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/bot.ConsultarCliente.NumeroTelefone]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sSql = "select distinct telefone, Nome, no_Servico, cd_Servico, Empresa, Cod_Puxada" +
                           " from vw_usuario_servico_empresa" +
                           " where upper(rtrim(TELEFONE)) = upper(rtrim('" + _Funcoes.FNC_FormatarTelefone(sMensagem[2]) + "'))" +
                             " and tp_ativo = 'S' and ServicoUsuarioAtivo = 'S'" +
                           " order by Nome, Empresa, no_Servico, cd_Servico";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                        oMensagem.Termo = "";
                        oMensagem.messagebody = "Nenhum serviço vinculado a esse número";
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                    else
                    {
                        foreach (DataRow oRow in oData.Rows)
                        {
                            if (sMensagemEnviar.Trim() != "")
                            { sMensagemEnviar = sMensagemEnviar + Environment.NewLine; }

                            sMensagemEnviar = sMensagemEnviar + oRow["Nome"].ToString().ToUpper() + " - " + oRow["no_Servico"].ToString() + " (" + oRow["cd_Servico"].ToString() + "), na empresa " + oRow["Empresa"].ToString() + " (" + oRow["Cod_Puxada"].ToString() + ")";
                        }

                        oMensagem.Termo = "";
                        oMensagem.messagebody = sMensagemEnviar;
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                        /// bot.ativar.55319875622237.marino.mrsoft.alice.hnk.palavra_chave_boas_vindas
                    }
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_BotConsultarBot(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            string sMensagemEnviar = "";
            DataTable oData;

            // /bot.ConsultarCliente.Parceiro.NumeroTelefone

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 4))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/bot.ConsultarBot.Parceiro.NumeroTelefone]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sSql = "select bot.Apelido, srv.no_Servico, srv.cd_Servico" +
                           " from tb_bot bot" +
                            " inner join tb_bot_nroOrigem bto on bto.idBot = bot.idBot" +
                            " inner join tb_servicos srv on srv.id_Servico = bto.idServico" +
                           " where upper(rtrim(bto.nroOrigem)) = upper(rtrim('" + sMensagem[3] + "'))" +
                           " order by bot.Apelido, srv.no_Servico, srv.cd_Servico";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                        oMensagem.Termo = "";
                        oMensagem.messagebody = "Nenhum serviço vinculado a esse número";
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                    else
                    {
                        foreach (DataRow oRow in oData.Rows)
                        {
                            if (sMensagemEnviar.Trim() != "")
                            { sMensagemEnviar = sMensagemEnviar + Environment.NewLine; }

                            sMensagemEnviar = sMensagemEnviar + oRow["Apelido"].ToString().ToUpper() + " - " + oRow["no_Servico"].ToString() + " (" + oRow["cd_Servico"].ToString() + ")";
                        }

                        oMensagem.Termo = "";
                        oMensagem.messagebody = sMensagemEnviar;
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                        /// bot.ativar.55319875622237.marino.mrsoft.alice.hnk.palavra_chave_boas_vindas
                    }
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }
        private static void EnviarMensagem_Comando_BotHelp(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sMensagemEnviar = "";

            // /bot.Help

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 2))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/bot.help]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sMensagemEnviar = "*Para adicionar um número de celular no cadastro de usuário*" + Environment.NewLine +
                                      "/bot.ativar.telefone.nome.parceiro.codpuxada.bot.servico.termo_boas_vindas" + Environment.NewLine +
                                      " " + Environment.NewLine +
                                      "*Para cadastrar um número de celular no cadastro de bot*" + Environment.NewLine +
                                      "/bot.cadastrar.provider.bot.parceiro.servico.numerotelefone.token" + Environment.NewLine +
                                      " " + Environment.NewLine +
                                      "*Para descadastrar um número de celular no cadastro de bot*" + Environment.NewLine +
                                      "/bot.descadastrar.parceiro.bot.servico.numerotelefone" + Environment.NewLine +
                                      " " + Environment.NewLine +
                                      "*Para listar os serviços e números relacionados a um bot*" + Environment.NewLine +
                                      "/bot.ListarServicos.bot" + Environment.NewLine +
                                      " " + Environment.NewLine +
                                      "*Para listar os serviços e bots relacionados a um número*" + Environment.NewLine +
                                      "/bot.ConsultarCliente.Parceiro.NumeroTelefone";

                    oMensagem.Termo = "";
                    oMensagem.messagebody = sMensagemEnviar;
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_Geral_Query(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            DataTable oData;

            sSql = "select Command from vwmessagetermswords" +
                   " where rtrim(upper(Command)) = '" + sMensagem[0].Substring(1) + "'" +
                     " and (rtrim(upper(cd_Servico)) = '" + oMensagem.Servico + "' or cd_Servico is null)" +
                   " order by cd_Servico desc";
            oData = oConfig.oBancoDados.DBQuery(sSql);

            if (oData.Rows.Count != 0)
            {
                oMensagem.Termo = sMensagem[0].Substring(1);
                oMensagem.SemServico = true;
            }

            oData.Dispose();
            oData = null;
        }

        private static void EnviarMensagem_Comando_Geral_Ativar(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            string sMensagemEnviar = "";
            string sCodPuxada = "";
            clsBancoDados oBancoDados;
            DataTable oData;
            Boolean bAtivado = false;
            string sSenha = "";
            string sNome = "";
            string sTermo = "";
            string sFlexXTools_AtivarTelefone = "";

            int iIdProduto = 0;
            int iIdServico = 0;
            int iIdUsuario = 0;
            bool bSemServico = false;
            bool bEnviar_ChaveAtivacao = false;

            // /ativar.app.servico.codpuxada.cargo.telefone.nome

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                oMensagem.Termo = "";
                oMensagem.status = Constantes.const_Status_Enviado;
                oMensagem.EnviarSemTratar = true;
                oConfig.NaoValidarLicenca = true;

                if ((sMensagem.Length < 5))
                {
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/ativar.app.servico.codpuxada.EMPRESA.telefonecontato.nome] ou [/ativar.app.servico.codpuxada.cargo.telefone.nome.codigo]";
                }
                else
                {
                    sMensagem[5] = _Funcoes.FNC_FormatarTelefone(sMensagem[5]);

                    sSql = "select *" +
                           " from vw_produto" +
                           " where rtrim(upper(no_produto)) = '" + sMensagem[1].ToUpper().Trim() + "'";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                        oMensagem.messagebody = "O aplicativo " + sMensagem[1] + " não existe.";
                    }
                    else
                    {
                        sTermo = oData.Rows[0]["Command_BoasVindas"].ToString();
                        iIdProduto = Convert.ToInt32(oData.Rows[0]["id_Produto"]);
                        bSemServico = (_Funcoes.FNC_NuloString(oData.Rows[0]["idServico"]) == "");
                        bEnviar_ChaveAtivacao = (_Funcoes.FNC_NuloString(oData.Rows[0]["tp_Enviar_ChaveAtivacao"]) == "S");

                        oBancoDados = new clsBancoDados();
                        oBancoDados.DBConectar(oData.Rows[0]["tp_bancodados"].ToString(), oData.Rows[0]["ds_stringconexao"].ToString());

                        sCodPuxada = sMensagem[3].ToUpper().Trim();

                        if (sCodPuxada.Length != 8)
                        {
                            sCodPuxada = ("00000000" + sCodPuxada).Substring(("00000000" + sCodPuxada).Length - 8);
                        }

                        switch (sMensagem[4].Trim().ToUpper())
                        {
                            case "E":
                                {
                                    sMensagem[4] = "EMPRESA";
                                    break;
                                }
                            case "S":
                                {
                                    sMensagem[4] = "SUPERVISOR";
                                    break;
                                }
                            case "V":
                                {
                                    sMensagem[4] = "VENDEDOR";
                                    break;
                                }
                        }
                        switch (sMensagem[4].Trim().ToUpper())
                        {
                            case "1E":
                            case "1EMPRESA":
                                {
                                    if ((sMensagem.Length != 7))
                                    {
                                        oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/ativar.app.servico.codpuxada.EMPRESA.telefonecontato.nome] ou [/ativar.app.servico.codpuxada.cargo.telefone.nome.codigo]";
                                    }
                                    else
                                    {
                                        sSql = "select count(*)" +
                                               " from tb_empresas" +
                                               " where CodPuxada = '" + sCodPuxada + "'";
                                        if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                                        {
                                            oConfig.oBancoDados.DBProcedure("SP_EMPRESA_ATIVAR", new clsCampo[] {new clsCampo {Nome = "idEmpresa_Gestora", Tipo = DbType.Int32, Valor = oData.Rows[0]["id_empresa"].ToString()},
                                                                                                                 new clsCampo {Nome = "Cod_Puxada", Tipo = DbType.String, Valor = sCodPuxada},
                                                                                                                 new clsCampo {Nome = "nome", Tipo = DbType.String, Valor = sMensagem[6]},
                                                                                                                 new clsCampo {Nome = "licenca", Tipo = DbType.String, Valor = sMensagem[2].ToUpper()},
                                                                                                                 new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = sMensagem[5]}});
                                            sSql = "select count(*)" +
                                                   " from tb_empresas" +
                                                   " where CodPuxada = '" + sCodPuxada + "'";
                                            bAtivado = (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) != 0);

                                            if (bAtivado)
                                            {
                                                oMensagem.messagebody = "Empresa " + sMensagem[6] + "(" + sCodPuxada + ") ativada";
                                            }
                                            else
                                            {
                                                oMensagem.messagebody = "Não foi possível ativar a empresa " + sMensagem[6] + "(" + sCodPuxada + "), favor verificar com o suporte.";
                                            }
                                        }
                                        else
                                        {
                                            oMensagem.messagebody = "Empresa " + sMensagem[6] + "(" + sCodPuxada + ") já está ativada";

                                            bAtivado = true;
                                        }
                                    }

                                    break;
                                }
                            case "E":
                            case "EMPRESA":
                            case "V":
                            case "S":
                            case "VENDEDOR":
                            case "SUPERVISOR":
                                {
                                    if ((sMensagem.Length < 8))
                                    {
                                        oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/ativar.app.servico.codpuxada.cargo.telefone.nome.codigo]";
                                    }
                                    else
                                    {
                                        int Ativo = 1;

                                        if (sMensagem.Length == 9)
                                        {
                                            if (sMensagem[8].Trim().ToUpper() == "N")
                                            { Ativo = 0; }
                                        }

                                        sSql = "select count(*)" +
                                               " from tb_empresas" +
                                               " where CodPuxada = '" + sCodPuxada + "'" +
                                                 " and isnull(cod_ativacao_dash, '') <> ''";
                                        if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                                        {
                                            Bot.Tabelas_AtualizarValor(ref oMensagem, "tb_empresas", "cod_ativacao_dash", "");
                                            oMensagem.messagebody = "Empresa " + sCodPuxada + " não cadastrada";
                                        }
                                        else
                                        {
                                            Bot.CarregarTabelaEmpresa(ref oConfig, ref oMensagem, sCodPuxada);

                                            sSql = "select count(*)" +
                                                   " from tb_Usuario" +
                                                   " where TELEFONE = '" + sMensagem[5].Trim() + "'" +
                                                     " and Cod_Puxada = '" + sCodPuxada;
                                            if (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                                            {
                                                if (sMensagem[4].Substring(0, 1).ToUpper() == "E")
                                                {
                                                    sNome = sMensagem[6];
                                                }
                                                else
                                                {
                                                    sNome = sMensagem[6] + " (" + Convert.ToInt32(sMensagem[7].Trim()).ToString() + ")";
                                                }

                                                if (sMensagem[2].Trim().ToUpper() == "NTB")
                                                {
                                                    if (sNome.ToUpper() == "TADEU")
                                                    {
                                                        sSenha = "911221";
                                                    }
                                                    else if (sNome.ToUpper() == "GEANE")
                                                    {
                                                        sSenha = "715339";
                                                    }
                                                }
                                                else
                                                {
                                                    sSenha = _Funcoes.FNC_Senha_Gerar(6);

                                                    if (sSenha.Trim() == "")
                                                    {
                                                        sSenha = _Funcoes.FNC_Senha_Gerar(6);
                                                    }
                                                }

                                                if (sSenha.Trim() == "")
                                                {
                                                    oMensagem.messagebody = "Problema não geração automática de senha. Favor tentar novamente.";
                                                }
                                                else
                                                {
                                                    sSql = "SELECT * FROM dbo.tb_servicos where upper(cd_Servico) = '" + sMensagem[2].ToUpper() + "'";
                                                    oData = oConfig.oBancoDados.DBQuery(sSql);

                                                    if (oData.Rows.Count != 0)
                                                    {
                                                        iIdServico = Convert.ToInt32(oData.Rows[0]["id_Servico"]);
                                                    }
                                                    else
                                                    {
                                                        iIdServico = 0;
                                                    }

                                                    sSql = "select *" +
                                                           " from tb_Usuario" +
                                                           " where TELEFONE = '" + sMensagem[5].Trim() + "'" +
                                                             " and Cod_Puxada = '" + sCodPuxada + "'";
                                                    oData = oConfig.oBancoDados.DBQuery(sSql);

                                                    if (oData.Rows.Count > 0)
                                                    {
                                                        iIdUsuario = Convert.ToInt32(oData.Rows[0]["idUsuario"]);
                                                        sSenha = oData.Rows[0]["Senha"].ToString();
                                                    }

                                                    oConfig.oBancoDados.DBProcedure("SP_USUARIO_ATIVAR", new clsCampo[] {new clsCampo {Nome = "Cod_Puxada", Tipo = DbType.String, Valor = sCodPuxada},
                                                                                                                         new clsCampo {Nome = "CodEmpregado", Tipo = DbType.String, Valor = sMensagem[7]},
                                                                                                                         new clsCampo {Nome = "nome", Tipo = DbType.String, Valor = sNome},
                                                                                                                         new clsCampo {Nome = "departamento", Tipo = DbType.String, Valor = sMensagem[4]},
                                                                                                                         new clsCampo {Nome = "senha", Tipo = DbType.String, Valor = sSenha },
                                                                                                                         new clsCampo {Nome = "licenca", Tipo = DbType.String, Valor = sMensagem[2].ToUpper()},
                                                                                                                         new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = sMensagem[5]},
                                                                                                                         new clsCampo {Nome = "Ativo", Tipo = DbType.Int32, Valor = Ativo },
                                                                                                                         new clsCampo {Nome = "DTINTEGRACAO", Tipo = DbType.DateTime, Valor = DateTime.Now},
                                                                                                                         new clsCampo {Nome = "VERSAO_INTEGRADOR", Tipo = DbType.String, Valor = "/ativar"}});

                                                    if (iIdUsuario == 0)
                                                    {
                                                        if (!oMensagem.MessageWebHook_Util)
                                                        {
                                                            sFlexXTools_AtivarTelefone = "Status: " + Integradores.FlexXTools.FlexXTools_AtivarTelefone(sCodPuxada, sMensagem[4], sMensagem[5], sNome, sSenha, sMensagem[7]);
                                                        }

                                                        sSql = "select idUsuario" +
                                                               " from tb_Usuario" +
                                                               " where TELEFONE = '" + sMensagem[5].Trim() + "'" +
                                                                 " and Cod_Puxada = '" + sCodPuxada + "'" +
                                                                 " and LICENCA = '" + sMensagem[2].Trim().ToUpper() + "'";
                                                        iIdUsuario = Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql, 0));
                                                    }

                                                    sSql = "select count(*)" +
                                                           " from tb_Usuario" +
                                                           " where TELEFONE = '" + sMensagem[5].Trim() + "'" +
                                                             " and Cod_Puxada = '" + sCodPuxada;
                                                    oBancoDados.DBProcedure("SP_USUARIO_ATIVAR", new clsCampo[] { new clsCampo {Nome = "Cod_Puxada", Tipo = DbType.String, Valor = sCodPuxada},
                                                                                                                  new clsCampo {Nome = "CodEmpregado", Tipo = DbType.String, Valor = sMensagem[7]},
                                                                                                                  new clsCampo {Nome = "nome", Tipo = DbType.String, Valor = sNome},
                                                                                                                  new clsCampo {Nome = "departamento", Tipo = DbType.String, Valor = sMensagem[4]},
                                                                                                                  new clsCampo {Nome = "senha", Tipo = DbType.String, Valor = sSenha},
                                                                                                                  new clsCampo {Nome = "licenca", Tipo = DbType.String, Valor = sMensagem[2].ToUpper()},
                                                                                                                  new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = sMensagem[5]},
                                                                                                                  new clsCampo {Nome = "Ativo", Tipo = DbType.Int32, Valor = Ativo },
                                                                                                                  new clsCampo {Nome = "DTINTEGRACAO", Tipo = DbType.DateTime, Valor = DateTime.Now},
                                                                                                                  new clsCampo {Nome = "VERSAO_INTEGRADOR", Tipo = DbType.String, Valor = "/ativar"}});

                                                    sSql = "select count(*) from tb_Usuario_Servico" +
                                                           " where idUsuario = " + iIdUsuario.ToString() +
                                                             " and idEmpresa = " + Bot.Tabelas_BuscarValor(ref oMensagem, "tb_empresas", "idEmpresa").ToString() +
                                                             " and idServico = " + iIdServico.ToString() +
                                                             " and idProduto = " + iIdProduto.ToString();
                                                    if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                                                    {
                                                        sSql = "insert into tb_Usuario_Servico (idUsuario,idEmpresa,idProduto,idServico,Login,SenhaAPP,ServicoUsuarioAtivo,Departamento,TipoUsuario)" +
                                                               " values (#idUsuario,#idEmpresa,#idProduto,#idServico,#Login,#SenhaAPP,#ServicoUsuarioAtivo,#Departamento,#TipoUsuario)";

                                                        oConfig.oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "idUsuario", Valor = iIdUsuario, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "idEmpresa", Valor = Bot.Tabelas_BuscarValor(ref oMensagem, "tb_empresas", "idEmpresa"), Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "idProduto", Valor = iIdProduto, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "idServico", Valor = iIdServico, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "Login", Valor = sNome, Tipo = DbType.String },
                                                                                                              new clsCampo { Nome = "SenhaAPP", Valor = sSenha, Tipo = DbType.String },
                                                                                                              new clsCampo { Nome = "ServicoUsuarioAtivo", Valor = "S", Tipo = DbType.String },
                                                                                                              new clsCampo { Nome = "Departamento", Valor = sMensagem[4], Tipo = DbType.String },
                                                                                                              new clsCampo { Nome = "TipoUsuario", Valor = "US", Tipo = DbType.String }});
                                                    }
                                                    else
                                                    {
                                                        sSql = "update from tb_Usuario_Servico set idServico = " + iIdServico.ToString() +
                                                               " where idUsuario = " + iIdUsuario.ToString() +
                                                                 " and idEmpresa = " + Bot.Tabelas_BuscarValor(ref oMensagem, "tb_empresas", "idEmpresa").ToString() +
                                                                 " and idProduto = " + iIdProduto.ToString();
                                                        oConfig.oBancoDados.DBExecutar(sSql);
                                                    }

                                                    sSql = "select count(*)" +
                                                           " from tb_Usuario" +
                                                           " where TELEFONE = '" + sMensagem[5].Trim() + "'" +
                                                             " and Cod_Puxada = '" + sCodPuxada + "'" +
                                                             " and LICENCA = '" + sMensagem[2].Trim().ToUpper() + "'";
                                                    bAtivado = (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql)) != 0);

                                                    if (bAtivado)
                                                    {
                                                        sMensagemEnviar = "Número " + sMensagem[5] + " ativado no aplicativo " + sMensagem[1] + " com a senha: " + sSenha;

                                                        if (sFlexXTools_AtivarTelefone.Trim() != "")
                                                        {
                                                            sMensagemEnviar = sMensagemEnviar +
                                                                              Environment.NewLine +
                                                                              Environment.NewLine +
                                                                              "Status da ativação na Flag: " + sFlexXTools_AtivarTelefone;
                                                        }
                                                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagemEnviar, true);

                                                    }
                                                    else
                                                    {
                                                        oMensagem.messagebody = "Não foi possível ativar o número " + sMensagem[5] + ", favor verificar com o suporte.";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                sMensagemEnviar = "Número " + sMensagem[5] + " já está ativado no aplicativo " + sMensagem[1];

                                                Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagemEnviar, true);

                                                bAtivado = true;
                                            }

                                            if (bAtivado)
                                            {
                                                oMensagem.ParaLista = sMensagem[5].ToString();
                                                oMensagem.Termo = sTermo;
                                                oMensagem.status = "";
                                                oMensagem.EnviarSemTratar = false;
                                                oMensagem.SemServico = bSemServico;
                                                oConfig.NaoValidarLicenca = true;
                                                if (bEnviar_ChaveAtivacao) { oMensagem.MensagemFinal = Bot.Tabelas_BuscarValor(ref oMensagem, "tb_empresas", "cod_ativacao_dash", "").ToString(); }
                                            }
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    oMensagem.messagebody = "O cargo " + sMensagem[4] + " não existe. Informar 'E' ou 'EMPRESA' ou 'S' ou 'SUPERVISOR' ou 'V' ou 'VENDEDOR'.";

                                    break;
                                }
                        }

                        Bot.CarregarTabelaUsuario(ref oMensagem, oBancoDados, sMensagem[5], sCodPuxada, sMensagem[2].Trim().ToUpper());
                    }

                    oData.Dispose();
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_Pesquisa(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });

            //pesquisa.servico.pesquisa.nrotelefone

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length < 4))
                {
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/pesquisa.servico.pesquisa.nrotelefone]";
                }
                else
                {
                    string sSql;

                    sSql = "SELECT count(*) FROM vwmessagetermswords WHERE cd_Servico = '" + sMensagem[1].Trim().ToUpper() + "' AND COMMAND = '" + sMensagem[2].Trim().ToUpper() + "'";

                    if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                    {
                        oMensagem.messagebody = "Pesquisa inexistente para esse serviço";
                    }
                    else
                    {
                        if (sMensagem[1].Trim().ToUpper() != "BPU")
                        {
                            Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, "Pesquisa Enviada", true);
                        }

                        oConfig.oBancoDados.DBReconectar();

                        sSql = "select bot.*" +
                               " from tb_servicos srv" +
                                " inner join vw_bot bot on bot.idBot_nroOrigem = srv.idBot_nroOrigem_Resposta" +
                                " where srv.cd_Servico = '" + sMensagem[1].Trim().ToUpper() + "'";
                        oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_Bot] = oConfig.oBancoDados.DBQuery(sSql);

                        Bot_Atualiza(ref oMensagem, ref oConfig);

                        oMensagem.ParaLista = sMensagem[3].ToString();
                        oMensagem.Termo = sMensagem[2].ToString();
                        oMensagem.Servico = sMensagem[1].ToString();
                        oMensagem.status = "";
                        oMensagem.EnviarSemTratar = true;
                        oMensagem.SemServico = false;
                        oConfig.NaoValidarLicenca = true;
                    }

                    oMensagem.EnviarSemTratar = true;
                    oMensagem.SemServico = false;
                    oConfig.NaoValidarLicenca = true;
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_Cobranca(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });

            //sofiacob.hnk.nrotelefone

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length < 4))
                {
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/cobranca.hnk.nrotelefone.nome]";
                }
                else
                {
                    oMensagem.ParaLista = sMensagem[2].ToString();
                    oMensagem.Termo = "";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.messagebody = "Ola! " + sMensagem[3].ToString() + Environment.NewLine +
                                            " Sou Sofia*  assistente virtual Flag" + Environment.NewLine + Environment.NewLine +
                                            "A partir de hoje posso enviar Promoções  , notificações e outras informações." + Environment.NewLine + Environment.NewLine +
                                            "Caso deseje receber, responda SIM" + Environment.NewLine + Environment.NewLine +
                                            "Obrigado :)";
                    oMensagem.EnviarSemTratar = true;
                    oMensagem.SemServico = false;
                    oMensagem.command = "HNK_COBRANCA_TESTE";
                    oConfig.NaoValidarLicenca = true;
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_DashHelp(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sMensagemEnviar = "";

            // /bot.Help

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 2))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/dash.help]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sMensagemEnviar = "*Para adicionar um número de celular no cadastro de usuário*" + Environment.NewLine +
                                      "/dash.ativar.telefone.nome.parceiro.codpuxada.bot.servico.termo_boas_vindas" + Environment.NewLine;

                    oMensagem.Termo = "";
                    oMensagem.messagebody = sMensagemEnviar;
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_DashAtivar(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            string sMensagemEnviar = "";
            string sCodPuxada = "";
            clsBancoDados oBancoDados;
            DataTable oData;
            Boolean bAtivado = false;

            // bot.ativar.telefone.nome.parceiro.bot.servico.termo_boas_vindas

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 8))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/dash.ativar.telefone.nome.senha.codpuxada.servico.termo_boas_vindas]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sMensagem[2] = _Funcoes.FNC_FormatarTelefone(sMensagem[2]);

                    sSql = "select bdd.*" +
                           " from tb_servicos srv" +
                            " inner join tb_bancodados bdd on bdd.id_bancodados = srv.id_bancodados_dash" +
                           " where srv.cd_Servico = '" + sMensagem[6] + "'";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    oBancoDados = new clsBancoDados();
                    oBancoDados.DBConectar(oData.Rows[0]["tp_bancodados"].ToString(), oData.Rows[0]["ds_stringconexao"].ToString());

                    sCodPuxada = sMensagem[5].ToUpper().Trim();

                    if (sCodPuxada.Length != 8)
                    {
                        sCodPuxada = ("00000000" + sCodPuxada).Substring(("00000000" + sCodPuxada).Length - 8);
                    }

                    Bot.CarregarTabelaEmpresa(ref oConfig, ref oMensagem, sCodPuxada);

                    //oMensagem.MensagemFinal = _Funcoes.FNC_NuloString(Bot.Tabelas_BuscarValor("TB_EMPRESAS", "cod_ativacao_dash"));

                    sSql = "select count(*)" +
                           " from tb_Usuario" +
                           " where TELEFONE = '" + sMensagem[2].Trim() + "'" +
                             " and Cod_Puxada = '" + sCodPuxada + "'" +
                             " and LICENCA = '" + sMensagem[6].Trim().ToUpper() + "'";

                    if (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                    {
                        oBancoDados.DBProcedure("SP_USUARIO_CAD", new clsCampo[] {new clsCampo {Nome = "Cod_Puxada", Tipo = DbType.String, Valor = sCodPuxada},
                                                                                  new clsCampo {Nome = "nome", Tipo = DbType.String, Valor = sMensagem[3]},
                                                                                  new clsCampo {Nome = "senha", Tipo = DbType.String, Valor = sMensagem[4]},
                                                                                  new clsCampo {Nome = "licenca", Tipo = DbType.String, Valor = sMensagem[6].ToUpper()},
                                                                                  new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = sMensagem[2]},
                                                                                  new clsCampo {Nome = "DTINTEGRACAO", Tipo = DbType.DateTime, Valor = DateTime.Now},
                                                                                  new clsCampo {Nome = "VERSAO_INTEGRADOR", Tipo = DbType.String, Valor = "dash"}});
                        sSql = "select count(*)" +
                               " from tb_Usuario" +
                               " where TELEFONE = '" + sMensagem[2].Trim() + "'" +
                                 " and Cod_Puxada = '" + sCodPuxada + "'" +
                                 " and LICENCA = '" + sMensagem[6].Trim().ToUpper() + "'";
                        bAtivado = (Convert.ToInt32(oBancoDados.DBQuery(sSql)) != 0);

                        if (bAtivado)
                        {
                            sMensagemEnviar = "Número " + sMensagem[2] + " ativado";
                            Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagemEnviar, true);
                        }

                    }
                    else
                    {
                        sMensagemEnviar = "Número " + sMensagem[2] + " já está ativado";

                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagemEnviar, true);

                        bAtivado = true;
                    }

                    Bot.CarregarTabelaUsuario(ref oMensagem, oBancoDados, sMensagem[2], sCodPuxada, sMensagem[6].Trim().ToUpper());

                    if (bAtivado)
                    {
                        oMensagem.ParaLista = sMensagem[2].ToString();
                        oMensagem.Termo = sMensagem[7];
                        oMensagem.status = "";
                        oMensagem.EnviarSemTratar = false;
                        oMensagem.SemServico = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                    else
                    {
                        oMensagem.messagebody = "Não foi possível ativar o número " + sMensagem[2] + ", favor verificar com o suporte.";
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oMensagem.SemServico = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_BotOlaPdvMenu(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sMensagemEnviar = "";

            // /bot.Help

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 2))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/olapdv.ajuda]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sMensagemEnviar = "*Menu de Integração*" + Environment.NewLine +
                                      "/olapdv.integra" + Environment.NewLine +
                                      " " + Environment.NewLine +
                                      "*Listar opções de ajuda*" + Environment.NewLine +
                                      "/olapdv.ajuda";

                    oMensagem.Termo = "";
                    oMensagem.messagebody = sMensagemEnviar;
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_BotOlaPdvIntegra(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            DataTable oData;
            string Empresa = "";
            string Servico = "";
            bool bExecutar = true;
            string sMensagemRetorno = "";
            string sMensagemRetorno_Aux = "";
            string sEmpresaIntegracao = "";
            int iEmpresa = 0;
            int iServico = 0;
            int iTipoIntegracao = 0;

            // /bot.ConsultarCliente.Parceiro.NumeroTelefone

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length == 2))
                {
                    sSql = "select cd_tipo_integracao, ds_tipo_integracao" +
                           " from tb_tipo_integracao" +
                           " where id_Servico = 9" +
                             " and cd_tipo_integracao is not null" +
                           " order by ds_tipo_integracao";
                    oData = oConfig.oBancoDados.DBQuery(sSql);

                    foreach (DataRow oRow in oData.Rows)
                    {
                        if (sMensagemRetorno.Trim() != "")
                        {
                            sMensagemRetorno = sMensagemRetorno + Environment.NewLine;
                            sMensagemRetorno_Aux = sMensagemRetorno_Aux + Environment.NewLine;
                        }

                        sMensagemRetorno = sMensagemRetorno +
                                           "*Para oMensagem " + oRow["ds_tipo_integracao"].ToString() + "*" +
                                           Environment.NewLine +
                                           "/olapdv.integra." + oRow["cd_tipo_integracao"].ToString();
                        sMensagemRetorno_Aux = sMensagemRetorno_Aux + "*" + oRow["cd_tipo_integracao"].ToString() + "* - " +
                                                                      oRow["ds_tipo_integracao"].ToString();
                    }

                    sMensagemRetorno = "*Para acionar uma interface especifica do OlaPdv*" + Environment.NewLine +
                                       sMensagemRetorno +
                                       Environment.NewLine + Environment.NewLine +
                                       "OBS.: Caso você tenha acesso a mais de uma empresa, pode colocar '.<codpuxada>'" +
                                       Environment.NewLine + Environment.NewLine +
                                       "-------------------------------------------------------------------------" + Environment.NewLine +
                                       "*Para acionar uma interface do OlaPdv, com opções gerais*" + Environment.NewLine +
                                       "/olapdv.integra.codpuxada =<codpuxada>.servico =<servico>.integracao =<integracao>" +
                                       Environment.NewLine + Environment.NewLine +
                                       sMensagemRetorno_Aux;


                    oMensagem.Termo = "";
                    oMensagem.messagebody = sMensagemRetorno;
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    if (sMensagem[2].ToString().IndexOf('=') == -1)
                    {
                        sEmpresaIntegracao = sMensagem[2].ToString();

                        if (sMensagem.Length == 4)
                        {
                            Empresa = sMensagem[3].ToString();
                        }
                    }
                    else
                    {
                        foreach (string sParte in sMensagem)
                        {
                            if (sParte.IndexOf('=') > 0)
                            {
                                switch (sParte.Trim().Split(new char[] { '=' })[0].ToUpper())
                                {
                                    case "CODPUXADA":
                                        Empresa = sParte.Trim().Split(new char[] { '=' })[1];
                                        break;
                                    case "SERVICO":
                                        Servico = sParte.Trim().Split(new char[] { '=' })[1];
                                        break;
                                    case "INTEGRACAO":
                                        sEmpresaIntegracao = sParte.Trim().Split(new char[] { '=' })[1];
                                        break;
                                }
                            }
                        }
                    }

                    if ((Empresa != "") && (bExecutar))
                    {
                        sSql = "select * from tb_empresas where CodPuxada = '" + Empresa.Trim() + "'";
                        oData = oConfig.oBancoDados.DBQuery(sSql);

                        if (oData.Rows.Count == 0)
                        {
                            bExecutar = false;
                            sMensagemRetorno = "Código de puxada não identificado";
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
                                sMensagemRetorno = "Serviço não identificado";
                            }
                            else
                            {
                                sMensagemRetorno = "Serviço não identificado ou não associado oMensagem empresa informada";
                            }
                        }
                        else
                        {
                            iServico = Convert.ToInt32(oData.Rows[0]["id_Servico"]);
                        }
                    }

                    if (sEmpresaIntegracao.Trim() != "")
                    {
                        sSql = "select ti.id_tipo_integracao" +
                              " from tb_tipo_integracao ti" +
                               "  where upper(ti.cd_tipo_integracao) = '" + sEmpresaIntegracao.Trim().ToUpper() + "'";

                        oData = oConfig.oBancoDados.DBQuery(sSql);

                        if (oData.Rows.Count == 0)
                        {
                            bExecutar = false;
                            sMensagemRetorno = "Integração não encontrada";
                        }
                        else
                        {
                            iTipoIntegracao = Convert.ToInt32(oData.Rows[0]["id_tipo_integracao"]);
                        }
                    }

                    if (!bExecutar)
                    {
                        oMensagem.Termo = "";
                        oMensagem.messagebody = sMensagemRetorno;
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                    else
                    {
                        if (bExecutar)
                        {
                            oConfig.oBancoDados.DBProcedure("sp_tarefas_processar", new clsCampo[] {new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = oMensagem.contactuid},
                                                                                                   new clsCampo {Nome = "id_servico", Tipo = DbType.Int16, Valor = iServico},
                                                                                                   new clsCampo {Nome = "id_empresa", Tipo = DbType.Int16, Valor = iEmpresa },
                                                                                                   new clsCampo {Nome = "idTipoIntegracao", Tipo = DbType.Int16, Valor = iTipoIntegracao }});
                        }

                        oMensagem.Termo = "";
                        oMensagem.messagebody = "Integração iniciada";
                        oMensagem.status = Constantes.const_Status_Enviado;
                        oMensagem.EnviarSemTratar = true;
                        oConfig.NaoValidarLicenca = true;
                    }
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_BotAtivar(ref Config oConfig, ref Mensagem oMensagem)
        {
            string[] sMensagem = oMensagem.messagebody.Trim().Split(new char[] { '.' });
            string sSql = "";
            string sMensagemEnviar = "";
            string sCodPuxada = "";

            // bot.ativar.telefone.nome.parceiro.bot.servico.termo_boas_vindas

            if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
            {
                if ((sMensagem.Length != 9))
                {
                    oMensagem.Termo = "";
                    oMensagem.messagebody = "Está faltando parâmentro para a execução do comando. Formato correto é [/bot.ativar.telefone.nome.parceiro.codpuxada.bot.servico.termo_boas_vindas]";
                    oMensagem.status = Constantes.const_Status_Enviado;
                    oMensagem.EnviarSemTratar = true;
                    oConfig.NaoValidarLicenca = true;
                }
                else
                {
                    sMensagem[2] = _Funcoes.FNC_FormatarTelefone(sMensagem[2]);

                    sCodPuxada = sMensagem[5].ToUpper().Trim();

                    if (sCodPuxada.Length != 8)
                    {
                        sCodPuxada = ("00000000" + sCodPuxada).Substring(("00000000" + sCodPuxada).Length - 8);
                    }

                    sSql = "select count(*)" +
                           " from tb_usuario usu" +
                            " inner join tb_empresas emp on emp.idEmpresa = usu.idEmpresa" +
                            " inner join tb_bot bot on bot.idBot = usu.idBot" +
                           " where usu.Telefone = '" + sMensagem[2].ToUpper() + "'" +
                             " and upper(bot.Apelido) = '" + sMensagem[6].ToUpper() + "'" +
                             " and upper(emp.Empresa) = '" + sMensagem[4].ToUpper() + "'" +
                             " and upper(usu.Licenca) = '" + sMensagem[7].ToUpper() + "'";

                    if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                    {
                        oConfig.oBancoDados.DBProcedure("sp_usuario_ativacao_ins", new clsCampo[] {new clsCampo {Nome = "ds_empresa", Tipo = DbType.String, Valor = sMensagem[4]},
                                                                                                   new clsCampo {Nome = "ds_bot", Tipo = DbType.String, Valor = sMensagem[6]},
                                                                                                   new clsCampo {Nome = "nome", Tipo = DbType.String, Valor = sMensagem[3]},
                                                                                                   new clsCampo {Nome = "telefone", Tipo = DbType.String, Valor = sMensagem[2]},
                                                                                                   new clsCampo {Nome = "licenca", Tipo = DbType.String, Valor = sMensagem[7].ToUpper()},
                                                                                                   new clsCampo {Nome = "codpuxada", Tipo = DbType.String, Valor = sCodPuxada}});

                        sMensagemEnviar = "Número " + sMensagem[2] + " ativado";

                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagemEnviar, true);
                    }
                    else
                    {
                        sMensagemEnviar = "Número " + sMensagem[2] + " já está ativado";

                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagemEnviar, true);
                    }

                    Bot.CarregarTabelaUsuario(ref oMensagem, oConfig.oBancoDados, sMensagem[2], sCodPuxada, sMensagem[7].Trim().ToUpper());

                    oMensagem.ParaLista = sMensagem[2].ToString();
                    oMensagem.Termo = sMensagem[8];
                    oMensagem.status = "";
                    oMensagem.EnviarSemTratar = false;
                    oConfig.NaoValidarLicenca = true;
                    oMensagem.BotNameResposta = sMensagem[6].ToUpper();
                }
            }
            else
            {
                oMensagem.ParaLista = "";
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.status = "";
                oMensagem.EnviarSemTratar = false;
                oConfig.NaoValidarLicenca = true;
            }
        }

        private static void EnviarMensagem_Comando_Broadcast(ref Config oConfig, ref Mensagem oMensagem)
        {
            try
            {
                string sSql = "";
                string sPara = "";
                string sMensagem = oMensagem.messagebody.Trim().Substring(Constantes.const_Comando_Broadcast.Length).Trim();
                bool bErro = false;

                sPara = sMensagem.Substring(0, sMensagem.IndexOf(" ")).Trim();
                oMensagem.messagebody = sMensagem.Substring(sPara.Length).Trim();

                if (oConfig.Provider == Constantes.const_Provider_Telegram)
                {
                    sSql = "select distinct U.Telegram" +
                           " from tb_Usuario U";

                    if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico("select count(*) from tb_Usuario" +
                                                                              " where Admin = 'S' and Telegram = '" + oMensagem.contactuid.Trim() + "'")) == 0)
                        sSql = sSql +
                            " inner join (select Cod_Puxada" +
                                            " from tb_Usuario" +
                                            " where PermitidoBroadCast = 'S'" +
                                              " and Telegram = '" + oMensagem.contactuid.Trim() + "')  PB on PB.Cod_Puxada = U.Cod_Puxada";

                    sSql = sSql +
                       " where U.Ativo = 1" +
                         " and U.Telegram is not null";
                }
                else
                {
                    sSql = "select distinct U.Telefone" +
                           " from tb_Usuario U";

                    if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico("select count(*) from tb_Usuario" +
                                                                              " where Admin = 'S' and Telefone = '" + _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid.Trim()) + "'")) == 0)
                        sSql = sSql +
                            " inner join (select Cod_Puxada" +
                                             " from tb_Usuario" +
                                             " where PermitidoBroadCast = 'S'" +
                                               " and Telefone = '" + _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid.Trim()) + "')  PB on PB.Cod_Puxada = U.Cod_Puxada";

                    sSql = sSql +
                       " where U.Ativo = 1" +
                         " and U.Telefone is not null ";
                }

                sSql = sSql + " and isnull(RecebeBroadCast, 'N') = 'S'";

                if (sPara.Substring(0, 1).ToUpper() == "*")
                {
                    bErro = true;
                }
                else if (sPara.Substring(0, 2).ToUpper() != "P=" &&
                         sPara.Substring(0, 2).ToUpper() != "C=" &&
                         sPara.Substring(0, 2).ToUpper() != "S=")
                {
                    bErro = true;
                }
                else
                {
                    if (sPara.Substring(2).Trim() != "*")
                    {
                        switch (sPara.Substring(0, 1).ToUpper())
                        {
                            case "P":
                                sSql = sSql + " and U.Cod_Puxada in (select CodPuxada" +
                                                                   " from tb_empresas" +
                                                                   " where idEmpresa_Gestora in (select idEmpresa" +
                                                                                               " from tb_empresas" +
                                                                                               " where upper(sigla) in (" + _Funcoes.FNC_Lista_ColocarAspar(sPara.Substring(2).ToUpper()) + "))";
                                break;
                            case "C":
                                sSql = sSql + " and U.Cod_Puxada in (" + _Funcoes.FNC_Lista_ColocarAspar(sPara.Substring(2).ToUpper()) + ")";
                                break;
                            case "S":
                                sSql = sSql + " and U.LICENCA in (" + _Funcoes.FNC_Lista_ColocarAspar(sPara.Substring(2).ToUpper()) + ")";
                                break;
                        }
                    }
                }

                if (bErro)
                {
                    oMensagem.EnviarSemTratar = false;
                    oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                }
                else
                {
                    sPara = "";

                    DataTable oData = oConfig.oBancoDados.DBQuery(sSql);

                    foreach (DataRow oRow in oData.Rows)
                    {
                        _Funcoes.FNC_Str_Adicionar(ref sPara, oRow[0].ToString(), ";");
                    }

                    if (sPara.Trim() == "")
                    {
                        oMensagem.EnviarSemTratar = false;
                        oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                    }
                    else
                    {
                        oMensagem.ParaLista = sPara;
                    }
                }
            }
            catch (Exception)
            {
                oMensagem.EnviarSemTratar = false;
                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                oMensagem.ParaLista = "";
            }
        }

        private static Boolean EnviarMensagem_ValidarComando_Checar(string sMensagem, string sTermo)
        {
            Boolean bOk = false;
            try
            {
                bOk = (sMensagem.Substring(0, sTermo.Length).ToUpper() == sTermo.ToUpper());
            }
            catch (Exception)
            {
            }

            return bOk;
        }

        private static string EnviarMensagem_ValidarComando(string sMensagem)
        {
            string sRet = "";
            string[] sComando = new string[18];

            sComando[0] = Constantes.const_Comando_Broadcast;

            sComando[1] = Constantes.const_Comando_Geral_Ativar;
            sComando[2] = Constantes.const_Comando_Geral_Query;

            sComando[3] = Constantes.const_Comando_BotHelp;
            sComando[4] = Constantes.const_Comando_BotAtivar;
            sComando[5] = Constantes.const_Comando_BotCadastrar;
            sComando[6] = Constantes.const_Comando_BotDescadastrar;
            sComando[7] = Constantes.const_Comando_BotListarServicos;
            sComando[8] = Constantes.const_Comando_BotConsultarCliente;

            sComando[9] = Constantes.const_Comando_BotCobranca;

            sComando[10] = Constantes.const_Comando_DashHelp;
            sComando[11] = Constantes.const_Comando_DashAtivar;
            sComando[12] = Constantes.const_Comando_DashFeira;

            sComando[13] = Constantes.const_Comando_OlaPdvMenu;
            sComando[14] = Constantes.const_Comando_OlaPdvIntegra;
            sComando[15] = Constantes.const_Comando_OlaPdvAjuda;

            sComando[16] = Constantes.const_Comando_Pesquisa;
            sComando[17] = Constantes.const_Comando_Boomerangue;

            try
            {
                sMensagem = sMensagem.Trim();

                foreach (string sItem in sComando)
                {
                    if (EnviarMensagem_ValidarComando_Checar(sMensagem, sItem))
                    {
                        sRet = sItem;
                        break;
                    }

                }
            }
            catch (Exception)
            {
            }

            return sRet;
        }

        private static bool EnviarMensagem_ValidarLicenca(ref Config oConfig,
                                                          ref Mensagem oMensagem,
                                                          clsBancoDados oBancoDados,
                                                          string sStringConexao,
                                                          string sTelefone)
        {
            bool bOk = false;

            try
            {
                DataTable oData;
                string sSql;

                if (oBancoDados.DBStringConexao().Trim() != sStringConexao.Trim() && sStringConexao.Trim() != "")
                {
                    oBancoDados = new clsBancoDados();
                    oBancoDados.DBConectar(Constantes.const_TipoBancoDados_SqlServer, sStringConexao);
                }

                switch (oConfig.Provider.ToUpper().Trim())
                {
                    case Constantes.const_Provider_Telegram:
                        {
                            sSql = "select * from vw_Usuario_Bot" +
                                   " where UPPER(RTRIM(LTRIM(Telegram))) = '" + oMensagem.Usuario.ToUpper().Trim() + "'";
                            oData = oBancoDados.DBQuery(sSql);

                            break;
                        }
                    default:
                        {
                            sSql = "select * from vw_Usuario_Bot" +
                                   " where RTRIM(LTRIM(TELEFONE)) = '" + _Funcoes.FNC_FormatarTelefone(sTelefone.Trim()) + "'" +
                                      " or RTRIM(LTRIM(TELEFONE)) = '" + sTelefone.Trim() + "'";
                            oData = oBancoDados.DBQuery(sSql);

                            break;
                        }
                }

                if (oData != null)
                {
                    if (oData.Rows.Count != 0)
                    {
                        bOk = true;
                        if (oMensagem.Servico.Trim() == "") { oMensagem.Servico = oData.Rows[0]["LICENCA"].ToString(); };

                        switch (oConfig.Provider.ToUpper().Trim())
                        {
                            case Constantes.const_Provider_Telegram:
                                {
                                    oMensagem.contactuid = oData.Rows[0]["TELEFONE"].ToString();

                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception)
            {
                bOk = true;
            }

            return bOk;
        }

        private static void Bot_Atualiza(ref Mensagem oMensagem, ref Config oConfig)
        {
            try
            {
                oMensagem.idBot = Convert.ToInt32(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idBot"));
                oMensagem.idBot_nroOrigem = Convert.ToInt32(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idBot_nroOrigem"));
                oMensagem.idServico = Convert.ToInt32(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idServico", 0));
                oMensagem.Uid = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "nroOrigem").ToString();

                if (oMensagem.idBot != 0)
                {
                    oConfig.Provider = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "tp_ProviderWhatsApp").ToString();

                    switch (_Funcoes.FNC_NuloString(oConfig.Provider).ToUpper().Trim())
                    {
                        case Constantes.const_Provider_ChartAPI:
                            {
                                oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_ChatAPI").ToString();
                                oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_ChatAPI").ToString();
                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_ChatAPI").ToString();

                                break;
                            }
                        case Constantes.const_Provider_Waboxapp:
                            {
                                oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_WaboxApp").ToString();
                                oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_WaboxApp").ToString();
                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_WaboxApp").ToString();
                                break;
                            }
                        case Constantes.const_Provider_BTrive:
                            {
                                oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_BTrive").ToString();
                                oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_BTrive").ToString();
                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_BTrive").ToString();
                                oMensagem.Token2 = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token2_BTrive").ToString();
                                break;
                            }
                        case Constantes.const_Provider_Telegram:
                            {
                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_Telegram").ToString();
                                break;
                            }
                    }
                }
            }
            catch (Exception Ex)
            {
            }
        }

        private static void CarregarConfiguracao(ref Mensagem oMensagem, ref Config oConfig, ref string sTipoBancoDados, ref string sStringConexao)
        {
            try
            {
                DataTable oData;
                string sSql;

                sSql = "select* from vw_bot" +
                       " where botAtivo = 1 and rtrim(upper(Apelido)) = '" + oMensagem.botname.Trim().ToUpper() + "'";
                
                if (oMensagem.idBotExterno != null )
                {
                    sSql = sSql +
                         " and idBotExterno = '" + oMensagem.idBotExterno.Trim() + "'";
                }
                oData = oConfig.oBancoDados.DBQuery(sSql);

                oMensagem.idBot = Convert.ToInt32(oData.Rows[0]["idBot"]);
                oMensagem.idBot_nroOrigem = Convert.ToInt32(oData.Rows[0]["idBot_nroOrigem"]);
                oMensagem.idServico = Convert.ToInt32(oData.Rows[0]["idServico"]);
                oMensagem.Uid = oData.Rows[0]["nroOrigem"].ToString();

                if (oMensagem.idBot != 0)
                {
                    oConfig.Provider = oData.Rows[0]["tp_ProviderWhatsApp"].ToString();
                    sTipoBancoDados = _Funcoes.FNC_NuloString(oData.Rows[0]["tp_bancodados"]);
                    sStringConexao = _Funcoes.FNC_NuloString(oData.Rows[0]["ds_stringconexao"]);

                    switch (_Funcoes.FNC_NuloString(oConfig.Provider).ToUpper().Trim())
                    {
                        case Constantes.const_Provider_ChartAPI:
                            {
                                oConfig.Chat_Url = oData.Rows[0]["Url_ChatAPI"].ToString();
                                oConfig.ChatImage_Url = oData.Rows[0]["UrlImage_ChatAPI"].ToString();
                                oMensagem.Token = oData.Rows[0]["Token_ChatAPI"].ToString();

                                break;
                            }
                        case Constantes.const_Provider_Waboxapp:
                            {
                                oConfig.Chat_Url = oData.Rows[0]["Url_WaboxApp"].ToString();
                                oConfig.ChatImage_Url = oData.Rows[0]["UrlImage_WaboxApp"].ToString();
                                oMensagem.Token = oData.Rows[0]["Token_WaboxApp"].ToString();
                                break;
                            }
                        case Constantes.const_Provider_BTrive:
                            {
                                oConfig.Chat_Url = oData.Rows[0]["Url_BTrive"].ToString();
                                oConfig.ChatImage_Url = oData.Rows[0]["UrlImage_BTrive"].ToString();
                                oMensagem.Token = oData.Rows[0]["Token_BTrive"].ToString();
                                oMensagem.Token2 = oData.Rows[0]["Token2_BTrive"].ToString();
                                break;
                            }
                        case Constantes.const_Provider_Telegram:
                            {
                                oMensagem.Token = oData.Rows[0]["Token_Telegram"].ToString();
                                break;
                            }
                    }
                }

                oData.Dispose();
            }
            catch (Exception Ex)
            {
            }
        }

        public static string Mensagem_TratarParametro(string sMensagem, string sParametro)
        {
            int iParam = 1;
            string[] sLista;

            if (_Funcoes.FNC_NuloString(sParametro) != "")
            {
                if (sParametro.Contains("|"))
                {
                    sLista = sParametro.Split(new char[] { '|' });
                }
                else
                {
                    sLista = new string[] { sParametro };
                }

                foreach (string sItem in sLista)
                {
                    sMensagem = sMensagem.Replace("[PARAM" + ("0" + iParam.ToString().Trim()).PadRight(2) + "]", sItem);

                    iParam = iParam + 1;
                }
            }

            return sMensagem;
        }

        public static string[] EnviarMensagem_Pesquisa(clsBancoDados oMySqlBancoDados, Config oConfig, Mensagem oMensagem, string[] sMensagemCommand, bool AvisarEnvio, ref bool Notificacao)
        {
            string sSql;
            string[] sMensagem = null;

            try
            {
                DataTable oDataPesquisa;
                string sPergunta = "";
                string sTexto = "";

                Notificacao = false;

                sSql = "select * from pluggestao.vw_pesquisa_entrevista_persona" +
                       " where NumeroTelefone in ('" + _Funcoes.FNC_FormatarTelefone(sMensagemCommand[3]).Trim() + "','" + 
                                                       sMensagemCommand[3].Trim() + "')";
                oDataPesquisa = oMySqlBancoDados.DBQuery(sSql);

                sMensagemCommand[3] = _Funcoes.FNC_FormatarTelefone(sMensagemCommand[3]);

                if (_Funcoes.FNC_Data_Vazio(oDataPesquisa))
                {
                    sMensagem = new string[] { "Pesquisa inexistente para o número " + sMensagemCommand[3] };
                    Notificacao = true;
                }
                else
                {
                    oMensagem.dsUrl_Image = "";

                    if (AvisarEnvio)
                    {
                        sTexto = "Pesquisa " + sMensagemCommand[2].Trim().ToUpper() + " enviada para " + _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["NomePersona"]) + " [" + sMensagemCommand[3].Trim().ToUpper() + "]";
                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sTexto, true);
                    }

                    oMensagem.dsUrl_Image = _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["URL_Imagem"]);

                    try
                    {
                        sPergunta = Mensagem_TratarParametro(oDataPesquisa.Rows[0]["Pergunta"].ToString(), _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["DadosComplementares"]));
                        sPergunta = sPergunta.Replace("[Complemento1]", _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["Complemento1"]));
                        sPergunta = sPergunta.Replace("[Complemento2]", _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["Complemento2"]));
                        sPergunta = sPergunta.Replace("[Complemento3]", _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["Complemento3"]));
                        sPergunta = sPergunta.Replace("[Complemento4]", _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["Complemento4"]));
                        sPergunta = sPergunta.Replace("[Complemento5]", _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["Complemento5"]));
                        sPergunta = sPergunta.Replace("[NomePersona]", _Funcoes.FNC_NuloString(oDataPesquisa.Rows[0]["NomePersona"]));
                    }
                    catch (Exception)
                    {
                    }

                    sMensagem = new string[] { sPergunta };

                    string sMessageID = "";

                    sMessageID = _Funcoes.FNC_FormatarTelefone(sMensagemCommand[3]).Trim() + "@c.us|" + _Funcoes.FNC_NuloString(oMensagem.instanceId);

                    oMySqlBancoDados.DBProcedure("sp_entrevista_persona_upd", new clsCampo[] { new clsCampo { Nome = "_idEntrevistaPersona", Tipo = DbType.Int16, Valor = Convert.ToInt32(oDataPesquisa.Rows[0]["idEntrevistaPersona"].ToString()) },
                                                                                               new clsCampo { Nome = "_idUltimaPergunta", Tipo = DbType.Int16, Valor = Convert.ToInt32(oDataPesquisa.Rows[0]["idPesquisaPergunta"].ToString()) },
                                                                                               new clsCampo { Nome = "_idUltimaEntrevistaPersonaPergunta", Tipo = DbType.Int16, Valor = 0 },
                                                                                               new clsCampo { Nome = "_MessageID", Tipo = DbType.String, Valor = sMessageID }});
                }

                sSql = "insert into tb_Usuario_Processo (idBot, idProcesso, TELEFONE, dtAtualizacao)" +
                       " values (" + oMensagem.idBot.ToString() + "," + Constantes.const_Processo_Pesquisa.ToString() + ",'" + sMensagemCommand[3] + "', getdate())";
                oConfig.oBancoDados.DBExecutar(sSql);

                oDataPesquisa.Dispose();
            }
            catch (Exception)
            {
            }

            return sMensagem;
        }

        public static bool EnviarMensagem(ref Mensagem oMensagem,
                                          ref Config oConfig,
                                          bool bEnviar = true)
        {
            bool bOk = false;
            string[] sMensagem = null;
            string sComando = "";
            string sSql = "";
            string sTipoBancoDados = "";
            string sStringConexao = "";
            bool Notificacao = false;
            int _idPerguntaResposta = 0;
            int _idEntrevistaPersonaResposta = 0;
            int _idUltimaPergunta = 0;
            int _idUltimaEntrevistaPersonaPergunta = 0;
            bool PerguntaUsuario = false;

            oConfig.oBancoDados = new clsBancoDados();
            oConfig.oBancoDados.DBConectar(oConfig.tipobancodados, oConfig.dbconstring);

            oMensagem.contactuid = _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid);

            CarregarConfiguracao(ref oMensagem, ref oConfig, ref sTipoBancoDados, ref sStringConexao);

            if (oMensagem.EnviarSemTratar)
            {
                bOk = Bot.EnviarTexto(ref oMensagem, ref oConfig, new string[] { oMensagem.messagebody }, true);
            }
            else
            {
                bool ExecutarRotinaPradro = false;

                try
                {
                    if (oMensagem.messagebody.Trim().Substring(0, 1) == "/")
                    {
                        string sPergunta="";
                        DataTable oData;
                        clsBancoDados oMySqlBancoDados = new clsBancoDados();
                        oMySqlBancoDados.DBConectar(Constantes.const_TipoBancoDados_MySql, "Server=db2.plugthink.com;Port=3306;Database=pluggestao;Uid=usrDBA;Pwd=5iAJANYPyYPs54a0;SslMode=none");

                        sSql = "select eop.idEmpresa, etv.idEntrevista, etv.idPesquisa, etd.idEntidade, etd.Telefone1, etd.Entidade" +
                               " from pluggestao.tb_pesquisa pqs" +
                                " inner join pluggestao.tb_entrevista etv on etv.idPesquisa = pqs.idPesquisa" +
                                " inner join pluggestao.tb_empresa emp on emp.idEmpresa = pqs.idEmpresa" +
                                " inner join olapdv.tb_empresas eop on eop.idEmpresa = emp.idEmpresaOlaPdv" +
                                 " left join olapdv.tb_entidade etd on etd.idEmpresa = eop.idEmpresa" +
                                                                 " and etd.EDI_Integracao = '" + oMensagem.messagebody.Split('.')[1] + "'" +
                                " where pqs.ComandoPesquisa = '" + oMensagem.messagebody.Split('.')[0] + "'";
                        oData = oMySqlBancoDados.DBQuery(sSql);

                        if (!_Funcoes.FNC_Data_Vazio(oData))
                        {
                            if (oData.Rows[0]["idEntidade"] == null)
                            {
                                sMensagem = new string[] { "Não existe esse código de cliente para ess pesquisa" };
                            }
                            else
                            {
                                oMySqlBancoDados.DBProcedure("sp_entrevista_persona_boomerague_ins", new clsCampo[] { new clsCampo { Nome = "_idEmpresa", Tipo = DbType.Int16, Valor = Convert.ToInt32(oData.Rows[0]["idEmpresa"].ToString()) },
                                                                                                                      new clsCampo { Nome = "_idEntrevista", Tipo = DbType.Int16, Valor = Convert.ToInt32(oData.Rows[0]["idEntrevista"].ToString()) },
                                                                                                                      new clsCampo { Nome = "_idEntidade", Tipo = DbType.Int16, Valor = Convert.ToInt32(oData.Rows[0]["idEntidade"].ToString()) } });

                                sMensagem = new string[] { "Pesquisa enviada para " + oData.Rows[0]["Entidade"].ToString() + " (" + oData.Rows[0]["Telefone1"].ToString() + ")" };
                                Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagem[0], true);

                                oMensagem.ParaLista = oData.Rows[0]["Telefone1"].ToString();

                                sSql = "select distinct idPesquisa, idPesquisaPergunta, idEntrevistaPersonaPergunta, idEntrevistaPersona, Pergunta," +
                                                       "DadosComplementares, Complemento1, Complemento2, Complemento3, Complemento4, Complemento5," +
                                                       "NomePersona, CPFCNPJ_Persona" +
                                       " from vw_entrevista_persona_geral" +
                                       " where idEntrevista = " + oData.Rows[0]["idEntrevista"].ToString() +
                                         " and NumeroTelefone = '" + oData.Rows[0]["Telefone1"].ToString() + "'";
                                oData = oMySqlBancoDados.DBQuery(sSql);

                                sPergunta = Mensagem_TratarParametro(oData.Rows[0]["Pergunta"].ToString(), _Funcoes.FNC_NuloString(oData.Rows[0]["DadosComplementares"]));
                                sPergunta = sPergunta.Replace("[Complemento1]", _Funcoes.FNC_NuloString(oData.Rows[0]["Complemento1"]));
                                sPergunta = sPergunta.Replace("[Complemento2]", _Funcoes.FNC_NuloString(oData.Rows[0]["Complemento2"]));
                                sPergunta = sPergunta.Replace("[Complemento3]", _Funcoes.FNC_NuloString(oData.Rows[0]["Complemento3"]));
                                sPergunta = sPergunta.Replace("[Complemento4]", _Funcoes.FNC_NuloString(oData.Rows[0]["Complemento4"]));
                                sPergunta = sPergunta.Replace("[Complemento5]", _Funcoes.FNC_NuloString(oData.Rows[0]["Complemento5"]));
                                sPergunta = sPergunta.Replace("[NOMEPERSONA]", _Funcoes.FNC_NuloString(oData.Rows[0]["NomePersona"]));
                                sPergunta = sPergunta.Replace("[CPFCNPJ_Persona]", _Funcoes.FNC_NuloString(oData.Rows[0]["CPFCNPJ_Persona"]));
                                sPergunta = sPergunta.Replace("[EDI_Integracao]", _Funcoes.FNC_ZerosEsquerda(oMensagem.messagebody.Split('.')[1], 9));
                                sMensagem = new string[] { sPergunta };

                                oMySqlBancoDados.DBProcedure("sp_entrevista_persona_upd", new clsCampo[] { new clsCampo { Nome = "_idEntrevistaPersona", Tipo = DbType.Int16, Valor = Convert.ToInt32(oData.Rows[0]["idEntrevistaPersona"].ToString()) },
                                                                                                           new clsCampo { Nome = "_idUltimaPergunta", Tipo = DbType.Int16, Valor = Convert.ToInt32(oData.Rows[0]["idPesquisaPergunta"].ToString()) },
                                                                                                           new clsCampo { Nome = "_idUltimaEntrevistaPersonaPergunta", Tipo = DbType.Int16, Valor = _Funcoes.FNC_NuloZero(oData.Rows[0]["idEntrevistaPersonaPergunta"]) },
                                                                                                           new clsCampo { Nome = "_MessageID", Tipo = DbType.String, Valor = "" }});

                                sSql = "insert into tb_Usuario_Processo (idBot, idProcesso, TELEFONE, dtAtualizacao)" +
                                       " values (" + oMensagem.idBot.ToString() + "," + Constantes.const_Processo_Pesquisa.ToString() + ",'" + oMensagem.ParaLista + "', getdate())";
                                oConfig.oBancoDados.DBExecutar(sSql);
                            }
                        }

                        oData.Dispose();
                        oMySqlBancoDados.DBDesconectar();
                        oMySqlBancoDados = null;
                    }
                }
                catch (Exception)
                {
                }


                sComando = EnviarMensagem_ValidarComando(oMensagem.messagebody);

                switch (sComando)
                {
                    case Constantes.const_Comando_Pesquisa:
                        {
                            //pesquisa.purina.11.0.enviar.20.0.A
                            const int const_Comando_Bot = 1;
                            const int const_Comando_idEntrevista = 2;
                            const int const_Comando_idPesquisa = 3;
                            const int const_Comando_Acao = 4;
                            const int const_Comando_Qtde = 5;
                            const int const_Comando_IntervaloEnvio = 6;
                            const int const_Comando_SinteticoAnalitico = 7;

                            string[] sMensagemCommand = oMensagem.messagebody.Trim().Split(new char[] { '.' });

                            if (sMensagemCommand[const_Comando_Bot].Trim().ToUpper() != "SPQ")
                            {
                                clsBancoDados oMySqlBancoDados = new clsBancoDados();
                                oMySqlBancoDados.DBConectar(Constantes.const_TipoBancoDados_MySql, "Server=db2.plugthink.com;Port=3306;Database=pluggestao;Uid=usrDBA;Pwd=5iAJANYPyYPs54a0;SslMode=none");

                                sSql = "SELECT count(*) FROM pluggestao.tb_pesquisa where idPesquisa = " + sMensagemCommand[const_Comando_idPesquisa].Trim();

                                if (Convert.ToInt32(oMySqlBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                                {
                                    ExecutarRotinaPradro = true;
                                }
                                else
                                {
                                    int iLimit = 10000;
                                    int iSleep = 1;
                                    string scontactuid = oMensagem.contactuid;
                                    bool bAnalitico = true;
                                    string sNotificarPesquisa = "";

                                    iLimit = Convert.ToInt32(sMensagemCommand[const_Comando_Qtde]);

                                    if (iLimit == 0)
                                    {
                                        iLimit = 10000;
                                    }

                                    iSleep = Convert.ToInt32(sMensagemCommand[const_Comando_IntervaloEnvio]);
                                    bAnalitico = (sMensagemCommand[const_Comando_SinteticoAnalitico].ToString() == "A");

                                    sSql = "select NumeroTelefone, sNotificarPesquisa from pluggestao.vw_entrevista_persona_enviar" +
                                           " where idEntrevista = " + sMensagemCommand[const_Comando_idEntrevista].Trim() +
                                             " and idPesquisa = " + sMensagemCommand[const_Comando_idPesquisa].Trim() +
                                           " limit " + iLimit.ToString();
                                    DataTable oData = oMySqlBancoDados.DBQuery(sSql);

                                    sMensagemCommand[2] = sMensagemCommand[const_Comando_Bot].Trim().ToUpper();
                                    sMensagemCommand[1] = "SPQ";

                                    foreach (DataRow oRow in oData.Rows)
                                    {
                                        sNotificarPesquisa = _Funcoes.FNC_NuloString(oRow["sNotificarPesquisa"]);

                                        oMensagem.ParaLista = scontactuid;
                                        sMensagemCommand[3] = oRow["NumeroTelefone"].ToString();
                                        sMensagem = EnviarMensagem_Pesquisa(oMySqlBancoDados, oConfig, oMensagem, sMensagemCommand, bAnalitico, ref Notificacao);

                                        if (Notificacao)
                                        {
                                            oMensagem.ParaLista = oMensagem.contactuid;
                                        }
                                        else
                                        {
                                            oMensagem.ParaLista = oRow["NumeroTelefone"].ToString();
                                        }

                                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagem[0], true);

                                        System.Threading.Thread.Sleep(iSleep * 1000);
                                    }

                                    string[] sLista = new string[] { oMensagem.contactuid };

                                    if (sNotificarPesquisa != "")
                                    {
                                        if (sNotificarPesquisa.Contains(","))
                                        {
                                            sLista = sNotificarPesquisa.Split(new Char[] { ',' });
                                        }
                                        else
                                        {
                                            sLista = new string[] { sNotificarPesquisa };
                                        }
                                    }

                                    oMensagem.dsUrl_Image = "";
                                    sMensagem = new string[] { "Foi enviada a pesquisa " + sMensagemCommand[1].Trim().ToUpper() + " para " + oData.Rows.Count.ToString() + " número(s)" };

                                    foreach (string Item in sLista)
                                    {
                                        oMensagem.ParaLista = Item;
                                        Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, sMensagem[0], true);
                                    }

                                    oMensagem.contactuid = scontactuid;
                                    oMensagem.ParaLista = oMensagem.contactuid;
                                }

                                oMySqlBancoDados.DBDesconectar();
                                oMySqlBancoDados = null;
                            }
                            else if (EnviarMensagem_Comando_Bot_UserAdmin(ref oConfig, ref oMensagem))
                            {
                                if ((sMensagemCommand.Length < 4))
                                {
                                    sMensagem = new string[] { "Está faltando parâmentro para a execução do comando. Formato correto é [/pesquisa.servico.nomepesquisa.nrotelefone]" };
                                }
                                else
                                {
                                    clsBancoDados oMySqlBancoDados = new clsBancoDados();
                                    oMySqlBancoDados.DBConectar(Constantes.const_TipoBancoDados_MySql, "Server=db2.plugthink.com;Port=3306;Database=pluggestao;Uid=usrDBA;Pwd=5iAJANYPyYPs54a0;SslMode=none");

                                    sMensagem = EnviarMensagem_Pesquisa(oMySqlBancoDados, oConfig, oMensagem, sMensagemCommand, true, ref Notificacao);

                                    if (Notificacao)
                                    {
                                        oMensagem.ParaLista = oMensagem.contactuid;
                                    }
                                    else
                                    {
                                        oMensagem.ParaLista = sMensagemCommand[3].ToString();
                                    }

                                    oMySqlBancoDados.DBDesconectar();
                                    oMySqlBancoDados = null;
                                }
                            }
                            else
                            {
                                oMensagem.ParaLista = "";
                                oMensagem.Termo = Constantes.const_Template_Command_AJUDAERRO;
                                oMensagem.status = "";
                                oMensagem.EnviarSemTratar = false;
                                oConfig.NaoValidarLicenca = true;
                            }

                            break;
                        }
                    case Constantes.const_Comando_Boomerangue:
                        {
                            break;
                        }
                    default:
                        {
                            if (sMensagem == null)
                            {
                                DataTable oData;

                                sSql = "select * from vw_Usuario_Processo" +
                                       " where TELEFONE = '" + oMensagem.contactuid + "'" +
                                         " and isnull(idBot, 0) = " + oMensagem.idBot.ToString() +
                                         " and idProcesso = " + Constantes.const_Processo_Pesquisa.ToString();
                                oData = oConfig.oBancoDados.DBQuery(sSql);

                                if (!_Funcoes.FNC_Data_Vazio(oData))
                                {
                                    clsBancoDados oMySqlBancoDados = new clsBancoDados();
                                    oMySqlBancoDados.DBConectar(Constantes.const_TipoBancoDados_MySql, "Server=db2.plugthink.com;Port=3306;Database=pluggestao;Uid=usrDBA;Pwd=5iAJANYPyYPs54a0;SslMode=none");

                                    sSql = "select * from vw_entrevista_persona where NumeroTelefone = '" + oMensagem.contactuid + "'";
                                    oData = oMySqlBancoDados.DBQuery(sSql);

                                    if (_Funcoes.FNC_Data_Vazio(oData))
                                    {
                                        ExecutarRotinaPradro = (oMensagem.messagedir.Trim().ToUpper() == "I");
                                    }
                                    else
                                    {
                                        if (oMensagem.messagedir.Trim().ToUpper() == "I")
                                        {
                                            DataRow oRowSelecionado = null;

                                            _idPerguntaResposta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["idPerguntaResposta"]));
                                            _idUltimaPergunta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["idProximaPergunta"]));

                                            if (_Funcoes.FNC_NuloZero(oData.Rows[0]["idUltimaEntrevistaPersonaPergunta"]) != 0)
                                            {
                                                PerguntaUsuario = true;

                                                sSql = "select * from vw_entrevista_persona_pergunta where NumeroTelefone = '" + oMensagem.contactuid + "'";
                                                oData = oMySqlBancoDados.DBQuery(sSql);

                                                _idEntrevistaPersonaResposta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["idEntrevistaPersonaResposta"]));
                                                _idUltimaEntrevistaPersonaPergunta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["idEntrevistaPersonaPergunta"]));
                                                _idUltimaPergunta = 0;
                                            }

                                            if (oData.Rows.Count == 1)
                                            {
                                                oRowSelecionado = oData.Rows[0];
                                            }
                                            else
                                            {
                                                bool bValido = false;

                                                foreach (DataRow oRow in oData.Rows)
                                                {
                                                    string[] sLista;

                                                    if (_Funcoes.FNC_NuloString(oRow["Possiveis_Respostas"]) == "")
                                                    {
                                                        bValido = true;
                                                    }
                                                    else
                                                    {
                                                        if (_Funcoes.FNC_NuloString(oRow["Possiveis_Respostas"]).Trim().Contains(","))
                                                        {
                                                            sLista = oRow["Possiveis_Respostas"].ToString().Split(new char[] { ',' });

                                                            foreach (string sItem in sLista)
                                                            {
                                                                if (oMensagem.messagebody.Trim().ToUpper().Contains(sItem.Trim().ToUpper()))
                                                                {
                                                                    bValido = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (_Funcoes.FNC_NuloString(oRow["Possiveis_Respostas"]).Trim().ToUpper() == oMensagem.messagebody.Trim().ToUpper())
                                                            {
                                                                bValido = true;
                                                            }
                                                        }
                                                    }

                                                    if (!bValido)
                                                    {
                                                        sMensagem = new string[] { oRow["MensagemNaoConformidade"].ToString() };
                                                    }
                                                    else
                                                    {
                                                        if (_Funcoes.FNC_NuloString(oRow["Resposta_Equivalentes"]).Trim() == "")
                                                        {
                                                            oRowSelecionado = oRow;
                                                        }
                                                        else
                                                        {
                                                            if (_Funcoes.FNC_NuloString(oData.Rows[0]["Resposta_Equivalentes"]).Trim().Contains(","))
                                                            {
                                                                sLista = oRow["Resposta_Equivalentes"].ToString().Split(new char[] { ',' });

                                                                foreach (string sItem in sLista)
                                                                {
                                                                    if (oMensagem.messagebody.Trim().ToUpper().Contains(sItem.Trim().ToUpper()))
                                                                    {
                                                                        oRowSelecionado = oRow;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                oRowSelecionado = oRow;
                                                            }
                                                        }

                                                        if (oRowSelecionado != null)
                                                        {
                                                            _idPerguntaResposta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oRowSelecionado["idPerguntaResposta"]));
                                                            _idUltimaPergunta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oRowSelecionado["idProximaPergunta"]));

                                                            if (_Funcoes.FNC_NuloString(oRowSelecionado["tpTipoFluxoMensagem"].ToString()) == "PU")
                                                            {
                                                                PerguntaUsuario = true;

                                                                sSql = "select * from vw_entrevista_persona_pergunta where NumeroTelefone = '" + oMensagem.contactuid + "'";
                                                                oData = oMySqlBancoDados.DBQuery(sSql);

                                                                oRowSelecionado = oData.Rows[0];
                                                            }

                                                            break;
                                                        }
                                                    }
                                                }
                                            }

                                            if ((oRowSelecionado != null))
                                            {
                                                bool bValido = true;

                                                switch (_Funcoes.FNC_NuloZero(oRowSelecionado["idTipoPergunta"].ToString()))
                                                {
                                                    case const_TipoPergunta_SimNao:
                                                        {
                                                            break;
                                                        }
                                                    case const_TipoPergunta_Texto:
                                                        {
                                                            break;
                                                        }
                                                    case const_TipoPergunta_Email:
                                                        {
                                                            break;
                                                        }
                                                    case const_TipoPergunta_Numero:
                                                        {
                                                            if (!_Funcoes.FNC_IsNumeric(oMensagem.messagebody))
                                                            {
                                                                bValido = false;
                                                            }
                                                            else
                                                            {
                                                                if (_Funcoes.FNC_NuloZero(oRowSelecionado["ValorMinino"].ToString()) >
                                                                    _Funcoes.FNC_StringValor(oMensagem.messagebody))
                                                                {
                                                                    bValido = false;
                                                                }
                                                                if ((_Funcoes.FNC_NuloZero(oRowSelecionado["ValorMaximo"].ToString()) <
                                                                     _Funcoes.FNC_StringValor(oMensagem.messagebody)) &&
                                                                    (_Funcoes.FNC_NuloZero(oRowSelecionado["ValorMaximo"].ToString()) != 0))
                                                                {
                                                                    bValido = false;
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    case const_TipoPergunta_CPF:
                                                        {
                                                            break;
                                                        }
                                                    case const_TipoPergunta_CNPJ:
                                                        {
                                                            break;
                                                        }
                                                    case const_TipoPergunta_FaixaNumeros:
                                                        {
                                                            break;
                                                        }
                                                    case const_TipoPergunta_Data:
                                                        {
                                                            break;
                                                        }
                                                }

                                                if (bValido)
                                                {
//                                                    if (_Funcoes.FNC_NuloString(oRowSelecionado["tpTipoFluxoMensagem"].ToString()) == "PF")
                                                    if (_Funcoes.FNC_NuloString(oRowSelecionado["tpTipoFluxoMensagem"].ToString()) == "")
                                                        {
                                                            if (PerguntaUsuario)
                                                        {
                                                            sSql = "select * from vw_EntrevistaPersona_Sequencia" +
                                                                   " where idEntrevistaPersona = " + oRowSelecionado["idEntrevistaPersona"].ToString();
                                                            oData = oMySqlBancoDados.DBQuery(sSql);

                                                            _idUltimaPergunta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["idPesquisaPergunta"]));

                                                            oRowSelecionado = oData.Rows[0];
                                                        }
                                                        else
                                                        {
                                                            sSql = "update tb_Usuario_Processo set dtFinalizado = getdate()" +
                                                                   "  where idBot = " + oMensagem.idBot.ToString() +
                                                                      " and idProcesso = " + Constantes.const_Processo_Pesquisa.ToString() +
                                                                      " and TELEFONE = '" + oMensagem.contactuid + "'";
                                                            oConfig.oBancoDados.DBExecutar(sSql);
                                                        }
                                                    }
                                                    else if (PerguntaUsuario)
                                                    {
                                                        _idEntrevistaPersonaResposta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["idEntrevistaPersonaResposta"]));
                                                        _idUltimaEntrevistaPersonaPergunta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["idEntrevistaPersonaPergunta"]));
                                                    }

                                                    oMySqlBancoDados.DBProcedure("sp_entrevista_resposta_ins", new clsCampo[] { new clsCampo { Nome = "_idEntrevistaPersona", Tipo = DbType.Int16, Valor = Convert.ToInt32(oRowSelecionado["idEntrevistaPersona"].ToString()) },
                                                                                                                                new clsCampo { Nome = "_idPerguntaResposta", Tipo = DbType.Int16, Valor = _idPerguntaResposta },
                                                                                                                                new clsCampo { Nome = "_idEntrevistaPersonaResposta", Tipo = DbType.Int16, Valor = _idEntrevistaPersonaResposta },
                                                                                                                                new clsCampo { Nome = "_idUltimaPergunta", Tipo = DbType.Int16, Valor = _idUltimaPergunta },
                                                                                                                                new clsCampo { Nome = "_idUltimaEntrevistaPersonaPergunta", Tipo = DbType.Int16, Valor = _idUltimaEntrevistaPersonaPergunta },
                                                                                                                                new clsCampo { Nome = "_dsResposta", Tipo = DbType.String, Valor = oMensagem.messagebody } });

                                                    sMensagem = new string[] { Mensagem_TratarParametro(oRowSelecionado["Pergunta"].ToString(), _Funcoes.FNC_NuloString(oRowSelecionado["DadosComplementares"].ToString())) };
                                                }
                                                else
                                                {
                                                    sMensagem = new string[] { oRowSelecionado["MensagemNaoConformidade"].ToString() };
                                                }

                                                break;
                                            }
                                        }
                                        else
                                        {
                                            string sMessageID = "";

                                            sMessageID = oMensagem.To.Trim() + "|" + oMensagem.instanceId.Trim();

                                            switch (oMensagem.chat_status)
                                            {
                                                case "delivered":
                                                    {
                                                        oMySqlBancoDados.DBProcedure("sp_entrevista_persona_entregue_upd", new clsCampo[] { new clsCampo { Nome = "_MessageID", Tipo = DbType.String, Valor = sMessageID } });

                                                        break;
                                                    }
                                                case "viewed":
                                                    {
                                                        oMySqlBancoDados.DBProcedure("sp_entrevista_persona_leitura_upd", new clsCampo[] { new clsCampo { Nome = "_MessageID", Tipo = DbType.String, Valor = sMessageID } });

                                                        break;
                                                    }
                                            }
                                        }
                                    }

                                    oMySqlBancoDados.DBDesconectar();
                                    oMySqlBancoDados = null;
                                }
                                else
                                {
                                    if (oMensagem.messagedir.Trim().ToUpper() == "I")
                                    {
                                        ExecutarRotinaPradro = true;
                                    }
                                    else
                                    {
                                        EnviarMensagem_Gravar(ref oConfig, ref oMensagem, sTipoBancoDados, sStringConexao);
                                    }
                                }
                            }

                            break;
                        }
                }

                if (sMensagem != null)
                {
                    bOk = Bot.EnviarTexto(ref oMensagem, ref oConfig, sMensagem, true);
                    EnviarMensagem_Gravar(ref oConfig, ref oMensagem, sTipoBancoDados, sStringConexao);
                }
                else if (ExecutarRotinaPradro)
                {
                    bOk = EnviarMensagem_Padrao(ref oMensagem, ref oConfig, bEnviar);
                }
                else
                {
                    EnviarMensagem_Gravar(ref oConfig, ref oMensagem, sTipoBancoDados, sStringConexao);
                }
            }

            return bOk;
        }

        public static bool EnviarMensagem_Padrao(ref Mensagem oMensagem,
                                                 ref Config oConfig,
                                                 bool bEnviar = true)
        {
            bool bOk = false;
            bool bAcessoLiberado = false;
            string sSql;
            string message_body;

            if (oConfig.oBancoDados != null)
            {
                oConfig.oBancoDados = new clsBancoDados();
                oConfig.oBancoDados.DBConectar(oConfig.tipobancodados, oConfig.dbconstring);
            }

            message_body = oMensagem.messagebody;
            
            bool bPesquisa = false;

            try
            {
                //Tratar somente oMensagem de entrada (Mensagens enviadas pelos os usuário e não mensagens de confirmação do Chat Bot)
                object Valor;

                Bot.CarregarTabelas(ref oMensagem, ref oConfig, true);
                Valor = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idBot");

                oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem] = oConfig.oBancoDadosBot.DBQuery("select * from vw_message_ultmsg_contact_i where contact_uid = '" + oMensagem.contactuid.Trim() + "'");

                if (!_Funcoes.FNC_Data_Vazio(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem]))
                {
                    if ((_Funcoes.FNC_NuloZero(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["idMessageTerms"]) == 69 ||
                         _Funcoes.FNC_NuloZero(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["idMessageTerms"]) == 71 ||
                         _Funcoes.FNC_NuloZero(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["idMessageTerms"]) == 72 ||
                         _Funcoes.FNC_NuloZero(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["idMessageTerms"]) == 73) &&
                        ((EnviarMensagem_ValidarComando(oMensagem.messagebody) == "")))
                    {
                        oMensagem.Servico = "SPQ";
                        oMensagem.idServico = 16;
                        bPesquisa = true;
                    }
                }

                if (!bPesquisa)
                {
                    Bot.CarregarTabelaUsuario(ref oMensagem, oConfig.oBancoDados, _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid), "", "");

                    if (_Funcoes.FNC_NuloString(oMensagem.CodPuxada).Trim() == "")
                    {
                        oMensagem.CodPuxada = _Funcoes.FNC_NuloString(Bot.Tabelas_BuscarValor(ref oMensagem, "TB_USUARIO", "Cod_Puxada"));
                    }

                    Bot.CarregarTabelaEmpresa(ref oConfig, ref oMensagem, oMensagem.CodPuxada);
                }

                oMensagem.idBot = 0;

                if (_Funcoes.FNC_NuloString(Valor) != "")
                {
                    oMensagem.idBot = Convert.ToInt32(Valor);
                    oMensagem.idBot_nroOrigem = Convert.ToInt32(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idBot_nroOrigem"));
                    oMensagem.idServico = Convert.ToInt32(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "idServico", 0));
                    oMensagem.Uid = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "nroOrigem").ToString();

                    //if (_Funcoes.FNC_NuloString(oMensagem.Servico).Trim() == "") { oMensagem.Servico = _Funcoes.FNC_NuloString(Bot.Tabelas_BuscarValor(ref oMensagem, "TB_USUARIO", "LICENCA")); };
                    if (_Funcoes.FNC_NuloString(oMensagem.Servico).Trim() == "") { oMensagem.Servico = _Funcoes.FNC_NuloString(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "cd_Servico")); };
                    if (_Funcoes.FNC_NuloString(oConfig.Provider).Trim() == "") { oConfig.Provider = _Funcoes.FNC_NuloString(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "tp_ProviderWhatsApp")); };
                }

                Bot.CarregarTabelaServico(ref oConfig, ref oMensagem, oMensagem.Servico);

                switch (oMensagem.Servico)
                {
                    case Constantes.const_BotServico_Sistema_KITEI_OLD:
                        break;
                    default:
                        {
                            if (oMensagem.messagedir.Trim().ToUpper() == "I")
                            {
                                int idMessageTerms = 0;
                                string[] sMensagem;

                                oMensagem.Terms = null;

                                if (Declaracao.otbmessageterms.messageterms == null)
                                {
                                    Bot.Terms_Carregar(ref oMensagem, ref oConfig);
                                }

                                if (oMensagem.idBot != 0)
                                {
                                    if (_Funcoes.FNC_NuloString(oConfig.Provider).ToUpper().Trim() == Constantes.const_Provider_WhatsApp)
                                    {
                                        oConfig.Provider = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "tp_ProviderWhatsApp").ToString();
                                    }

                                    switch (_Funcoes.FNC_NuloString(oConfig.Provider).ToUpper().Trim())
                                    {
                                        case Constantes.const_Provider_ChartAPI:
                                            {
                                                oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_ChatAPI").ToString();
                                                oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_ChatAPI").ToString();
                                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_ChatAPI").ToString();

                                                break;
                                            }
                                        case Constantes.const_Provider_Waboxapp:
                                            {
                                                oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_WaboxApp").ToString();
                                                oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_WaboxApp").ToString();
                                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_WaboxApp").ToString();
                                                break;
                                            }
                                        case Constantes.const_Provider_BTrive:
                                            {
                                                oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_BTrive").ToString();
                                                oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_BTrive").ToString();
                                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_BTrive").ToString();
                                                oMensagem.Token2 = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token2_BTrive").ToString();
                                                break;
                                            }
                                        case Constantes.const_Provider_Telegram:
                                            {
                                                oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_Telegram").ToString();
                                                break;
                                            }
                                    }
                                }

                                if (message_body != null || oMensagem.Termo != null)
                                {
                                    if (bPesquisa)
                                    {
                                        bAcessoLiberado = true;
                                    }
                                    else
                                    {
                                        if (message_body.Trim().ToUpper() == "CONFIRMO" ||
                                            message_body.Trim().ToUpper() == "SIM" ||
                                            message_body.Trim().ToUpper() == "YES")
                                        {
                                            if (!_Funcoes.FNC_Data_Vazio(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem]))
                                            {
                                                switch (oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["command"].ToString())
                                                {
                                                    case Constantes.const_Template_Command_HNK_COBRANCA_TESTE:
                                                        {
                                                            oMensagem.messagebody = "Ola! " + oMensagem.contactname + Environment.NewLine +
                                                                                    "Sou Sofia" + Environment.NewLine + Environment.NewLine +
                                                                                    "Passando pra lembrar que dia 06/11/2019" +
                                                                                    " venceu seu boleto 48657812" +
                                                                                    " no valor de R$ 512,59" + Environment.NewLine + Environment.NewLine +
                                                                                    "Obrigado";
                                                            bOk = Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, oMensagem.messagebody, bEnviar);
                                                            oMensagem.messagebody = "Ola! " + oMensagem.contactname + Environment.NewLine +
                                                                                    "Sou Sofia" + Environment.NewLine + Environment.NewLine +
                                                                                    "Passando pra lembrar que dia 07/11/2019" +
                                                                                    " venceu seu boleto 48368812" +
                                                                                    " no valor de R$ 317,89" + Environment.NewLine + Environment.NewLine +
                                                                                    "Obrigado";
                                                            bOk = Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, oMensagem.messagebody, bEnviar);
                                                            oMensagem.messagebody = "Ola! " + oMensagem.contactname + Environment.NewLine +
                                                                                    "Sou Sofia" + Environment.NewLine + Environment.NewLine +
                                                                                    "Passando pra lembrar que dia 11/11/2019" + Environment.NewLine +
                                                                                    " ira vencer seu boleto 111849421" + Environment.NewLine +
                                                                                    " no valor de R$ 378,90" + Environment.NewLine + Environment.NewLine +
                                                                                    "Obrigado";
                                                            bOk = Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, oMensagem.messagebody, bEnviar);
                                                            oMensagem.messagebody = "Ola! " + oMensagem.contactname + Environment.NewLine +
                                                                                    "Sou Sofia" + Environment.NewLine + Environment.NewLine +
                                                                                    "Passando pra lembrar que dia 12/11/2019" + Environment.NewLine +
                                                                                    " ira vencer seu boleto 11157812" + Environment.NewLine +
                                                                                    " no valor de R$ 249,90" + Environment.NewLine + Environment.NewLine +
                                                                                    "Obrigado";

                                                            oMensagem.Termo = "";
                                                            oMensagem.SemServico = true;
                                                            oMensagem.EnviarSemTratar = true;
                                                            break;
                                                        }
                                                    case Constantes.const_Template_Command_CONFIRMARNOTIFICACAO:
                                                        {
                                                            sSql = "SELECT NOME FROM TB_CLIENTE WHERE TELEFONE = '" + _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid) + "'";
                                                            oMensagem.contactname = oConfig.oBancoDados.DBQuery_ValorUnico(sSql).ToString();

                                                            oConfig.oBancoDados.DBProcedure("SP_CLIENTE_NOTIFICAR", new clsCampo[] { new clsCampo { Nome = "TELEFONE", Tipo = DbType.String, Valor = oMensagem.contactuid } });

                                                            oMensagem.Termo = Constantes.const_Template_Command_NOTIFICACAOCONFIRMADA;
                                                            oMensagem.SemServico = true;
                                                            message_body = oMensagem.messagebody;
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            switch (oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["tp_status"].ToString())
                                                            {
                                                                case Constantes.const_Status_PendenteConfirmacao:
                                                                    {
                                                                        oMensagem.status = Constantes.const_Status_Enviado;
                                                                        oMensagem.messagebody = oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["command"].ToString();
                                                                        message_body = oMensagem.messagebody;
                                                                        oMensagem.idMensagem = Convert.ToInt32(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["idMessage"]);

                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        if (_Funcoes.FNC_NuloZero(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["idMessageTerms"]) == 77)
                                                                        {
                                                                            oMensagem.messagebody = "Informe o código";
                                                                            oMensagem.command = Constantes.const_Template_Command_BOOMERANGUE_PURINA;

                                                                            oMensagem.Termo = "";
                                                                            oMensagem.SemServico = true;
                                                                            oMensagem.EnviarSemTratar = true;

                                                                            break;
                                                                        }

                                                                        break;
                                                                    }
                                                            }

                                                            break;
                                                        }
                                                }
                                            }
                                        }

                                        try
                                        {
                                            if (oMensagem.messagebody.Trim().Substring(0, 3).ToUpper() == "SIM")
                                            {
                                                sSql = "SELECT COUNT(*) FROM tb_Usuario_Boomerangue" +
                                                        " WHERE TELEFONE = '" + _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid).Trim() + "'" +
                                                            " AND DATA_ACEITE IS NULL" +
                                                            " AND DATA_CANCELADO IS NULL";
                                                if (Convert.ToInt32(oConfig.oBancoDados.DBQuery_ValorUnico(sSql)) != 0)
                                                {
                                                    oMensagem.messagebody = oMensagem.messagebody.Substring(3);
                                                }
                                            }
                                        }
                                        catch (Exception Ex)
                                        {
                                        }

                                        try
                                        {
                                            if (Int32.Parse(oMensagem.messagebody.Trim()) > 0)
                                            {
                                                DataTable oDataPsq = null;

                                                sSql = "SELECT * FROM tb_Usuario_Boomerangue" +
                                                        " WHERE TELEFONE = '" + _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid).Trim() + "'" +
                                                            " AND PROTOCOLO = '" + Convert.ToInt32(oMensagem.messagebody).ToString().Trim() + "'" +
                                                            " AND DATA_ACEITE IS NULL" +
                                                            " AND DATA_CANCELADO IS NULL";
                                                oDataPsq = oConfig.oBancoDados.DBQuery(sSql);

                                                if (!_Funcoes.FNC_Data_Vazio(oDataPsq))
                                                {
                                                    double idMessage = 0;

                                                    if (!_Funcoes.FNC_Data_Vazio ( oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem]))
                                                    {
                                                        idMessage =  _Funcoes.FNC_NuloZero(oMensagem.oTabela[Constantes.const_TabelasAuxiliares_TabelaView_ultMensagem].Rows[0]["idMessage"]);
                                                    }

                                                    oMensagem.Servico = "BPU";

                                                    FlexXTools.FlexXTools_BOOMERANGUE_PURINA_ACEITE(ref oConfig,
                                                                                                    ref oMensagem,
                                                                                                    _Funcoes.FNC_FormatarTelefone(oMensagem.contactuid).Trim(),
                                                                                                    Convert.ToInt32(oDataPsq.Rows[0]["PROTOCOLO"].ToString()).ToString(),
                                                                                                    idMessage);
                                                }

                                                oDataPsq.Dispose();
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        if (EnviarMensagem_ValidarComando(oMensagem.messagebody) != "")
                                        {
                                            oMensagem.command = message_body;
                                            oMensagem.EnviarSemTratar = true;

                                            switch (EnviarMensagem_ValidarComando(message_body))
                                            {
                                                case Constantes.const_Comando_BotCobranca:
                                                    {
                                                        EnviarMensagem_Comando_Cobranca(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_Pesquisa:
                                                    {
                                                        oConfig.UsuarioAdminValido = true;
                                                        EnviarMensagem_Comando_Pesquisa(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_DashFeira:
                                                    {
                                                        // /feira.hnk.codpuxada.telefone.nome
                                                        string[] sMensagem_DashFeira = oMensagem.messagebody.Trim().Split(new char[] { '.' });

                                                        oMensagem.messagebody = "/ativar.sofia." + sMensagem_DashFeira[1] + "." + sMensagem_DashFeira[2] + ".EMPRESA." + sMensagem_DashFeira[3] + "." + sMensagem_DashFeira[4] + ".0001";
                                                        EnviarMensagem_Comando_Geral_Ativar(ref oConfig, ref oMensagem);
                                                        oMensagem.messagebody = "/ativar.flexxpower." + sMensagem_DashFeira[1] + "." + sMensagem_DashFeira[2] + ".EMPRESA." + sMensagem_DashFeira[3] + "." + sMensagem_DashFeira[4] + ".0001";
                                                        EnviarMensagem_Comando_Geral_Ativar(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_Geral_Query:
                                                    {
                                                        EnviarMensagem_Comando_Geral_Query(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_Geral_Ativar:
                                                    {
                                                        EnviarMensagem_Comando_Geral_Ativar(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_Broadcast:
                                                    {
                                                        EnviarMensagem_Comando_Broadcast(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_BotAtivar:
                                                    {
                                                        EnviarMensagem_Comando_BotAtivar(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_BotCadastrar:
                                                    {
                                                        EnviarMensagem_Comando_BotCadastrar(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_BotDescadastrar:
                                                    {
                                                        EnviarMensagem_Comando_BotDescadastrar(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_BotListarServicos:
                                                    {
                                                        EnviarMensagem_Comando_BotListarServicos(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_BotConsultarCliente:
                                                    {
                                                        EnviarMensagem_Comando_BotConsultarCliente(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_BotHelp:
                                                    {
                                                        EnviarMensagem_Comando_BotHelp(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_OlaPdvAjuda:
                                                case Constantes.const_Comando_OlaPdvMenu:
                                                    {
                                                        EnviarMensagem_Comando_BotOlaPdvMenu(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_OlaPdvIntegra:
                                                    {
                                                        EnviarMensagem_Comando_BotOlaPdvIntegra(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_DashHelp:
                                                    {
                                                        EnviarMensagem_Comando_DashHelp(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                                case Constantes.const_Comando_DashAtivar:
                                                    {
                                                        EnviarMensagem_Comando_DashAtivar(ref oConfig, ref oMensagem);
                                                        break;
                                                    }
                                            }

                                            if (_Funcoes.FNC_NuloString(oMensagem.status).Trim() == "" && _Funcoes.FNC_NuloString(oMensagem.Termo).Trim() == "")
                                            {
                                                oMensagem.status = Constantes.const_Status_PendenteConfirmacao;
                                            }
                                        }

                                        if ((oMensagem.Agente.Trim().ToUpper() != _Funcoes.FNC_NuloString(oMensagem.BotNameResposta).Trim().ToUpper()) &&
                                            (_Funcoes.FNC_NuloString(oMensagem.BotNameResposta).Trim().ToUpper() != ""))
                                        {
                                            sSql = "select * from vw_bot where trim(upper(Apelido)) = '" + oMensagem.BotNameResposta.Trim().ToUpper() + "'";
                                            oConfig.oBancoDados.DBConectar(oConfig.tipobancodados, oConfig.dbconstring);

                                            DataTable oData;
                                            oData = new DataTable();
                                            oData = oConfig.oBancoDados.DBQuery(sSql);

                                            oConfig.Provider = oData.Rows[0]["tp_ProviderWhatsApp"].ToString();

                                            switch (_Funcoes.FNC_NuloString(oConfig.Provider).ToUpper().Trim())
                                            {
                                                case Constantes.const_Provider_ChartAPI:
                                                    {
                                                        oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_ChatAPI").ToString();
                                                        oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_ChatAPI").ToString();
                                                        oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_ChatAPI").ToString();

                                                        break;
                                                    }
                                                case Constantes.const_Provider_Waboxapp:
                                                    {
                                                        oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_WaboxApp").ToString();
                                                        oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_WaboxApp").ToString();
                                                        oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_WaboxApp").ToString();
                                                        break;
                                                    }
                                                case Constantes.const_Provider_BTrive:
                                                    {
                                                        oConfig.Chat_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Url_BTrive").ToString();
                                                        oConfig.ChatImage_Url = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "UrlImage_BTrive").ToString();
                                                        oMensagem.Token = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token_BTrive").ToString();
                                                        oMensagem.Token2 = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "Token2_BTrive").ToString();
                                                        break;
                                                    }
                                                case Constantes.const_Provider_Telegram:
                                                    {
                                                        oMensagem.Token = oData.Rows[0]["Token_Telegram"].ToString();
                                                        break;
                                                    }
                                            }

                                            oData.Dispose();

                                            oMensagem.Agente = oMensagem.BotNameResposta;
                                        }

                                        if ((oConfig.NaoValidarLicenca) || (Bot.Tabelas_BuscarValor(ref oMensagem, "vw_servicos", "SoTelefoneCadastrados", "S").ToString() == "N"))
                                        {
                                            bAcessoLiberado = true;
                                        }
                                        else
                                        {
                                            bAcessoLiberado = EnviarMensagem_ValidarLicenca(ref oConfig,
                                                                                            ref oMensagem,
                                                                                            oConfig.oBancoDados,
                                                                                            "server=sql1.plugthink.com;database=i9ativa;uid=i9admin;pwd=8nbzw4FFrXEzJui", //_Funcoes.FNC_NuloString(Bot.Tabelas_BuscarValor("vw_bot", "ds_stringconexao")),
                                                                                            oMensagem.contactuid);
                                        }

                                        switch (oMensagem.Termo)
                                        {
                                            case Constantes.const_Template_Command_CONFIRMARNOTIFICACAO:
                                                {
                                                    oMensagem.SemServico = true;
                                                    oMensagem.command = Constantes.const_Template_Command_CONFIRMARNOTIFICACAO;
                                                    oConfig.oBancoDados.DBProcedure("SP_CLIENTE_CAD", new clsCampo[] { new clsCampo {Nome = "TELEFONE", Tipo = DbType.String, Valor = oMensagem.contactuid },
                                                                                                                   new clsCampo {Nome = "NOME", Tipo = DbType.String, Valor = oMensagem.contactname }});
                                                    break;
                                                }
                                        }
                                    }

                                    if (bAcessoLiberado)
                                    {
                                        if (!oMensagem.EnviarSemTratar || _Funcoes.FNC_NuloString(oMensagem.Termo) != "")
                                        {
                                            if (_Funcoes.FNC_NuloString(oMensagem.Termo) == "")
                                            { oMensagem.Terms = Declaracao.otbmessageterms.IdentificarTermo(ref oConfig, ref oMensagem, message_body); }
                                            else
                                            { oMensagem.Terms = Declaracao.otbmessageterms.PesquisarPorTermo(ref oConfig, ref oMensagem, oMensagem.Termo, true, "", "", oMensagem.SemServico); }
                                        }
                                    }
                                    else
                                        oMensagem.Terms = Declaracao.otbmessageterms.PesquisarPorTermo(ref oConfig, ref oMensagem, Constantes.const_Template_Command_SEMACESSO, true);

                                    if (oMensagem.Terms != null)
                                    {
                                        if (oMensagem.idMessageTerms == 0)
                                            oMensagem.idMessageTerms = oMensagem.Terms.idMessageTerms;

                                        if (_Funcoes.FNC_NuloString(oMensagem.Terms.WS_TipoRetornoTipo).Trim() == "" || _Funcoes.FNC_NuloString(oMensagem.Terms.WS_TipoRetornoTipo) == "I")
                                        {
                                            idMessageTerms = oMensagem.Terms.idMessageTerms;

                                            while (idMessageTerms != 0)
                                            {
                                                sMensagem = Bot.Terms_Processar(ref oMensagem, ref oConfig);

                                                if (oMensagem.ReprocessarTerms)
                                                {
                                                    oMensagem.Terms = Declaracao.otbmessageterms.PesquisarPorTermo(ref oConfig, ref oMensagem, sMensagem[0], true, "", "", oMensagem.SemServico);
                                                    sMensagem = Bot.Terms_Processar(ref oMensagem, ref oConfig);
                                                }

                                                bOk = Bot.EnviarTexto(ref oMensagem, ref oConfig, sMensagem, bEnviar);

                                                if (oMensagem.Terms.WS_ProximaMensagem != 0)
                                                {
                                                    oMensagem.Terms = Declaracao.otbmessageterms.PesquisarPorId(oMensagem.Terms.WS_ProximaMensagem);
                                                    idMessageTerms = oMensagem.Terms.idMessageTerms;
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                        else
                                            bOk = true;

                                        if (_Funcoes.FNC_NuloString(oMensagem.MensagemFinal) != "")
                                        {
                                            Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, oMensagem.MensagemFinal, true);
                                        }
                                    }
                                    else
                                    {
                                        if (oMensagem.EnviarSemTratar)
                                        {
                                            bOk = Bot.EnviarTexto_Provider(ref oMensagem, ref oConfig, oMensagem.messagebody, bEnviar);
                                        }
                                    }
                                }

                                Bot.Terms_Descarregar();

                                if (!bOk)
                                {
                                    oMensagem.idStatusMensagem = 9;
                                }
                            }
                            else
                            {
                                oMensagem.messagemtd = DateTime.Now;
                                oMensagem.messagemtdd = DateTime.Now;
                                bOk = true;
                            }

                            try
                            {
                                oConfig.oBancoDados.DBDesconectar();
                                oConfig.oBancoDados.DBConectar(oConfig.tipobancodados, oConfig.dbconstring);
                            }
                            catch (Exception Ex)
                            {
                                bOk = bOk;
                            }

                            break;
                        }
                }

                string sql = "";

                if ((oMensagem.idMensagem == 0) && (bEnviar))
                {
                    switch (oMensagem.Servico)
                    {
                        case Constantes.const_BotServico_Sistema_KITEI_OLD:
                            {
                                if (_Funcoes.FNC_NuloString(oMensagem.contactuid).Trim() != "")
                                {
                                    int id_plugzapGateway = Convert.ToInt32(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "id_plugzapGateway", 0));
                                    int id_plugzapUser = Convert.ToInt32(Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "id_plugzapUser", 0));
                                    int idMensagem = 0;

                                    sSql = "SELECT id as idMensagem FROM plugthi1_whats.sys_sms_history where userid = " + id_plugzapUser.ToString() + " and sender = '" + oMensagem.Uid + "' and use_gateway = " + id_plugzapGateway.ToString() + " and receiver = '" + oMensagem.contactuid + "'";
                                    idMensagem = Convert.ToInt32(oConfig.oBancoDadosBot.DBQuery_ValorUnico(sSql, 0));

                                    if (idMensagem == 0)
                                    {
                                        sSql = "INSERT INTO sys_sms_history (userid, sender, receiver, message, amount, use_gateway, api_key, status, sms_type, send_by, created_at, updated_at)" +
                                               " VALUES (#userid, #sender, #receiver, #message, #amount, #use_gateway, #api_key, #status, #sms_type, #send_by, #created_at, #updated_at);";

                                        oConfig.oBancoDadosBot.DBExecutar(sSql, new clsCampo[] {new clsCampo {Nome = "userid", Tipo = DbType.Int32, Valor = id_plugzapUser },
                                                                                                new clsCampo {Nome = "sender", Tipo = DbType.String, Valor = oMensagem.Uid },
                                                                                                new clsCampo {Nome = "receiver", Tipo = DbType.String, Valor = oMensagem.contactuid },
                                                                                                new clsCampo {Nome = "message", Tipo = DbType.String, Valor = oMensagem.messagebody },
                                                                                                new clsCampo {Nome = "amount", Tipo = DbType.String, Valor = 1},
                                                                                                new clsCampo {Nome = "use_gateway", Tipo = DbType.Int32, Valor = id_plugzapGateway },
                                                                                                new clsCampo {Nome = "api_key", Tipo = DbType.String, Valor = "" },
                                                                                                new clsCampo {Nome = "status", Tipo = DbType.String, Valor = "Success|15" },
                                                                                                new clsCampo {Nome = "sms_type", Tipo = DbType.String, Valor = "plain" },
                                                                                                new clsCampo {Nome = "send_by", Tipo = DbType.String, Valor = "receiver" },
                                                                                                new clsCampo {Nome = "created_at", Tipo = DbType.DateTime, Valor = DateTime.Now},
                                                                                                new clsCampo {Nome = "updated_at", Tipo = DbType.DateTime, Valor = DateTime.Now}});

                                        sSql = "SELECT id as idMensagem FROM plugthi1_whats.sys_sms_history where userid = " + id_plugzapUser.ToString() + " and sender = '" + oMensagem.Uid + "' and use_gateway = " + id_plugzapGateway.ToString() + " and receiver = '" + oMensagem.contactuid + "'";
                                        idMensagem = Convert.ToInt32(oConfig.oBancoDadosBot.DBQuery_ValorUnico(sSql));
                                    }

                                    sSql = "INSERT INTO plugthi1_whats.sys_sms_inbox (msg_id, amount, message, status, send_by, mark_read, created_at, updated_at)" +
                                               " VALUES (#msg_id, #amount, #message, #status, #send_by, #mark_read, #created_at, #updated_at);";

                                    oConfig.oBancoDadosBot.DBExecutar(sSql, new clsCampo[] {new clsCampo {Nome = "msg_id", Tipo = DbType.Int32, Valor = idMensagem},
                                                                                           new clsCampo {Nome = "amount", Tipo = DbType.Int32, Valor = 1 },
                                                                                           new clsCampo {Nome = "message", Tipo = DbType.String, Valor = oMensagem.messagebody },
                                                                                           new clsCampo {Nome = "status", Tipo = DbType.String, Valor = "Success|3" },
                                                                                           new clsCampo {Nome = "send_by", Tipo = DbType.String, Valor = "receiver" },
                                                                                           new clsCampo {Nome = "mark_read", Tipo = DbType.String, Valor = "yes" },
                                                                                           new clsCampo {Nome = "created_at", Tipo = DbType.DateTime, Valor = DateTime.Now},
                                                                                           new clsCampo {Nome = "updated_at", Tipo = DbType.DateTime, Valor = DateTime.Now}});
                                }

                                break;
                            }
                        default:
                            {
                                //Gravar oMensagem - Início
                                Declaracao.ErroMensagem = "Gravar";
                                if (oConfig.oBancoDadosBot.DBProcedure("sp_message_ins", new clsCampo[] {new clsCampo {Nome = "Origem", Tipo = DbType.String, Valor = "azure"},
                                                                                                         new clsCampo {Nome = "Agente", Tipo = DbType.String, Valor = oMensagem.botname},
                                                                                                         new clsCampo {Nome = "Bot", Tipo = DbType.String, Valor = oMensagem.botname},
                                                                                                         new clsCampo {Nome = "Token", Tipo = DbType.String, Valor = "ppbh90"},
                                                                                                         new clsCampo {Nome = "uid", Tipo = DbType.String, Valor = oMensagem.Uid},
                                                                                                         new clsCampo {Nome = "contact_uid", Tipo = DbType.String, Valor = oMensagem.contactuid},
                                                                                                         new clsCampo {Nome = "contact_name", Tipo = DbType.String, Valor = oMensagem.contactname},
                                                                                                         new clsCampo {Nome = "contact_type", Tipo = DbType.String, Valor = oMensagem.contacttype},
                                                                                                         new clsCampo {Nome = "message_mtd", Tipo = DbType.DateTime, Valor = oMensagem.messagemtd},
                                                                                                         new clsCampo {Nome = "message_dtm", Tipo = DbType.DateTime, Valor = oMensagem.messagedtm},
                                                                                                         new clsCampo {Nome = "message_rcv", Tipo = DbType.DateTime, Valor = oMensagem.messagercv},
                                                                                                         new clsCampo {Nome = "message_prov_rqt", Tipo = DbType.DateTime, Valor = oMensagem.messagerqt},
                                                                                                         new clsCampo {Nome = "message_prov_rst", Tipo = DbType.DateTime, Valor = oMensagem.messagerst},
                                                                                                         new clsCampo {Nome = "message_uid", Tipo = DbType.String, Valor = oMensagem.messageuid},
                                                                                                         new clsCampo {Nome = "message_cuid", Tipo = DbType.String, Valor = oMensagem.messagecuid},
                                                                                                         new clsCampo {Nome = "message_diretion", Tipo = DbType.String, Valor = oMensagem.messagedir},
                                                                                                         new clsCampo {Nome = "message_type", Tipo = DbType.String, Valor = oMensagem.messagetype},
                                                                                                         new clsCampo {Nome = "message_body", Tipo = DbType.String, Valor = oMensagem.messagebody},
                                                                                                         new clsCampo {Nome = "ack_status", Tipo = DbType.String, Valor = oMensagem.messageack},
                                                                                                         new clsCampo {Nome = "EventoWZ", Tipo = DbType.Int16, Valor = Convert.ToInt32(_Funcoes.FNC_NuloZero(oMensagem.events))},
                                                                                                         new clsCampo {Nome = "processador", Tipo = DbType.String, Valor = Declaracao.processador},
                                                                                                         new clsCampo {Nome = "idStatusMensagem", Tipo = DbType.Int16, Valor = oMensagem.idStatusMensagem},
                                                                                                         new clsCampo {Nome = "idProtocolo", Tipo = DbType.Int16, Valor = 0},
                                                                                                         new clsCampo {Nome = "idMensagemEnviada", Tipo = DbType.Int16, Valor = 0},
                                                                                                         new clsCampo {Nome = "idMessageTerms", Tipo = DbType.Int16, Valor = oMensagem.idMessageTerms},
                                                                                                         new clsCampo {Nome = "ds_provider", Tipo = DbType.Int16, Valor = oConfig.Provider },
                                                                                                         new clsCampo {Nome = "command", Tipo = DbType.Int16, Valor = _Funcoes.FNC_NuloString(oMensagem.command) },
                                                                                                         new clsCampo {Nome = "tp_status", Tipo = DbType.Int16, Valor = _Funcoes.FNC_NuloString(oMensagem.status) }}))
                                    oMensagem.idMensagem = Convert.ToInt64(oConfig.oBancoDadosBot.DBQuery_ValorUnico("select @@Identity"));

                                if (oMensagem.idBot_nroOrigem != 0)
                                {
                                    oConfig.oBancoDados.DBProcedure("sp_bot_nroOrigem_Diario_addenvio", new clsCampo[] { new clsCampo { Nome = "idBot_nroOrigem", Tipo = DbType.Int16, Valor = oMensagem.idBot_nroOrigem } });
                                }

                                if (oMensagem.TipoAcao == Constantes.const_Acao_Boomerangue_ConfirmarEnvio)
                                {
                                    FlexXTools.FlexxTolls_BOOMERANGUE_PURINA_ENVIADO(ref oConfig, ref oMensagem, oMensagem.ParaLista, oMensagem.CampoAuxiliar_02.ToString(), oMensagem.idMensagem, oMensagem.CampoAuxiliar_01);
                                }

                                break;
                            }
                    }
                    //Gravar oMensagem - Fim
                }
                else
                {
                    if (oMensagem.idMensagem != 0)
                    {
                        oConfig.oBancoDadosBot.DBProcedure("sp_message_ins", new clsCampo[] {new clsCampo {Nome = "idMessage", Tipo = DbType.Int16, Valor = oMensagem.idMensagem},
                                                                                            new clsCampo {Nome = "tp_status", Tipo = DbType.Int16, Valor = _Funcoes.FNC_NuloString(oMensagem.status) }});
                    }
                }

                if (bEnviar)
                {
                    if (bOk)
                    {
                        switch (oMensagem.Servico)
                        {
                            case Constantes.const_BotServico_Sistema_KITEI_OLD:
                            default:
                                {
                                    Bot.Custom_uid_Atualizar(ref oConfig, oMensagem.idbot_requisicao, "S");

                                    if (oMensagem.idMensagem != 0)
                                    {
                                        int iAux = 0;

                                        if (oMensagem.Terms != null)
                                            iAux = oMensagem.Terms.idMessageTerms;

                                        Bot.MessageSend(ref oConfig,
                                                        ref oMensagem,
                                                        oMensagem.Custom_uid,
                                                        oMensagem.messagebody_response,
                                                        "",
                                                        oMensagem.Custom_uid,
                                                        iAux);
                                    }

                                    //Gravar log oMensagem - Início
                                    foreach (Mensagem_log.Item Item in Declaracao.oMensagem_log.Itens)
                                    {
                                        sql = "INSERT INTO dbo.tbmessage_log(message_uid,message_log_evento,message_log_processo,message_log_comentario, nr_ordem) VALUES" +
                                                                           "('" + Item.messageuid + "','" + _Funcoes.FNC_Data_DB(Item.message_log_evento) + "','" +
                                                                                  Item.message_log_processo + "','" + Item.message_log_comentario + "'," +
                                                                                  Item.message_log_ordem.ToString() + ")";
                                        oConfig.oBancoDadosBot.DBExecutar(sql);
                                    }
                                    //Gravar log oMensagem - Fim

                                    break;
                                }
                        }
                    }
                }

                oMensagem.oTabela = null;
                oMensagem = null;
                oConfig = null;

                oConfig.oBancoDadosBot.DBDesconectar();
                oConfig.oBancoDadosBot = null;

                oConfig.oBancoDados.DBDesconectar();
                oConfig.oBancoDados = null;
            }
            catch (Exception Ex)
            {
                oConfig.oBancoDados.DBSQL_Log_Gravar(0, "EnviarMensagem", "B", Ex.Message);
            }

            return bOk;
        }

        public static bool EnviarMensagem_Gravar(ref Config oConfig, ref Mensagem oMensagem, string sTipoBancoDados, string sStringConexao)
        {
            try
            {
                if (sTipoBancoDados.Trim() == "")
                {
                    sTipoBancoDados = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "tp_bancodados").ToString();
                }
                if (sStringConexao.Trim() == "")
                {
                    sStringConexao = Bot.Tabelas_BuscarValor(ref oMensagem, "vw_bot", "ds_stringconexao").ToString();
                }

                oConfig.oBancoDadosBot = new clsBancoDados();
                oConfig.oBancoDadosBot.DBConectar(sTipoBancoDados, sStringConexao);

                if (oConfig.oBancoDadosBot.DBProcedure("sp_message_ins", new clsCampo[] {new clsCampo {Nome = "Origem", Tipo = DbType.String, Valor = "azure"},
                                                                        new clsCampo {Nome = "Agente", Tipo = DbType.String, Valor = oMensagem.botname},
                                                                        new clsCampo {Nome = "Bot", Tipo = DbType.String, Valor = oMensagem.botname},
                                                                        new clsCampo {Nome = "Token", Tipo = DbType.String, Valor = "ppbh90"},
                                                                        new clsCampo {Nome = "uid", Tipo = DbType.String, Valor = oMensagem.Uid},
                                                                        new clsCampo {Nome = "contact_uid", Tipo = DbType.String, Valor = oMensagem.contactuid},
                                                                        new clsCampo {Nome = "contact_name", Tipo = DbType.String, Valor = oMensagem.contactname},
                                                                        new clsCampo {Nome = "contact_type", Tipo = DbType.String, Valor = oMensagem.contacttype},
                                                                        new clsCampo {Nome = "message_mtd", Tipo = DbType.DateTime, Valor = oMensagem.messagemtd},
                                                                        new clsCampo {Nome = "message_dtm", Tipo = DbType.DateTime, Valor = oMensagem.messagedtm},
                                                                        new clsCampo {Nome = "message_rcv", Tipo = DbType.DateTime, Valor = oMensagem.messagercv},
                                                                        new clsCampo {Nome = "message_prov_rqt", Tipo = DbType.DateTime, Valor = oMensagem.messagerqt},
                                                                        new clsCampo {Nome = "message_prov_rst", Tipo = DbType.DateTime, Valor = oMensagem.messagerst},
                                                                        new clsCampo {Nome = "message_uid", Tipo = DbType.String, Valor = oMensagem.messageuid},
                                                                        new clsCampo {Nome = "message_cuid", Tipo = DbType.String, Valor = oMensagem.messagecuid},
                                                                        new clsCampo {Nome = "message_diretion", Tipo = DbType.String, Valor = oMensagem.messagedir},
                                                                        new clsCampo {Nome = "message_type", Tipo = DbType.String, Valor = oMensagem.messagetype},
                                                                        new clsCampo {Nome = "message_body", Tipo = DbType.String, Valor = oMensagem.messagebody},
                                                                        new clsCampo {Nome = "ack_status", Tipo = DbType.String, Valor = oMensagem.messageack},
                                                                        new clsCampo {Nome = "EventoWZ", Tipo = DbType.Int16, Valor = Convert.ToInt32(_Funcoes.FNC_NuloZero(oMensagem.events))},
                                                                        new clsCampo {Nome = "processador", Tipo = DbType.String, Valor = Declaracao.processador},
                                                                        new clsCampo {Nome = "idStatusMensagem", Tipo = DbType.Int16, Valor = oMensagem.idStatusMensagem},
                                                                        new clsCampo {Nome = "idProtocolo", Tipo = DbType.Int16, Valor = 0},
                                                                        new clsCampo {Nome = "idMensagemEnviada", Tipo = DbType.Int16, Valor = 0},
                                                                        new clsCampo {Nome = "idMessageTerms", Tipo = DbType.Int16, Valor = oMensagem.idMessageTerms},
                                                                        new clsCampo {Nome = "ds_provider", Tipo = DbType.Int16, Valor = oConfig.Provider },
                                                                        new clsCampo {Nome = "command", Tipo = DbType.Int16, Valor = _Funcoes.FNC_NuloString(oMensagem.command) },
                                                                        new clsCampo {Nome = "tp_status", Tipo = DbType.Int16, Valor = _Funcoes.FNC_NuloString(oMensagem.status) }}))
                    oMensagem.idMensagem = Convert.ToInt64(oConfig.oBancoDadosBot.DBQuery_ValorUnico("select @@Identity"));

                if (oMensagem.idBot_nroOrigem != 0)
                {
                    oConfig.oBancoDados.DBProcedure("sp_bot_nroOrigem_Diario_addenvio", new clsCampo[] { new clsCampo { Nome = "idBot_nroOrigem", Tipo = DbType.Int16, Valor = oMensagem.idBot_nroOrigem } });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool FNC_Enviar(string sPara,
                                      string sCC,
                                      string sAssunto,
                                      string sMensagem,
                                      string[] sAnexo = null)
        {
            bool bOk = false;
            string[] Para = null;

            if (sPara.Contains(";"))
            {
                Para = sPara.Split(new char[] { ';' });
            }
            else
            {
                Array.Resize(ref Para, 1);
                Para[Para.Length - 1] = sPara;
            }

            try
            {
                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
                {
                    smtp.Host = Propriedade_Ler(const_Propriedade_EMail_Host).ToString();
                    smtp.Port = Convert.ToInt32(Propriedade_Ler(const_Propriedade_EMail_Port));
                    smtp.EnableSsl = (Propriedade_Ler(const_Propriedade_EMail_EnableSsl).ToString().ToUpper() == "S");
                    smtp.UseDefaultCredentials = (Propriedade_Ler(const_Propriedade_EMail_UseDefaultCredentials).ToString().ToUpper() == "S");
                    smtp.Credentials = new System.Net.NetworkCredential(Propriedade_Ler(const_Propriedade_EMail_UserName).ToString(),
                                                                        Propriedade_Ler(const_Propriedade_EMail_Password).ToString());

                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                    {
                        mail.From = new System.Net.Mail.MailAddress(Propriedade_Ler(const_Propriedade_EMail_UserName).ToString());

                        if (!string.IsNullOrWhiteSpace(sPara))
                        {
                            foreach (string sTo in Para)
                            {
                                mail.To.Add(new System.Net.Mail.MailAddress(sTo));
                            }
                        }
                        else
                        {
                            goto Sair;
                        }
                        if (!string.IsNullOrWhiteSpace(sCC))
                            mail.CC.Add(new System.Net.Mail.MailAddress(sCC));

                        mail.Subject = sAssunto;
                        mail.Body = sMensagem;

                        if (sAnexo != null)
                        {
                            foreach (string file in sAnexo)
                            {
                                mail.Attachments.Add(new System.Net.Mail.Attachment(file));
                            }
                        }

                        smtp.Send(mail);
                    }
                }

                bOk = true;
            }
            catch (Exception Ex)
            {
            }

        Sair:
            return bOk;
        }
    }
}