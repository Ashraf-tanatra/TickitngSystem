using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class TicketAttachments
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public Ticket Ticket { get; set; }

        public string Url { get; set; }

    }
}
