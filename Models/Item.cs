using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingList.Models
{
    public class Item
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public int Name { get; set; }

        [Required]
        [Range(1,10000000)]
        public int Quantity { get; set; }
        public bool Done { get; set; }


        public int CartID { get; set; }
        public Cart Cart { get; set; }
    }
}
