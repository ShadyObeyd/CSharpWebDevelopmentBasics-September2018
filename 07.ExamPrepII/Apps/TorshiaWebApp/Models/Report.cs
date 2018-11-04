namespace TorshiaWebApp.Models
{
    using Enums;
    using System;

    public class Report
    {
        public int Id { get; set; }

        public Status Status { get; set; }

        public DateTime ReportedOn { get; set; }

        public int TaskId { get; set; }

        public virtual Task Task { get; set; }

        public int ReporterId { get; set; }

        public virtual User Reporter { get; set; }
    }
}
