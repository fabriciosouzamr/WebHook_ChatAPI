using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace btService.Models
{
    public class Repositorio
    {
        public class PafaTratarMensagem_Param
        {
            public string Botname { get; set; }
            public string Servico { get; set; }
            public string Telefone { get; set; }
            public string Termo { get; set; }
            public string Mensagem { get; set; }
        }
    }
}