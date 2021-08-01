using System;
using System.ComponentModel.DataAnnotations;

namespace APIFilmes.Model
{
    /// <summary>
    /// Model para a entidade Filme
    /// </summary>
    public class Filme
    {
        /// <summary>
        /// Código identificador do Filme
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Nome do Filme, obrigatório e com tamanho máximo de 255 caracteres
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "O nome do Filme não pode ser maior que 200 caracteres!")]
        public string Nome { get; set; }

        /// <summary>
        /// Data do Cadastro do Filme
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Situação atual do Filme na base de dados.
        /// 0 - Inativo
        /// 1 - Ativo
        /// </summary>
        public short Ativo { get; set; }
    }
}
