import {Component, inject} from '@angular/core';
import {WebSocketClientService} from "../ws.client.service";
import {FormControl} from "@angular/forms";
import {ClientWantsToOpenJournal} from "../models/client/auth/clientWantsToOpenJournal";
import {ClientWantsToJournal} from "../models/client/auth/clientWantsToJournal";

@Component({
  selector: 'app-root',
  templateUrl: `
    <p-toast></p-toast>
    <h1>Authenticate</h1>
    <input type="email" [formControl]="username" placeholder="username">
    <input type="password" [formControl]="password" placeholder="password">
    <button (click)="SignIn()">Sign in</button>
    <button (click)="Register()">Register</button>


<app-image-detection></app-image-detection>

  `,
})
export class AppComponent {
  ws = inject(WebSocketClientService);
  username = new FormControl("");
  password = new FormControl("");


  SignIn(){
    this.ws.socketConnection.sendDto(new ClientWantsToOpenJournal({username: this.username.value!, password: this.password.value!}))
  }

  Register(){
    this.ws.socketConnection.sendDto(new ClientWantsToJournal({username: this.username.value!, password: this.password.value!}))
  }


}
