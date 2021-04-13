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
      <button type="button" class="btn btn-success" (click)="auth.loginWithRedirect()"
              [disabled]="auth.isLoading$ | async">
        <ng-container *ngIf="auth.isLoading$ | async; else loginText">
          Loading...
        </ng-container>
        <ng-template #loginText>
          Log in
        </ng-template>
      </button>
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
