using AutoMapper.Configuration.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFilmes.Model
{
    [Table("Locacao_Item")]
    public class LocacaoItem
    {
        [Key]
        public int ID { get; set; }        

        /// <summary>
        /// Propriedade de navegação para fazer o link com a tabela de locação
        /// </summary>
        public Locacao Locacao { get; set; }

        /// <summary>
        /// Variável para a chave da tabela locação
        /// </summary>
        public int LocacaoID { get; set; }

        /// <summary>
        /// O Filme relativo a este item de locação
        /// </summary>
        public Filme Filme { get; set; }

        /// <summary>
        /// Propriedade de navegação para vincular com a tabela Filme
        /// </summary>
        public int FilmeID { get; set; }
    }
}
