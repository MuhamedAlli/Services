
namespace ServicesApp.Domain.Entities
{
    [NotMapped]
    public class Payment
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = null!;
        public decimal Ammount { get; set; }
        public bool State { get; set; }
        public string CardName { get; set; } = null!;
        public int CardNumber { get; set; }
        public int Cvv { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ReferenceNumber { get; set; }
    }
}
