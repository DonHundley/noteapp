import {Component, inject, OnInit} from "@angular/core";
import {WebSocketClientService} from "../../ws.client.service";
import {FormControl} from "@angular/forms";
import {ActivatedRoute} from "@angular/router";
import {ClientWantsToSubscribeToSubject} from "../../models/client/subject/ClientWantsToSubscribeToSubject";
import {ClientWantsToCreateNote} from "../../models/client/note/ClientWantsToCreateNote";
import {Note, SubjectEnums} from "../../models/entities";
import {ClientWantsToDeleteNote} from "../../models/client/note/ClientWantsToDeleteNote";
import {ClientWantsToEditNote} from "../../models/client/note/ClientWantsToEditNote";
import {ClientWantsToSpeak} from "../../models/client/speech/ClientWantsToSpeak";


@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss']
})
export class NoteComponent implements OnInit {
  protected readonly SubjectEnums = SubjectEnums;
  ws = inject(WebSocketClientService);

  // Loading options
  isLoading = true;  // loading status
  retries = [100, 250, 500, 1000, 10000, 30000];

  // subject and note variables
  subjectId: number = Number.MAX_SAFE_INTEGER;  // we know we do not have this many subjects.
  messageContent = new FormControl("");
  notesReversed = false;
  selectedNote: Note | undefined;

  // recording options
  isRecording = false;
  recorder: MediaRecorder | null = null;
  chunks: Blob[] = [];
  audio: Blob | null = null;

  // Quill options
  toolbarOptions = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'],
      ['blockquote', 'code-block'],
      [{ 'header': 1 }, { 'header': 2 }],
      [{ 'list': 'ordered'}, { 'list': 'bullet' }],
      [{ 'script': 'sub'}, { 'script': 'super' }],
      [{ 'indent': '-1'}, { 'indent': '+1' }],
      [{ 'direction': 'rtl' }],
      [{ 'size': ['small', false, 'large', 'huge'] }],
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      [{ 'color': [] }, { 'background': [] }],
      [{ 'font': [] }],
      [{ 'align': [] }],
      ['clean']
    ]};

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
      this.ws.messageService.add({severity:'error', summary:'Error', detail:'Failed to load subject'});
    }
  }

  ngOnInit(): void {
    const subId = this.route.snapshot.paramMap.get('id');
    this.subjectId = Number(subId);
    if (this.subjectId !== Number.MAX_SAFE_INTEGER) {
      this.ws.socketConnection.sendDto(new ClientWantsToSubscribeToSubject({subjectId: this.subjectId}));
      this.loadSubject();
    } else {
      this.ws.messageService.add({severity:'error', summary:'Error', detail:'Invalid subject ID'});
    }
  }

  publishNoteToSubject(key: number){
    this.ws.socketConnection.sendDto(
      new ClientWantsToCreateNote({messageContent: this.messageContent.value!, subjectId: key})
    )
    this.messageContent.reset();
  }

  handleNoteOrder() {
    const subject = this.ws.subjectWithNotes.get(this.subjectId);
    if (subject) {
      this.ws.subjectWithNotes.set(this.subjectId, subject.reverse());
      this.notesReversed = !this.notesReversed;
    }
  }

  dateFromStr(timestamp: string | undefined) {
    return new Date(timestamp!).toLocaleString();
  }

  selectNote(note: Note) {
    this.selectedNote = note;
    if (note.noteContent) {
      this.messageContent.setValue(note.noteContent);
    }
  }

  deleteNoteFromSubject(){
    if (this.selectedNote){
      this.ws.socketConnection.sendDto(new ClientWantsToDeleteNote({id: this.selectedNote.id, subjectId: this.selectedNote.subjectId}))
      this.resetNote();
    }

  }

  onNewOrUpdateClick(): void {
    if (this.selectedNote) {
      this.resetNote();
    } else {
      this.publishNoteToSubject(this.subjectId);
    }
  }

  updateNoteInSubject(){
    if (this.selectedNote) {
      this.ws.socketConnection.sendDto(new ClientWantsToEditNote({id: this.selectedNote.id, subjectId: this.selectedNote.subjectId, messageContent: this.messageContent.value!}))
    }
  }

  resetNote() {
    this.selectedNote = undefined;
    this.messageContent.reset();
  }

  async setupAndStartRecording(): Promise<void> {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      this.recorder = new MediaRecorder(stream, {mimeType: 'audio/ogg;codecs=opus'});

      if (!MediaRecorder.isTypeSupported('audio/ogg; codecs=opus')) {
        this.ws.messageService.add({severity:'error', summary:'Error', detail:'Browser does not support audio format.'});
      }

      // Event triggered when data is available
      this.recorder.ondataavailable = e => {
        this.chunks.push(e.data);
      }


      // Event triggered when recording stops
      this.recorder.onstop = async () => {
        this.audio = new Blob(this.chunks, { 'type' : 'audio/ogg; codecs=opus' });
        this.chunks = [];

        console.log(this.chunks);

        const reader = new FileReader();

        // Wrap the reader in a Promise to await it
        const loadPromise = new Promise((resolve, reject) => {
          reader.onload = resolve;
          reader.onerror = reject;
        });

        reader.readAsDataURL(this.audio);

        // Wait for the FileReader to finish
        await loadPromise;

        let base64Audio = reader.result as string;
        base64Audio = base64Audio.split(",")[1];


        this.ws.socketConnection.sendDto(new ClientWantsToSpeak({AudioData: base64Audio, SubjectId: this.subjectId}));
      };

      this.recorder.start(20);
      this.isRecording = true;

    } catch (error) {
      this.ws.messageService.add({severity:'error', summary:'Error', detail:'There was a problem with recording.'});
    }
  }

  stopRecording(): void {
    if (this.recorder) {
      this.recorder.stop();
      this.isRecording = false;
    }
  }

  startStopRecording(): void {
    if (this.isRecording) {
      this.stopRecording();
    } else {
      this.setupAndStartRecording();
    }
  }
}
