using System;

namespace APIFilmes.DTOs
{
    public class LocacaoDTO
    {
        public int ID { get; set; }
        public string CPFCliente { get; set; }
        public DateTime DataLocacao { get; set; }
    }
}
