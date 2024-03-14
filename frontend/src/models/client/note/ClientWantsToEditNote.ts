import {BaseDto} from "../../baseDto";

export class ClientWantsToEditNote extends BaseDto<ClientWantsToEditNote>{
  messageContent?: string;
  subjectId?: number;
  id?: number;
}
