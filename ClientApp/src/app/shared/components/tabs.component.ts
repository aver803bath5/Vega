import { AfterContentInit, Component, ContentChildren, Input, QueryList} from "@angular/core";
import { TabComponent } from "./tab.component";

@Component({
  selector: 'app-tabs',
  template: `
    <ul class="nav nav-tabs" [ngClass]="navClass">
      <li *ngFor="let t of tabs" (click)="selectTab(t)" [class.active]="t.active">
        <button type="button" class="btn btn-link" [class.active]="t.active">{{t.title}}</button>
      </li>
    </ul>
    <ng-content></ng-content>
  `
})
export class TabsComponent implements AfterContentInit {
  @ContentChildren(TabComponent) tabs: QueryList<TabComponent>;
  @Input('nav-class') navClass

  ngAfterContentInit() {
    let activeTabs = this.tabs.filter(t => t.active);

    // If there are no tabs are active, make the first tab active.
    if (activeTabs.length === 0)
      this.selectTab(this.tabs.first);
  }

  // After deactivating all tabs, activate the selected tab
  selectTab(tab: TabComponent) {
    this.tabs.forEach(t => t.active = false);

    tab.active = true;
  }
}
