import {Injectable} from "@angular/core";
import {WebSocketSuperClass} from "./models/WebSocketSuperClass";
import {Note} from "./models/entities";
import {MessageService} from "primeng/api";
import {environment} from "./environments/environment";
import {BaseDto} from "./models/baseDto";
import {ServerAuthenticatesJournalist} from "./models/server/auth/serverAuthenticatesJournalist";
import {ServerSendsErrorMessageToClient} from "./models/server/error/serverSendsErrorMessageToClient";
import {ServerSubscribesClientToSubject} from "./models/server/serverAdds/serverSubscribesClientTosubject";
import {ServerAddsNoteToSubject} from "./models/server/serverAdds/serverAddsNoteToSubject";
import {BehaviorSubject} from "rxjs";
import {Router} from "@angular/router";
import {ServerUpdatesNoteInSubject} from "./models/server/serverAdds/ServerUpdatesNoteInSubject";
import {ServerDeletesNoteInSubject} from "./models/server/serverAdds/ServerDeletesNoteInSubject";

@Injectable({providedIn: 'root'})
export class WebSocketClientService {
  public subjectWithNotes: Map<number, Note[]> = new Map<number, Note[]>();
  public subjectWithConnections: Map<number, number> = new Map<number, number>();

  public socketConnection: WebSocketSuperClass;

  loginResult = new BehaviorSubject<boolean>(false);

  constructor(public messageService: MessageService, private router: Router){
    this.socketConnection = new WebSocketSuperClass(environment.websocketBaseUrl);
    this.handleEventsEmittedByServer()
  }

  handleEventsEmittedByServer(){
    this.socketConnection.onmessage = (event) => {
      const data = JSON.parse(event.data) as BaseDto<any>;
      console.log("Received: " + JSON.stringify(data));
      //@ts-ignore
      this[data.eventType].call(this, data);
    }
    this.socketConnection.onerror = (err) => {
      this.messageService.add({life: 5000, severity: 'error', detail:'Websocket API is not running.'})
    }
  }


  ServerAuthenticatesJournalist(dto: ServerAuthenticatesJournalist){
    this.messageService.add({life: 2000, detail: 'Authentication successful'});
    localStorage.setItem("jwt", dto.token!);
    this.loginResult.next(true);
  }

  ServerAuthenticatesJournalistJwt(dto: ServerAuthenticatesJournalist){
    this.messageService.add({life: 2000, detail: 'Authentication successful'});
  }

  ServerSendsErrorMessageToClient(dto: ServerSendsErrorMessageToClient){
    this.messageService.add({life: 5000, severity: 'error', detail: dto.message});
    if(dto.message?.includes('"errorMessage":"Not Authorized"')){
      localStorage.removeItem("jwt"); // remove the token as it's no longer valid
      this.loginResult.next(false);   // set login status to false
      this.router.navigate(['/login']);
    }
  }

  ServerSubscribesClientToSubject(dto: ServerSubscribesClientToSubject){
    this.subjectWithNotes.set(dto.subjectId!, dto.notes!.reverse());
    this.subjectWithConnections.set(dto.subjectId!, dto.connections!);
  }

  ServerAddsNoteToSubject(dto: ServerAddsNoteToSubject){
    this.subjectWithNotes.get(dto.subjectId!)!.push(dto.note!);
    this.messageService.add({life: 2000, detail: "New Note!"});
  }

  ServerUpdatesNoteInSubject(dto: ServerUpdatesNoteInSubject){
    const subjectNotes = this.subjectWithNotes.get(dto.subjectId!);

    console.log(dto.note?.noteContent);
    if (subjectNotes) {
      const noteIndex = subjectNotes.findIndex(note => note.id === dto.note?.id);
      if (noteIndex !== -1) {
        console.log(subjectNotes[noteIndex]);
        subjectNotes[noteIndex] = dto.note!;
        console.log(subjectNotes[noteIndex]);
        this.messageService.add({life: 2000, detail: "Updated Note!"});
      } else {
        this.messageService.add({life: 2000, detail: "Note update failed"});
      }
    }else {
      this.messageService.add({life: 2000, detail: "Note update failed"});
    }
  }

  ServerDeletesNoteInSubject(dto: ServerDeletesNoteInSubject){
    const subjectNotes = this.subjectWithNotes.get(dto.subjectId!);
    if (subjectNotes) {
      const noteIndex = subjectNotes.findIndex(note => note.id === dto.id);
      if (noteIndex !== -1) {
        subjectNotes.splice(noteIndex, 1);
        this.messageService.add({life: 2000, detail: "Deleted Note!"});
      } else {
        this.messageService.add({life: 2000, detail: "Note deletion failed"});
      }
    } else {
      this.messageService.add({life: 2000, detail: "Note deletion failed"});
    }
  }
}
