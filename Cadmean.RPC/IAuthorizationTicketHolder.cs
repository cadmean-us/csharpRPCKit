namespace Cadmean.RPC
{
    public interface IAuthorizationTicketHolder
    {
        JwtAuthorizationTicket Ticket { get; set; }
    }
}