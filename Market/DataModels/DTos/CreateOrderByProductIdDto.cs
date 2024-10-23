using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.DTos
{
    public class CreateOrderByProductIdDto
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        public int Quantity { get; set; }
    }
}
