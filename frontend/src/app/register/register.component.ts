import {Component, OnDestroy, OnInit} from "@angular/core";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Subscription} from "rxjs";
import {WebSocketClientService} from "../../ws.client.service";
import {Router} from "@angular/router";
import {ClientWantsToJournal} from "../../models/client/auth/clientWantsToJournal";

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit, OnDestroy {
  registrationForm: FormGroup = null!;
  registerResultSub: Subscription = null!;
  ws: WebSocketClientService;

  constructor(private formBuilder: FormBuilder, private router: Router, private webSocketClientService: WebSocketClientService) {
    this.ws = this.webSocketClientService;
  }

  ngOnInit() {
    this.initRegistrationForm();

    this.registerResultSub = this.ws.loginResult.subscribe(success => {
      if (success) {
        this.router.navigate(['/subject']);
      }
    });
  }

  initRegistrationForm() {
    this.registrationForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  register() {
    if (this.registrationForm.valid) {
      this.ws.socketConnection.sendDto(new ClientWantsToJournal(this.registrationForm.value));
    }
  }

  ngOnDestroy() {
    if (this.registerResultSub) {
      this.registerResultSub.unsubscribe();
    }
  }
}
