import {Injectable} from "@angular/core";
import {WebSocketSuperClass} from "./models/WebSocketSuperClass";
import {Note} from "./models/entities";

@Injectable({providedIn: 'root'})
export class WebSocketClientService {
  public subjectWithNotes: Map<number, Note[]> = new Map<number, Note[]>();
  public subjectWithConnections: Map<number, number> = new Map<number, number>();

}
