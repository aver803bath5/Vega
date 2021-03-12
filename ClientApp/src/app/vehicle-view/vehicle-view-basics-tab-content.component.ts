import { Component, Input, OnInit } from "@angular/core";
import { VehicleService } from "../vehicle.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

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
      <div class="mt-2">
        <a [routerLink]="['/vehicles', vehicleId]" class="btn btn-primary" role="button">Edit</a>
        <button type="button" class="btn btn-danger ml-2" (click)="delete()">Delete</button>
      </div>
    </div>
  `
})
export class VehicleViewBasicsTabContentComponent implements OnInit{
  tableData = [
    { title: 'Make', key: 'make.name', value: '', type: 'string' },
    { title: 'Model', key: 'model.name', value: '', type: 'string' },
    { title: 'Contact Name', key: 'contact.name', value: '', type: 'string' },
    { title: 'Contact Phone', key: 'contact.phone', value: '', type: 'string' },
    { title: 'Contact Email', key: 'contact.email', value: '', type: 'string' },
    { title: 'Registered', key: 'isRegistered', value: '', type: 'string' },
    { title: 'Features', key: 'features', value: [], type: 'array' },
  ];
  vehicleId = 0

  constructor(
    private vehicleService: VehicleService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.vehicleId = +this.route.snapshot.paramMap.get('id');
  }

  ngOnInit(): void {
    this.setTableData();
  }

  private setTableData() {
    this.vehicleService.getVehicle(this.vehicleId).subscribe(vehicle => {
      this.tableData = this.tableData.map(row => {
        if (row.type === 'string') {
          // Get the property value from the object.
          // eg: vehicle: { contact: { name: 'abc' } } so I need to get vehicle['contact]['name'] value.
          const value = row.key.split('.').reduce((acc, key) => {
            return acc[key];
          }, vehicle);
          return {
            ...row,
            value: value
          };
        } else if (row.type === 'array') {
          // If row type is array, I will process it in the html. because it is easier to iterate it and display the
          // elements inside the array.
          return {
            ...row,
            value: vehicle[row.key]
          };
        }
      });
    });
  }

  delete() {
    if (confirm('Are you sure?')) {
      this.vehicleService.delete(this.vehicleId).subscribe(() => {
        this.toastr.success('Vehicle has been deleted', 'Success');
        this.router.navigate(['/']);
      });
    }
  }
}
