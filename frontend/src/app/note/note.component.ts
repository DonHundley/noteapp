import {Component, inject, OnInit} from '@angular/core';
import {ClientWantsToCreateNote} from "../../models/client/note/ClientWantsToCreateNote";
import {WebSocketClientService} from "../../ws.client.service";
import {FormControl} from "@angular/forms";
import {ActivatedRoute} from "@angular/router";
import {SubjectEnums} from "../../models/entities";
import {ClientWantsToSubscribeToSubject} from "../../models/client/subject/ClientWantsToSubscribeToSubject";

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss']
})
export class NoteComponent implements OnInit {
  ws = inject(WebSocketClientService);
  isLoading = true;  // loading status
  messageContent = new FormControl("");
  retries = [100, 250, 500, 1000, 10000, 30000];
  subjectId: number = Number.MAX_SAFE_INTEGER; // we know we do not have this many subjects.
  constructor(private route: ActivatedRoute) { }



  loadSubject(index: number = 1) {
    const subject  = this.ws.subjectWithNotes.get(this.subjectId);


    if (subject || index >= this.retries.length) {

      this.isLoading = false;
    }

    if (!subject && index < this.retries.length) {
      setTimeout(() => {
        this.loadSubject(index + 1);
      }, this.retries[index]);
    }

    if (index >= this.retries.length) {
      // Display an error message to the user
      this.ws.messageService.add({severity:'error', summary:'Error', detail:'Failed to load subject'});
    }
  }

  ngOnInit(): void {
    const subId = this.route.snapshot.paramMap.get('id');
    this.subjectId = Number(subId);

    if (this.subjectId !== Number.MAX_SAFE_INTEGER) { // if this didn't change we have a problem.
      this.ws.socketConnection.sendDto(new ClientWantsToSubscribeToSubject({subjectId: this.subjectId}));
      this.loadSubject();
    }else{
      this.ws.messageService.add({severity:'error', summary:'Error', detail:'Invalid subject ID'});
    }
  }

  publishNoteToSubject(key: number){
    this.ws.socketConnection.sendDto(
      new ClientWantsToCreateNote({messageContent: this.messageContent.value!, subjectId: key})
    )
    this.messageContent.reset();
  }

  dateFromStr(timestamp: string | undefined) {
    return new Date(timestamp!).toLocaleString();
  }

  protected readonly Number = Number;
  protected readonly SubjectEnums = SubjectEnums;
}
