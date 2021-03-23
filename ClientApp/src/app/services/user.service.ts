import { Injectable } from '@angular/core';
import { AuthService } from "@auth0/auth0-angular";
import {
  catchError,
  map,
  pluck,
} from "rxjs/operators";
import { of } from 'rxjs';
import jwtDecode from "jwt-decode";

@Injectable({
  providedIn: "root",
})
export class UserService {
  roles: string[] = []

  constructor(private auth: AuthService) {
  }

  // This function is used to check if `app_metadata` property of user data
  // has property `https://vega.com/roles` which represent the current logged in user's
  // role, is `role` which is the value of the method argument.
  isInRole$(role: string) {
    return this.auth.getAccessTokenSilently().pipe(
      // Get user data object from the auth0 access token so we need to decode the jwt token first.
      map(accessToken => jwtDecode(accessToken)),
      pluck('https://vega.com/roles'),
      // If user object doesn't have `https://vega.com/roles` property or the property array doesn't contain
      // `role` value then emit false.
      map<string[], boolean>(roles => roles !== undefined && roles.indexOf(role) > -1),
      // If current user doesn't log in then `getAccessTokenSilently()` will throw error, so we need to catch the error status
      // then return a observable which emit false to represent that current user is not in logged in status so this
      // user is impossible to have any role.
      catchError(() => of(false))
    );
  }
}
