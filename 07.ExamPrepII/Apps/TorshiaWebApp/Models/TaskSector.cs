using TorshiaWebApp.Models.Enums;

namespace TorshiaWebApp.Models
{
    public class TaskSector
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public virtual Task Task { get; set; }

        public Sector Sector { get; set; }
    }
}
