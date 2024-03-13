import {BaseDto} from "../../baseDto";

export class ClientWantsToCreateNote extends BaseDto<ClientWantsToCreateNote>{
  messageContent?: string;
  subjectId?: number;
}
