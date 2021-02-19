import { Component, OnInit } from '@angular/core';
import { VehicleService } from "../vehicle.service";
import { IVehicle } from "../models/IVehicle";

@Component({
  selector: 'app-vehicles',
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent implements OnInit {
  vehicles: Array<IVehicle> = [];

  constructor(
    private vehicleService: VehicleService
  ) { }

  ngOnInit() {
    this.vehicleService.getVehicles().subscribe(vehicles => {
      this.vehicles = [...vehicles];
    });
  }
}
