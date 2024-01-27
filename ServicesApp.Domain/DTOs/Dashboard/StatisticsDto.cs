namespace ServicesApp.Domain.DTOs.Dashboard
{
    public class StatisticsDto
    {
        public int ProvidersCount { get; set; }
        public int ClientsCount { get; set; }
        public int ServicesCount { get; set; }
        public int PendingOrdersCount { get; set; }
        public int AcceptedOrdersCount { get; set; }
        public int RejectedOrdersCount { get; set; }
        public int CanceledOrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }

        public int BlockedOrdersCount { get; set; }


    }
}
