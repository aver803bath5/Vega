import { Component, OnInit } from '@angular/core';
import { AuthService } from "@auth0/auth0-angular";
import { UserService } from "../services/user.service";


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  isAdmin = false;

  constructor(public auth: AuthService, public user: UserService) {
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  ngOnInit(): void {
    // TODO: 在 template 中以 async pipe 使用 user.service 中的 isRoot$ 回傳的 observable
    // 原本想要在 template 裡面使用 pipe 來檢查登入的使用者的 role 是否為 admin 但是不知道為什麼在 template 中加上 {{user.isInRole$ | async }} 時
    // this.isInRole$ 會在每次 view change 的時候都被執行一次，然後 this.user.isInRole$ 裡面如果是 return this.auth.isAuthenticated$ 的話，
    // view 中 的 {{user.isInRole$ |async }} 就會讓 this.user.isInRole$ 這個 method 被執行無限次。會造成整個 view 卡死。
    this.user.isInRole$('Admin').subscribe(isAdmin => this.isAdmin = isAdmin);
  }
}
