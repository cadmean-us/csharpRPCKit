namespace Cadmean.RPC
{
    public class TransientAuthorizationHolder : IAuthorizationTicketHolder
    {
        public JwtAuthorizationTicket Ticket { get; set; }
    }
}