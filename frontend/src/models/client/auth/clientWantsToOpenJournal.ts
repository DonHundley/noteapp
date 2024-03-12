import {BaseDto} from "../../baseDto";

export class ClientWantsToOpenJournal extends BaseDto<ClientWantsToOpenJournal>{
  username?: string;
  password?: string;
}
