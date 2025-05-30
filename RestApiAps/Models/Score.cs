using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiAps.Models
{
    [Table("Pontuacoes")]
    public class Score
    {
        [Key]
        public int Id { get; set; }

        public string? Nome { get; set; }

        public int Pontuacao { get; set; }

        public DateTime DataRegistro { get; set; } = DateTime.Now;
    }
}
