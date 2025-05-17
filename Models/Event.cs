namespace Api_entradas.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime Date { get; set; }
        public int TotalTickets { get; set; }
        public int TicketsSold { get; set; }
    }
}
