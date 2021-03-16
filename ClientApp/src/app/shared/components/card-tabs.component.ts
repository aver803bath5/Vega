import { Component } from "@angular/core";
import { TabsComponent } from "./tabs.component";

@Component({
  selector: 'app-card-tabs',
  template: `
    <div class="card-header">
      <ul class="nav nav-tabs card-header-tabs" [ngClass]="navClass">
        <li *ngFor="let t of tabs" (click)="selectTab(t)" [class.active]="t.active" class="nav-item">
          <button class="btn btn-link nav-link" [class.active]="t.active">{{t.title}}</button>
        </li>
      </ul>
    </div>
    <ng-content></ng-content>
  `
})
export class CardTabsComponent extends TabsComponent {

}
