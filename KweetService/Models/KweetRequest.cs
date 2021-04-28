using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KweetService.Models
{
    public class KweetRequest
    {
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
    }
}
