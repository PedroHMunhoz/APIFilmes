using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace APIFilmes.Model
{
    /// <summary>
    /// Model para a entidade Genero
    /// </summary>
    public class Genero
    {
        /// <summary>
        /// Construtor da classe Gênero
        /// </summary>
        public Genero()
        {
            // Inicializando a coleção de filmes ao iniciar uma nova classe Gênero
            Filmes = new Collection<Filme>();
        }

        /// <summary>
        /// Código identificador do Gênero
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Nome do Gênero, obrigatório e com tamanho máximo de 255 caracteres
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "O nome do Gênero não pode ser maior que 100 caracteres!")]
        public string Nome { get; set; }

        /// <summary>
        /// Data do Cadastro do Gênero
        /// </summary>        
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Situação atual do Gênero na base de dados.
        /// 0 - Inativo
        /// 1 - Ativo
        /// </summary>
        public short Ativo { get; set; }

        /// <summary>
        /// Propriedade de navegação do EF, para indicar que um mesmo Gênero
        /// possui uma coleção (um ou mais, 1:N) de Filmes
        /// </summary>
        public ICollection<Filme> Filmes { get; set; }
    }
}
