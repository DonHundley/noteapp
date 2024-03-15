import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {platformBrowserDynamic} from "@angular/platform-browser-dynamic";
import {ToastModule} from "primeng/toast";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {QuillModule} from "ngx-quill";
import {MessageService} from "primeng/api";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

// Components
import {LoginComponent} from "./app/login/login.component";
import {AppComponent} from "./app/app.component";
import {RegistrationComponent} from "./app/register/register.component";
import {SubjectComponent} from "./app/subject/subject.component";
import {NoteComponent} from "./app/note/note.component";

// Routing
import {AppRoutingModule} from "./app/app-routing.module";

// Pipes
import {RemoveHtmlTagsPipe} from "./app/pipes/remove-html-tags.pipe";



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegistrationComponent,
    SubjectComponent,
    NoteComponent,
    RemoveHtmlTagsPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    ToastModule,
    BrowserAnimationsModule,
    QuillModule.forRoot(),
    FormsModule
  ],
  providers: [MessageService],
  bootstrap: [AppComponent]
})
export class AppModule { }


platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.log(err));
