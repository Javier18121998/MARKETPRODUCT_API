﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.DTos
{
    public class CreateOrderByProductNameAndSizeDto
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "El tamaño del producto es obligatorio.")]
        public string ProductSize { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        public int Quantity { get; set; }
    }
}