import {BaseDto} from "../../baseDto";

export class ClientWantsToSpeak extends BaseDto<ClientWantsToSpeak> {
  AudioData?: string;
  SubjectId?: number;
}
