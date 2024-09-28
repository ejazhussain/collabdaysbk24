using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using O365C.FuncApp.ProjectTracker.Models;
using O365C.FuncApp.ProjectTracker.Services;
using Newtonsoft.Json;
using Azure.Core;


namespace O365C.FuncApp.ProjectTracker
{
    public class ProjectManagement
    {
        private readonly ILogger<ProjectManagement> _logger;
        private readonly ISQLService _SQLService;

        public ProjectManagement(ILogger<ProjectManagement> logger, ISQLService SQLService)
        {
            _logger = logger;
            _SQLService = SQLService;
        }

        [Function("ProjectManagement")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("ProjectManagement function triggered.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                // Replace the incorrect line with the correct one                
                RequestDetail? request = JsonConvert.DeserializeObject<RequestDetail>(requestBody);
                if (request == null)
                {
                    return new BadRequestObjectResult("Invalid request body");
                }
                
                if (!string.IsNullOrEmpty(request.ProjectStatus))
                {

                 _logger.LogInformation("Fetching all projects with status: {0}", request.ProjectStatus);
                    var projects = await _SQLService.GetProjectsByStatusAsync<Project>("usp_GetProjectsByStatus", new { NewStatus = request.ProjectStatus });
                    //Log the number of projects returned
                    _logger.LogInformation("Number of projects returned: {0}", projects.Count());
                    return new OkObjectResult(projects);
                }
                else if (!string.IsNullOrEmpty(request.ProjectName))
                {
                    _logger.LogInformation("Fetching all tasks for project: {0}", request.ProjectName);
                    var tasks = await _SQLService.GetProjectTasksAsync<ProjectTask>("usp_GetProjectTasks", new { ProjectName = request.ProjectName });
                    //Log the number of tasks returned
                    _logger.LogInformation("Number of tasks returned: {0}", tasks.Count());
                    return new OkObjectResult(tasks);
                }               
                else if (!string.IsNullOrEmpty(request.TaskStatus) && !string.IsNullOrEmpty(request.TaskId))
                {
                    _logger.LogInformation("Updating task status for task ID: {0}", request.TaskId);
                    await _SQLService.UpdateTaskAsync("usp_UpdateTaskStatus", new { TaskID = request.TaskId, @NewStatusName = request.TaskStatus });                    
                    var task = await _SQLService.GetTaskByIdAsync<ProjectTask>("usp_GetTaskDetails", new { TaskID = request.TaskId });                   
                    
                    return new OkObjectResult(task);                    
                }
                else if (!string.IsNullOrEmpty(request.TaskId))
                {
                    _logger.LogInformation("Fetching task details for task ID: {0}", request.TaskId);
                    var task = await _SQLService.GetTaskByIdAsync<ProjectTask>("usp_GetTaskDetails", new { TaskID = request.TaskId });
                    //Log the number of tasks returned
                    _logger.LogInformation("Number of tasks returned: {0}", task.Count());
                    return new OkObjectResult(task);
                }
                else
                {
                    return new BadRequestObjectResult("Invalid request body");
                }
                //return new OkObjectResult("Welcome to Azure Functions!");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }          


        }
    }
}
