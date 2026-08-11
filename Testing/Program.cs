
using Domain.Entities;

namespace Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            var ticket = new Ticket
            {
                TicketId = 1,
                TicketTitle = "Test"
            };
            Console.WriteLine(ticket);

            ticket.SetAsOnProgress();

            Console.WriteLine(ticket);

            Console.ReadKey();
        }
    }
}