using Exam.App.ViewModels.Receipts;
using SIS.HTTP.Responses;
using System.Globalization;
using System.Linq;

namespace Exam.App.Controllers
{
    public class ReceiptsController : BaseController
    {
        public IHttpResponse Index()
        {
            var receiptsViewModel = this.Db.Receipts.Where(r => r.Recipient.Username == this.User.Username).Select(r => new MyReceiptsViewModel
            {
                Id = r.Id,
                Fee = r.Fee,
                IssuedOn = r.IssuedOn.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Recepient = r.Recipient.Username
            }).ToArray();

            if (receiptsViewModel == null)
            {
                return this.BadRequestError("No receipts to show!");
            }

            return this.View(receiptsViewModel);
        }

        public IHttpResponse Details(int id)
        {
            var viewModel = this.Db.Receipts.Where(r => r.Id == id)
                .Select(r => new ReceiptDetailsViewModel
                {
                    Id = r.Id,
                    DeliveryAddress = r.Package.ShippingAddress,
                    Fee = $"{r.Fee:f2}",
                    IssuedOn = r.IssuedOn.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    PackageDescription = r.Package.Description,
                    PackageWeight = r.Package.Weight,
                    Recepient = r.Recipient.Username
                }).FirstOrDefault();

            if (viewModel == null)
            {
                return this.BadRequestError("Reciept does not exist!");
            }

            return this.View("/Receipts/Details", viewModel);
        }
    }
}
