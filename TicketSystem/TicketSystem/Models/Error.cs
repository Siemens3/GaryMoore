using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSystem.Models
{
    public class Error
    {
        public int ErrorId { get; set; }
        [StringLength(50000)]
        public string error { get; set; }
    }
}