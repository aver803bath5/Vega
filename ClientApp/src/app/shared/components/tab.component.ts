import { Component, Input } from "@angular/core";

@Component({
  selector: 'app-tab',
  template: `
    <div [hidden]="!active">
      <ng-content></ng-content>
    </div>
  `
})
export class TabComponent {
  @Input('tab-title') title = "";
  @Input() active = false;
}
