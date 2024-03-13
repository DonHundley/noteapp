import {Component, inject} from '@angular/core';
import {WebSocketClientService} from "../ws.client.service";
import {FormControl} from "@angular/forms";
import {ClientWantsToOpenJournal} from "../models/client/auth/clientWantsToOpenJournal";
import {ClientWantsToJournal} from "../models/client/auth/clientWantsToJournal";
import {ClientWantsToCreateNote} from "../models/client/note/ClientWantsToCreateNote";
import {ClientWantsToSubscribeToSubject} from "../models/client/subject/ClientWantsToSubscribeToSubject";

@Component({
  selector: 'app-root',
  template:  `
      <h1>Authenticate</h1>
      <input type="email" [formControl]="username" placeholder="username">
      <input type="password" [formControl]="password" placeholder="password">
      <button (click)="signIn()">Sign in</button>
      <button (click)="register()">Register</button>


      <h1>Subscribe to subject</h1>
      <input type="number" [formControl]="subjectId">
      <button (click)="subscribeToSubject()">Subscribe to subject</button>


      <div *ngFor="let n of ws.subjectWithNotes | keyvalue">
          Number of connections: {{ws.subjectWithConnections.get(n.key)!-1}}
          <h2>{{n.key}}</h2>
          <div *ngFor="let note of n.value">
              {{note.noteContent}} at {{dateFromStr(note.timestamp)}}
          </div>
          <input [formControl]="messageContent"><button (click)="publishNoteToSubject(n.key)">Send message</button>
      </div>
  `,
})
export class AppComponent {
  ws = inject(WebSocketClientService);
  username = new FormControl("");
  password = new FormControl("");
  messageContent = new FormControl("");
  subjectId = new FormControl<number>(0);

  signIn(){
    this.ws.socketConnection.sendDto(
      new ClientWantsToOpenJournal({username: this.username.value!, password: this.password.value!}))
  }

  register(){
    this.ws.socketConnection.sendDto(
      new ClientWantsToJournal({username: this.username.value!, password: this.password.value!}))
  }

  dateFromStr(timestamp: string | undefined) {
    return new Date(timestamp!).toLocaleString();
  }

  publishNoteToSubject(key: number){
    this.ws.socketConnection.sendDto(
      new ClientWantsToCreateNote({messageContent: this.messageContent.value!, subjectId: key})
    )
  }

  subscribeToSubject(){
    this.ws.socketConnection.sendDto(
      new ClientWantsToSubscribeToSubject({subjectId: this.subjectId.value!})
    )
  }


}
