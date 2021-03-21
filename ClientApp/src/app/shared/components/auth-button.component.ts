import { Component, Inject } from "@angular/core";
import { AuthService } from "@auth0/auth0-angular";
import { DOCUMENT } from "@angular/common";

@Component({
  selector: 'app-auth-button',
  template: `
    <ng-container *ngIf="auth.isAuthenticated$ | async; else loggedOut">
      <button type="button" class="btn btn-secondary" (click)="auth.logout({returnTo: document.location.origin })">
        Log out
      </button>
    </ng-container>

    <ng-template #loggedOut>
      <button type="button" class="btn btn-success" (click)="auth.loginWithRedirect()">Log in</button>
    </ng-template>
  `
})
export class AuthButtonComponent {
  constructor(
    public auth: AuthService,
    @Inject(DOCUMENT) public document
  ) {
  }
}
