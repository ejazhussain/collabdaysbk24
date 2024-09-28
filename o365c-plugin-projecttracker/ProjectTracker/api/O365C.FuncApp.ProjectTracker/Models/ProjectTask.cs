using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O365C.FuncApp.ProjectTracker.Models
{
    public class ProjectTask
    {
        public string ProjectName { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string TaskStatus { get; set; }
        public string Email { get; set; }      
    }
}
