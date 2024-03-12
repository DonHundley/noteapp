import {ToastModule} from "primeng/toast";
import {MessageModule} from "primeng/message";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {CommonModule} from "@angular/common";
import {platformBrowserDynamic} from "@angular/platform-browser-dynamic";
import {MessageService} from "primeng/api";
import {NgModule} from "@angular/core";
import {BrowserModule} from "@angular/platform-browser";
import {AppComponent} from "./app/app.component";


@NgModule({
  imports: [
    BrowserModule,
    CommonModule,
    ToastModule,
    MessageModule,
    ReactiveFormsModule,
    FormsModule,


  ],
  declarations: [AppComponent
  ],
  providers: [MessageService],
  bootstrap: [AppComponent]
})
export class AppModule {
}


platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.log(err));
