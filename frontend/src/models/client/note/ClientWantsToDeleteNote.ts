import {BaseDto} from "../../baseDto";

export class ClientWantsToDeleteNote extends BaseDto<ClientWantsToDeleteNote>{
  id?: number;
  subjectId?: number;
}
