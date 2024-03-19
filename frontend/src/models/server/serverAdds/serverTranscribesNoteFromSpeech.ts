import {BaseDto} from "../../baseDto";
import {Note} from "../../entities";

export class ServerTranscribesNoteFromSpeech extends BaseDto<ServerTranscribesNoteFromSpeech>{
  subjectId?: number;
  note?: Note;
}
