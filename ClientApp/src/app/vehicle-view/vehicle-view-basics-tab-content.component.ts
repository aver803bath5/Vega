import { Component, Input } from "@angular/core";

@Component({
  selector: 'app-vehicle-basics-tab-content',
  template: `
    <div class="card-body">
      <div class="card">
        <ul class="list-group list-group-flush">
          <li class="list-group-item d-flex" *ngFor="let v of tableData">
            <div class="d-flex w-100" *ngIf="v.type === 'string'">
              <div class="col-3 font-weight-bold">
                {{v.title}}
              </div>
              <div class="col">
                {{v.value}}
              </div>
            </div>
            <div class="d-flex w-100" *ngIf="v.type === 'array'">
              <div class="col-3 font-weight-bold">
                {{v.title}}
              </div>
              <div *ngFor="let f of v.value" class="col">
                {{f.name}}
              </div>
            </div>
          </li>
        </ul>
      </div>
    </div>
  `
})
export class VehicleViewBasicsTabContentComponent {
  @Input('table-data') tableData;
}
