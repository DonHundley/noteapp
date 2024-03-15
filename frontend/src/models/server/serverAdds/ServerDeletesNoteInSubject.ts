import {BaseDto} from "../../baseDto";

export class ServerDeletesNoteInSubject extends BaseDto<ServerDeletesNoteInSubject>{
  id?: number;
  subjectId?: number;
}
