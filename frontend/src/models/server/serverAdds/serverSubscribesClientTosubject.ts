import {BaseDto} from "../../baseDto";
import {Note} from "../../entities";

export class ServerSubscribesClientTosubject extends BaseDto<ServerSubscribesClientTosubject>{
  subjectId?: number;
  connections?: number;
  notes?: Note[];
}
