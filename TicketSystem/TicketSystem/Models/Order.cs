using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSystem.Models
{
    public class Order
    {

        public int OrderId { get; set; }

       
        [Range(1,190)]
        public int AmountOfTickets { get; set; }
    }
}