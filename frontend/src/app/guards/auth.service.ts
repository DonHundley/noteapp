import { Injectable } from '@angular/core';
import {CanActivate, Router} from "@angular/router";
import {WebSocketClientService} from "../../ws.client.service";
import {take} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthService implements CanActivate {


  constructor(
    private router: Router,
    private wsService: WebSocketClientService
  ) { }

  canActivate(): boolean {
    let isAuthenticated = false;

    // Take the latest value from the BehaviorSubject loginResult
    this.wsService.loginResult.pipe(take(1)).subscribe(isLoggedIn => {
      isAuthenticated = isLoggedIn;
    });

    if (isAuthenticated) {
      return true;
    }

    this.router.navigate(['/login']);
    return false;
  }

}
