import {Component, inject, OnDestroy, OnInit} from '@angular/core';


import {WebSocketClientService} from "../../ws.client.service";
import {ClientWantsToOpenJournal} from "../../models/client/auth/clientWantsToOpenJournal";
import {Router} from "@angular/router";
import {Subscription} from "rxjs";
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup = null!;
  loginResultSub: Subscription = null!;
  ws: WebSocketClientService;

  constructor(private formBuilder: FormBuilder, private router: Router, private webSocketClientService: WebSocketClientService) {
    this.ws = this.webSocketClientService;
  }

  ngOnInit() {
    this.initLoginForm();

    this.loginResultSub = this.ws.loginResult.subscribe(success => {
      if (success) {
        this.router.navigate(['/subject']);
      }
    });
  }

  initLoginForm() {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  signIn() {
    if (this.loginForm.valid) {
      this.ws.socketConnection.sendDto(new ClientWantsToOpenJournal(this.loginForm.value));
    }
  }

  ngOnDestroy() {
    if (this.loginResultSub) { // add check in case subscription doesn't exist
      this.loginResultSub.unsubscribe();  // unsubscribe when the component is destroyed
    }
  }
}
