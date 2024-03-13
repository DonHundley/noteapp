import {BaseDto} from "../../baseDto";
import {Note} from "../../entities";

export class ServerSubscribesClientToSubject extends BaseDto<ServerSubscribesClientToSubject>{
  subjectId?: number;
  connections?: number;
  notes?: Note[];
}
