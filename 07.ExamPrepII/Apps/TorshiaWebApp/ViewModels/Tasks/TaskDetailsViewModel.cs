using System;

namespace TorshiaWebApp.ViewModels.Tasks
{
    public class TaskDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Level { get; set; }

        public string Participants { get; set; }

        public string DueDate { get; set; }

        public string AffectedSectors { get; set; }

        public string Description { get; set; }
    }
}
