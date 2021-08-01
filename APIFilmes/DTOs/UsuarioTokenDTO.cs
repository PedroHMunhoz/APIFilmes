using System;

namespace APIFilmes.DTOs
{
    public class UsuarioTokenDTO
    {
        public bool Autenticado { get; set; }
        public DateTime DataExpiracao { get; set; }
        public string Token { get; set; }
        public string MensagemAuth { get; set; }
    }
}
