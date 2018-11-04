using SIS.HTTP.Responses;
using SIS.MvcFramework;
using System;
using System.Globalization;
using System.Linq;
using TorshiaWebApp.Models;
using TorshiaWebApp.Models.Enums;
using TorshiaWebApp.ViewModels.Tasks;

namespace TorshiaWebApp.Controllers
{
    public class TasksController : BaseController
    {
        [Authorize("Admin")]
        public IHttpResponse Create()
        {
            return this.View();
        }

        [Authorize("Admin")]
        [HttpPost]
        public IHttpResponse Create(TaskInputModel model)
        {
            if (model == null)
            {
                return this.BadRequestErrorWithView("All fields must be filled!");
            }

            var task = new Task
            {
                Title = model.Title,
                DueDate = DateTime.ParseExact(model.DueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Description = model.Description,
                IsReported = false,
                Participants = model.Participants
            };


            if (!string.IsNullOrWhiteSpace(model.AffectedSectors))
            {
                var taskSectors = model.AffectedSectors.Split(new char[] { ' ', ',', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var sectorName in taskSectors)
                {
                    if (Enum.TryParse(sectorName, out Sector result))
                    {
                        return this.BadRequestErrorWithView("Invalid task sector!");
                    }

                    var sector = new TaskSector
                    {
                        Sector = result,
                        Task = task
                    };
                }
            }

            this.Db.Tasks.Add(task);
            this.Db.SaveChanges();

            return this.Redirect($"/Tasks/Details?id={task.Id}");
        }

        [Authorize]
        public IHttpResponse Details(int id)
        {
            var taskViewModel = this.Db.Tasks.Where(t => t.Id == id).Select(t => new TaskDetailsViewModel
            {
                Id = t.Id,
                Title = t.Title,
                AffectedSectors = string.Join(", ", t.AffectedSectors.Select(s => s.Sector.ToString())),
                DueDate = t.DueDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Level = t.AffectedSectors.Count,
                Participants = t.Participants,
                Description = t.Description
            }).FirstOrDefault();

            if (taskViewModel == null)
            {
                return this.BadRequestErrorWithView("Task does not exist!");
            }

            return this.View(taskViewModel);
        }

        [Authorize]
        public IHttpResponse Report(int id)
        {
            var task = this.Db.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return this.BadRequestErrorWithView("Task does not exist");
            }

            if (task.IsReported)
            {
                return this.BadRequestErrorWithView("Task has already been reported!");
            }

            task.IsReported = true;

            this.Db.Tasks.Update(task);
            this.Db.SaveChanges();

            Status status;

            var rnd = new Random().Next(1, 100);

            if (rnd >= 75)
            {
                status = Status.Completed;
            }
            else
            {
                status = Status.Archived;
            }

            var report = new Report
            {
                ReportedOn = DateTime.Now,
                Reporter = this.Db.Users.FirstOrDefault(u => u.Username == this.User.Username),
                Task = task,
                Status = status
            };

            this.Db.Reports.Add(report);
            this.Db.SaveChanges();

            return this.Redirect("/");
        }
    }
}
