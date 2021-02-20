import { Component, OnInit } from '@angular/core';
import { VehicleService } from "../vehicle.service";
import { IVehicle } from "../models/IVehicle";
import { IMake } from "../models/IMake";

@Component({
  selector: 'app-vehicles',
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent implements OnInit {
  // vehicles is for client-side filtering
  // vehicles: Array<IVehicle> = [];
  tableVehicles: Array<IVehicle> = [];
  makes: Array<IMake> = [];
  selectMake = 0;

  constructor(
    private vehicleService: VehicleService
  ) {
  }

  ngOnInit() {
    this.vehicleService.getVehicles().subscribe(vehicles => {
      // this.vehicles = [...vehicles];
      this.tableVehicles = [...vehicles];
    });

    this.vehicleService.getMakes().subscribe(makes => {
      this.makes = [...makes];
    });
  }

  onMakeChange() {
    this.vehicleService.getVehicles(`makeId=${this.selectMake}`).subscribe(v => {
      this.tableVehicles = [...v];
    });
  }

  // Client-Side filter
  // onMakeChange() {
  //   if (this.selectMake === 0) {
  //     this.filteredVehicles = [...this.vehicles];
  //   } else {
  //     this.filteredVehicles = _.filter(this.vehicles, v => v.make.id === this.selectMake);
  //   }
  // }
}
