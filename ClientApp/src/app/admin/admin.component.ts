import { Component, OnInit } from '@angular/core';
import { AuthService } from "@auth0/auth0-angular";
import { UserService } from "../services/user.service";

@Component({
  selector: 'app-dashboard',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  isAdmin = false

  constructor(
    public auth: AuthService,
    public user: UserService
  ) {
  }

  ngOnInit(): void {
  }

}
