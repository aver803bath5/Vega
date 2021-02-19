import { Component, OnInit } from '@angular/core';
import { VehicleService } from "../vehicle.service";
import { IVehicle } from "../models/IVehicle";
import { IMake } from "../models/IMake";

import * as _ from "underscore";

@Component({
  selector: 'app-vehicles',
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent implements OnInit {
  vehicles: Array<IVehicle> = [];
  filteredVehicles: Array<IVehicle> = [];
  makes: Array<IMake> = [];
  selectMake = 0;

  constructor(
    private vehicleService: VehicleService
  ) {
  }

  ngOnInit() {
    this.vehicleService.getVehicles().subscribe(vehicles => {
      this.vehicles = [...vehicles];
      this.filteredVehicles = [...vehicles];
    });

    this.vehicleService.getMakes().subscribe(makes => {
      this.makes = [...makes];
    });
  }

  onMakeChange() {
    if (this.selectMake === 0) {
      this.filteredVehicles = [...this.vehicles];
    } else {
      this.filteredVehicles = _.filter(this.vehicles, v => v.make.id === this.selectMake);
    }
  }
}
