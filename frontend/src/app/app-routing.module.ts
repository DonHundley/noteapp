import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { SubjectComponent } from './subject/subject.component';
import {RegistrationComponent} from "./register/register.component";
import {NoteComponent} from "./note/note.component";
import {AuthService} from "./guards/auth.service";

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'subject', component: SubjectComponent, canActivate: [AuthService] },
  { path: 'notes/:id', component: NoteComponent, canActivate: [AuthService]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
