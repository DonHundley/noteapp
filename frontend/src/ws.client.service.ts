import {EventEmitter, Injectable} from "@angular/core";
import {WebSocketSuperClass} from "./models/WebSocketSuperClass";
import {Note} from "./models/entities";
import {MessageService} from "primeng/api";
import {environment} from "./environments/environment";
import {BaseDto} from "./models/baseDto";
import {ServerAuthenticatesJournalist} from "./models/server/auth/serverAuthenticatesJournalist";
import {ServerSendsErrorMessageToClient} from "./models/server/error/serverSendsErrorMessageToClient";

@Injectable({providedIn: 'root'})
export class WebSocketClientService {
  public subjectWithNotes: Map<number, Note[]> = new Map<number, Note[]>();
  public subjectWithConnections: Map<number, number> = new Map<number, number>();

  public socketConnection: WebSocketSuperClass;

  constructor(public messageService: MessageService){
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
  }

  ServerAuthenticatesJournalistJwt(dto: ServerAuthenticatesJournalist){
    this.messageService.add({life: 2000, detail: 'Authentication successful'});
  }

  ServerSendsErrorMessageToClient(dto: ServerSendsErrorMessageToClient){
    this.messageService.add({life: 5000, severity: 'error', detail: dto.message});
  }

}
