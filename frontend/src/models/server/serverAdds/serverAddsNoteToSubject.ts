import {BaseDto} from "../../baseDto";
import {Note} from "../../entities";

export class ServerAddsNoteToSubject extends BaseDto<ServerAddsNoteToSubject>{
  subjectId?: number;
  note?: Note;
}
