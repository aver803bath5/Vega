import { Injectable } from '@angular/core';
import { AuthService } from "@auth0/auth0-angular";
import { UserService } from "./user.service";
import { CanActivate, Router } from "@angular/router";
import { tap } from "rxjs/operators";

// This service is used to protect the admin page from user who directly navigates(ng: type the admin page URL to the browser) to admin page.
@Injectable({
  providedIn: "root"
})
export class AdminAuthGuardService implements CanActivate {

  constructor(private auth: AuthService, private user: UserService, private router: Router) {
  }

  canActivate() {
    // Check if current user role is Admin. If not navigate the user to the home page.
    return this.user.isInRole$('Admin').pipe(tap(isAdmin => !isAdmin && this.router.navigate(['/vehicles'])));
  }
}
