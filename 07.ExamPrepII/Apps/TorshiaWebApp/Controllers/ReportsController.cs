using SIS.HTTP.Responses;
using SIS.MvcFramework;
using System.Globalization;
using System.Linq;
using TorshiaWebApp.ViewModels.Reports;

namespace TorshiaWebApp.Controllers
{
    public class ReportsController : BaseController
    {
        [Authorize("Admin")]
        public IHttpResponse All()
        {
            var reports = this.Db.Reports.Select(r => new ReportAllViewModel
            {
                Id = r.Id,
                Status = r.Status.ToString(),
                TaskTitle = r.Task.Title,
                Level = r.Task.AffectedSectors.Count
            }).ToArray();

            if (reports == null)
            {
                return this.BadRequestErrorWithView("No reports to show");
            }

            return this.View(reports);
        }

        [Authorize("Admin")]
        public IHttpResponse Details(int id)
        {
            var report = this.Db.Reports.Where(r => r.Id == id).Select(r => new ReportDetailsViewModel
            {
                Id = r.Id,
                AffectedSectors = string.Join(", ", r.Task.AffectedSectors.Select(s => s.Sector.ToString())),
                DueDate = r.Task.DueDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                ReportedOn = r.ReportedOn.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                Level = r.Task.AffectedSectors.Count,
                Participants = r.Task.Participants,
                Reporter = this.User.Username,
                Status = r.Status.ToString(),
                TaskDescription = r.Task.Description,
                TaskTitle = r.Task.Title
            }).FirstOrDefault();

            if (report == null)
            {
                return this.BadRequestErrorWithView("Report does not exist!");
            }

            return this.View(report);
        }
    }
}
