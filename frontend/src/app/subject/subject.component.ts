import {Component, inject, OnInit} from "@angular/core";
import {WebSocketClientService} from "../../ws.client.service";
import {FormControl} from "@angular/forms";
import {SubjectEnums} from "../../models/entities";
import {ClientWantsToSubscribeToSubject} from "../../models/client/subject/ClientWantsToSubscribeToSubject";
import {ClientWantsToCreateNote} from "../../models/client/note/ClientWantsToCreateNote";

@Component({
  selector: 'app-subject',
  templateUrl: './subject.component.html',
  styleUrls: ['./subject.component.scss']
})
export class SubjectComponent implements OnInit {
  ws = inject(WebSocketClientService);

  messageContent = new FormControl("");
  subjects = Object.entries(SubjectEnums).filter(e => !isNaN(Number(e[0])));

  ngOnInit(): void {
    // Subscribe to first subjectId on init
    this.subscribeToSubject(Number(this.subjects[0][0]));
  }


  subscribeToSubject(key: number){
    this.ws.socketConnection.sendDto(
      new ClientWantsToSubscribeToSubject({subjectId: key})
    )
  }

  publishNoteToSubject(key: number){
    this.ws.socketConnection.sendDto(
      new ClientWantsToCreateNote({messageContent: this.messageContent.value!, subjectId: key})
    )
    this.messageContent.reset();  // clear input after sending
  }

  dateFromStr(timestamp: string | undefined) {
    return new Date(timestamp!).toLocaleString();
  }

  protected readonly Number = Number;
}
