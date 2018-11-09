using Exam.App.ViewModels.Packages;
using Exam.Models;
using Exam.Models.Enums;
using SIS.HTTP.Responses;
using SIS.MvcFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Exam.App.Controllers
{
    public class PackagesController : BaseController
    {
        [Authorize]
        public IHttpResponse Details(int id)
        {
            var package = this.Db.Packages.FirstOrDefault(p => p.Id == id);

            if (package == null)
            {
                return this.BadRequestError("Package not found!");
            }

            var estimatedDeliveryDate = string.Empty;

            if (package.Status == Status.Pending)
            {
                estimatedDeliveryDate = "N/A";
            }
            else if (package.Status == Status.Shipped)
            {
                estimatedDeliveryDate = package.EstimatedDeliveryDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else if (package.Status == Status.Delivered || package.Status == Status.Acquired)
            {
                estimatedDeliveryDate = "Delivered";
            }

            var viewModel = new PackageDetailsViewModel
            {
                Id = package.Id,
                Address = package.ShippingAddress,
                EstimatedDeliveryDate = estimatedDeliveryDate,
                Status = package.Status.ToString(),
                Description = package.Description,
                Weight = package.Weight,
                Recepient = package.Recipient.Username
            };

            return this.View(viewModel);
        }

        [Authorize("Admin")]
        public IHttpResponse Create()
        {
            var viewModel = this.Db.Users.Select(u => new CreatePackageViewModel
            {
                Username = u.Username
            }).ToArray();

            return this.View(viewModel);
        }

        [Authorize("Admin")]
        [HttpPost]
        public IHttpResponse Create(CreatePackageInputModel model)
        {
            if (model == null)
            {
                return this.BadRequestError("Invalid input data!");
            }

            var package = new Package
            {
                Description = model.Description,
                EstimatedDeliveryDate = null,
                ShippingAddress = model.ShippingAddress,
                Status = Status.Pending,
                Weight = model.Weight,
                Recipient = this.Db.Users.FirstOrDefault(u => u.Username == model.Recipient)
            };

            this.Db.Packages.Add(package);
            this.Db.SaveChanges();

            return this.Redirect("/");
        }

        [Authorize("Admin")]
        public IHttpResponse Pending()
        {
            var viewModel = this.Db.Packages.Where(p => p.Status == Status.Pending).Select(p => new PendingViewModel
            {
                Id = p.Id,
                Description = p.Description,
                Recipient = p.Recipient.Username,
                ShippingAddress = p.ShippingAddress,
                Weight = $"{p.Weight:f2}"
            }).ToArray();

            if (viewModel == null)
            {
                return this.BadRequestError("No pending orders");
            }

            return this.View(viewModel);
        }

        [Authorize("Admin")]
        public IHttpResponse Shipped()
        {
            var packages = this.Db.Packages.Where(p => p.Status == Status.Shipped).ToArray();

            ShippedViewModel[] viewModel = new ShippedViewModel[packages.Count()];

            for (int i = 0; i < packages.Length; i++)
            {
                var package = packages[i];

                var model = new ShippedViewModel
                {
                    Id = package.Id,
                    Description = package.Description,
                    EstimatedDeliveryDate = package.EstimatedDeliveryDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Recipient = package.Recipient.Username,
                    Weight = $"{package.Weight:f2}"
                };

                viewModel[i] = model;
            }

            if (viewModel == null)
            {
                return this.BadRequestError("No shipped packages!");
            }

            return this.View(viewModel);
        }

        [Authorize("Admin")]
        public IHttpResponse Delivered()
        {
            var viewModel = this.Db.Packages.Where(p => p.Status == Status.Delivered || p.Status == Status.Acquired).Select(p => new DeliveredViewModel
            {
                Id = p.Id,
                Description = p.Description,
                Recipient = p.Recipient.Username,
                ShippingAddress = p.ShippingAddress,
                Weight = $"{p.Weight:f2}"
            }).ToArray();

            if (viewModel == null)
            {
                return this.BadRequestError("No delivered orders");
            }

            return this.View(viewModel);
        }

        [Authorize("Admin")]
        public IHttpResponse Ship(int id)
        {
            var package = this.Db.Packages.FirstOrDefault(p => p.Id == id && p.Status == Status.Pending);

            package.Status = Status.Shipped;

            int days = new Random().Next(20, 40);

            package.EstimatedDeliveryDate = DateTime.Now.AddDays(days);

            this.Db.Packages.Update(package);
            this.Db.SaveChanges();

            return this.Redirect("/");
        }

        [Authorize("Admin")]
        public IHttpResponse Deliver(int id)
        {
            var package = this.Db.Packages.FirstOrDefault(p => p.Id == id && p.Status == Status.Shipped);

            package.Status = Status.Delivered;

            this.Db.Packages.Update(package);
            this.Db.SaveChanges();

            return this.Redirect("/");
        }

        [Authorize("Admin")]
        public IHttpResponse Acquire(int id)
        {
            var package = this.Db.Packages.FirstOrDefault(p => p.Id == id && p.Status == Status.Delivered);

            package.Status = Status.Acquired;

            this.Db.Packages.Update(package);
            this.Db.SaveChanges();

            var recepit = new Receipt
            {
                IssuedOn = DateTime.Now,
                Fee = (decimal)package.Weight * 2.67m,
                Package = package,
                Recipient = package.Recipient
            };

            this.Db.Receipts.Add(recepit);
            this.Db.SaveChanges();

            return this.Redirect("/");
        }
    }
}
