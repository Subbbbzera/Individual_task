using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        [Required]
        public string Person { get; set; }
        [Required]
        public string Address { get; set; }
        public int UserId { get; set; }
        public int MedicineId { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("MedicineId")]
        public virtual Medicine Medicine { get; set; }
        [Required(ErrorMessage = "Кількість є обов'язковою")]
        [Range(1, int.MaxValue, ErrorMessage = "Кількість повинна бути більше 0")]
        public int Quantity { get; set; }
    }


}
