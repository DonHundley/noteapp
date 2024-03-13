import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {platformBrowserDynamic} from "@angular/platform-browser-dynamic";

// Components
import {LoginComponent} from "./app/login/login.component";
import {AppComponent} from "./app/app.component";
import {RegistrationComponent} from "./app/register/register.component";
import {SubjectComponent} from "./app/subject/subject.component";


// Routing
import {AppRoutingModule} from "./app/app-routing.module";
import {ReactiveFormsModule} from "@angular/forms";
import {MessageService} from "primeng/api";



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegistrationComponent,
    SubjectComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule
  ],
  providers: [MessageService],
  bootstrap: [AppComponent]
})
export class AppModule { }


platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.log(err));
