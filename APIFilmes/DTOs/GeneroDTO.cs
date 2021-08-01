using System.Collections.Generic;

namespace APIFilmes.DTOs
{
    public class GeneroDTO
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public ICollection<FilmeDTO> Filmes { get; set; }
    }
}
