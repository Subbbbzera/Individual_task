using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pharmacy.Models
{

    public class Medicine
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Назва обов’язкова")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Виробник обов’язковий")]  
        public string Manufacturer { get; set; }
        [Required(ErrorMessage = "Ціна обов’язкова")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Кількість є обов'язковою")]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Quantity { get; set; }
        public string AdditionalInfo { get; set; }
        public string ImagePath { get; set; }
    }
}
