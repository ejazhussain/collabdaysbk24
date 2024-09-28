using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O365C.FuncApp.ProjectTracker.Models
{
    public class RequestDetail
    {
        public string ProjectStatus { get; set; }
        public string ProjectName { get; set; }
        public string TaskId { get; set; }
        public string TaskStatus { get; set; }
    }
}
