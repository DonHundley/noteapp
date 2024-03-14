import {Component, inject, OnInit} from "@angular/core";
import {WebSocketClientService} from "../../ws.client.service";
import {SubjectEnums} from "../../models/entities";
import {ClientWantsToSubscribeToSubject} from "../../models/client/subject/ClientWantsToSubscribeToSubject";


@Component({
  selector: 'app-subject',
  templateUrl: './subject.component.html',
  styleUrls: ['./subject.component.scss']
})
export class SubjectComponent {
  ws = inject(WebSocketClientService);

  subjects = Object.entries(SubjectEnums).filter(e => !isNaN(Number(e[0])));

  subscribeToSubject(key: number){
    this.ws.socketConnection.sendDto(
      new ClientWantsToSubscribeToSubject({subjectId: key})
    )
  }
}
