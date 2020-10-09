namespace Cadmean.RPC
{
    public class TransientAuthorizationTicketHolder : IAuthorizationTicketHolder
    {
        public JwtAuthorizationTicket Ticket { get; set; }
    }
}