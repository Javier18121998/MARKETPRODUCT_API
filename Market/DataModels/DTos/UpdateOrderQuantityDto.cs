using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.DTos
{
    public class UpdateOrderQuantityDto
    {
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        public int NewQuantity { get; set; }
    }
}
