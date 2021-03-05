import { Component, OnInit } from '@angular/core';
import { VehicleService } from "../vehicle.service";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-vehicle-view',
  templateUrl: './vehicle-view.component.html',
  styleUrls: ['./vehicle-view.component.css']
})
export class VehicleViewComponent implements OnInit {
  tableData = [
    { title: 'Make', key: 'make.name', value: '', type: 'string' },
    { title: 'Model', key: 'model.name', value: '', type: 'string' },
    { title: 'Contact Name', key: 'contact.name', value: '', type: 'string' },
    { title: 'Contact Phone', key: 'contact.phone', value: '', type: 'string' },
    { title: 'Contact Email', key: 'contact.email', value: '', type: 'string' },
    { title: 'Registered', key: 'isRegistered', value: '', type: 'string' },
    { title: 'Features', key: 'features', value: [], type: 'array' },
  ];

  constructor(
    private vehicleService: VehicleService,
    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.setTableData();
  }

  private setTableData() {
    const vehicleId = +this.route.snapshot.paramMap.get('id');
    this.vehicleService.getVehicle(vehicleId).subscribe(vehicle => {
      this.tableData = this.tableData.map(row => {
        if (row.type === 'string') {
          const value = row.key.split('.').reduce((acc, key) => {
            return acc[key];
          }, vehicle);
          return {
            ...row,
            value: value
          };
        } else if (row.type === 'array') {
          return {
            ...row,
            value: vehicle[row.key]
          };
        }
      });
    });
  }
}
