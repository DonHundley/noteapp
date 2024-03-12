import {BaseDto} from "../../baseDto";

export class ServerSendsErrorMessageToClient extends BaseDto<ServerSendsErrorMessageToClient>{
  message?: string;
}
