import { default as axios } from "axios";
import * as querystring from "querystring";
import {
  TeamsActivityHandler,
  CardFactory,
  TurnContext,
  MessagingExtensionQuery,
  MessagingExtensionResponse,
  AdaptiveCardInvokeResponse,
  AdaptiveCardInvokeValue,
} from "botbuilder";
import * as ACData from "adaptivecards-templating";
import ProjectCard from "./adaptiveCards/projectCard.json";
import SuccessCard from "./adaptiveCards/successCard.json";
import EditTasksCard from "./adaptiveCards/editTaskCard.json";
import config from "./config";
import { Project } from "./models/Project";
import { ProjectTask } from "./models/ProjectTask";
import { CreateActionErrorResponse, CreateAdaptiveCardInvokeResponse } from "./adaptiveCards/utils";

export class SearchApp extends TeamsActivityHandler {
  constructor() {
    super();
  }

  // Search.
  public async handleTeamsMessagingExtensionQuery(
    context: TurnContext,
    query: MessagingExtensionQuery
  ): Promise<MessagingExtensionResponse> {
    const searchQuery = query.parameters[0].value;

    var projectStatus = this.getQueryData(query, "getProjectStatus");
    var projectName = this.getQueryData(query, "getProjectTasks");
    var taskId = this.getQueryData(query, "getProjectTaskDetail");
    var taskStatus = this.getQueryData(query, "getUpdateTaskStatus");

    var payload = {
      projectStatus: projectStatus,
      projectName: projectName,
      taskId: taskId,
      taskStatus: taskStatus
    }

    
    let attachments = [];
    try {
      const response = await axios.post(config.functionAppUrl, payload);
      console.log(response.data);
      if (projectStatus) {
        attachments = [];
        response.data.forEach((obj) => {
          const template = new ACData.Template(ProjectCard);
          const card = template.expand({
            $root: {
              name: obj.projectName,              
              description: obj.description,
              startDate: obj.startDate,
              endDate: obj.endDate,
              manager: obj.projectManager,              
              status: obj.projectStatus
            },
          });
          const preview = CardFactory.heroCard(obj.projectName);
          const attachment = { ...CardFactory.adaptiveCard(card), preview };
          attachments.push(attachment);
        });

      } else if (taskId || projectName) {
        attachments = [];
        response.data.forEach((obj) => {
          const template = new ACData.Template(EditTasksCard);
          const card = template.expand({
            $root: {
              projectName: obj.projectName,
              taskName: obj.taskName,
              taskId: obj.taskId,
              description: obj.description,
              taskStatus: obj.taskStatus,
              dueDate: obj.dueDate,
              email: obj.email,

            } as ProjectTask,
          });
          const preview = CardFactory.heroCard(obj.taskName);
          const attachment = { ...CardFactory.adaptiveCard(card), preview };
          attachments.push(attachment);
        });
      }
      return {
        composeExtension: {
          type: "result",
          attachmentLayout: "list",
          attachments: attachments,
        },
      };
    } catch (e) {
      console.log(e);
    }

    // const response1 = await axios.get(
    //   `http://registry.npmjs.com/-/v1/search?${querystring.stringify({
    //     text: searchQuery,
    //     size: 8,
    //   })}`
    // );


    // response1.data.objects.forEach((obj) => {
    //   const template = new ACData.Template(helloWorldCard);
    //   const card = template.expand({
    //     $root: {
    //       name: obj.package.name,
    //       description: obj.package.description,
    //     },
    //   });
    //   const preview = CardFactory.heroCard(obj.package.name);
    //   const attachment = { ...CardFactory.adaptiveCard(card), preview };
    //   attachments.push(attachment);
    // });

  }

  public async onAdaptiveCardInvoke(context: TurnContext): Promise<AdaptiveCardInvokeResponse>  {
      
    try
    {
      switch (context.activity.value.action.verb) {
        case 'ok': {
          return this.handleTeamsCardActionUpdateTaskStatus(context);
        }       
        case 'cancel': {
          return this.handleTeamsCardActionCancel(context);
        }       
        default: {
          console.log ('Unknown Invoke activity received');
          return CreateActionErrorResponse(400, 0, `ActionVerbNotSupported: ${context.activity.value.action.verb} is not a supported action verb.`);
        }
      }

    }
    catch (err) {
      return CreateActionErrorResponse(500, 0, err.message);
    }
  }

  private async handleTeamsCardActionUpdateTaskStatus(context: TurnContext): Promise<AdaptiveCardInvokeResponse> {
    const request = context.activity.value;
    const data = request.action.data;
    console.log(`🎬 Handling update task status action, Updated Status=${data.choiceTaskStatus}`);

    // Update the task status in the database
    if(data.taskId && data.choiceTaskStatus) {
      // Update the task status in the database
      var payload = {
        projectStatus: "",
        projectName: "",
        taskId: data.taskId,
        taskStatus: data.choiceTaskStatus        
      }
      const response = await axios.post(config.functionAppUrl, payload);
      const [updatedTask] =  response.data;      
      var template = new ACData.Template(SuccessCard);
      var card = template.expand({
        $root: {
          projectName: updatedTask.projectName,
          taskId: updatedTask.taskId,
          taskName: updatedTask.taskName,
          description: updatedTask.description,
          taskStatus: updatedTask.taskStatus,
          dueDate: updatedTask.dueDate,
          email: updatedTask.email,
          // Card message
          message: `Task status updated to ${updatedTask.taskStatus}!`
        }
      });      
      return CreateAdaptiveCardInvokeResponse(200, card );
    }
  }
  private async handleTeamsCardActionCancel(context: TurnContext): Promise<AdaptiveCardInvokeResponse> {
    const request = context.activity.value;
    const data = request.action.data;
    console.log(`🎬 Handling cancel task status action`);

    // Update the task status in the database
    if(data.taskId) {
      // Update the task status in the database
      var payload = {
        projectStatus: "",
        projectName: "",
        taskId: data.taskId,
        taskStatus: ""        
      }
      const response = await axios.post(config.functionAppUrl, payload);
      const [task] =  response.data;
      var template = new ACData.Template(SuccessCard);
      var card = template.expand({
        $root: {
          projectName: task.projectName,
          taskId: task.taskId,
          taskName: task.taskName,
          description: task.description,
          taskStatus: task.taskStatus,
          dueDate: task.dueDate,
          email: task.email,
          // Card message                
          message: `Task status update cancelled for ${task.taskName}.`
        }
      });      
      return CreateAdaptiveCardInvokeResponse(200, card );
    }
  }

  private getQueryData(query: MessagingExtensionQuery, key: string): string {
    if (!query?.parameters?.length) {
      return '';
    }

    // Use Array.prototype.find to find the KeyValuePair with the specified key
    const foundPair = query.parameters.find(pair => pair.name === key);

    return foundPair?.value?.toString() ?? '';
  }
}
