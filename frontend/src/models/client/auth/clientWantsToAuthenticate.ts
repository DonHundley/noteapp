import {BaseDto} from "../../baseDto";

export class ClientWantsToAuthenticate extends BaseDto<ClientWantsToAuthenticate>{
  jwt?: string;
}
