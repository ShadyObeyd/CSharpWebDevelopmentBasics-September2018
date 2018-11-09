namespace Exam.App.ViewModels.Receipts
{
    public class ReceiptDetailsViewModel
    {
        public int Id { get; set; }

        public string IssuedOn { get; set; }

        public string DeliveryAddress { get; set; }

        public double PackageWeight { get; set; }

        public string PackageDescription { get; set; }

        public string Recepient { get; set; }

        public string Fee { get; set; }
    }
}
