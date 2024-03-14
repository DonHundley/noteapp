import {BaseDto} from "../../baseDto";
import {Note} from "../../entities";

export class ServerUpdatesNoteInSubject extends BaseDto<ServerUpdatesNoteInSubject>{
  subjectId?: number;
  note?: Note;
}
