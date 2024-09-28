import { AdaptiveCardInvokeResponse } from "botbuilder";

export const CreateActionErrorResponse = ( statusCode: number, errorCode: number = -1, errorMessage: string = 'Unknown error') => {
    return {
        statusCode: statusCode,
        type: 'application/vnd.microsoft.error',
        value: {
            error: {
                code: errorCode,
                message: errorMessage,
            },
        },
    };
  };

export const CreateAdaptiveCardInvokeResponse = (statusCode: number, body?: Record<string, unknown>): AdaptiveCardInvokeResponse => {
    return {
             statusCode: statusCode,
             type: 'application/vnd.microsoft.card.adaptive',
             value: body
         };
  };