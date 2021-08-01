using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFilmes.Model
{
    [Table("Locacao")]
    public class Locacao
    {
        /// <summary>
        /// Código identificar da locação
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// CPF do cliente que fez a locação
        /// </summary>
        [Required]
        [StringLength(14, ErrorMessage = "O CPF do cliente não pode ser maior do que 14 caracteres!")]
        public string CPFCliente { get; set; }

        /// <summary>
        /// Data em que a locação foi efetuada
        /// </summary>
        [Required]
        public DateTime DataLocacao { get; set; }

        [NotMapped]
        public ICollection<Filme> ListaFilmes { get; set; }
                
        public ICollection<LocacaoItem> ItensLocacao { get; set; }
    }
}
