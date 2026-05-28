using System.ComponentModel.DataAnnotations;

namespace Aplicacao_Web.Models
{
    public class Categoria
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 letras")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatoria")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "A descrição deve ter entre 20 e 200 letras")]
        public string? Descricao { get; set; }


        public ICollection<Produto>? Produtos { get; set; }

    }
}
