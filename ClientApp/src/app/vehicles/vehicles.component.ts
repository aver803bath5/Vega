import { Component, OnInit } from '@angular/core';

import { faSort } from "@fortawesome/free-solid-svg-icons";

import { VehicleService } from "../vehicle.service";
import { IVehicle } from "../models/IVehicle";
import { IMake } from "../models/IMake";
import { createPerformWatchHost } from "@angular/compiler-cli/src/perform_watch";

enum Sort {
  non,
  descending,
  ascending
}

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
  makeSort = Sort.non;
  faSort = faSort;
  readonly sortLength = Object.keys(Sort).filter(x => isNaN(Number(x))).length;

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
    this.vehicleService.getVehicles(`makeId=${ this.selectMake }`).subscribe(v => {
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
  onClickMake() {
    this.makeSort = this.changeSort(this.makeSort);
    switch (this.makeSort) {
      case Sort.descending:
        this.vehicleService.getVehicles(`orderBy=model.make.name desc`).subscribe(v => {
          this.tableVehicles = [...v];
        });
        break;
      case Sort.ascending:
        this.vehicleService.getVehicles(`orderBy=model.make.name`).subscribe(v => {
          this.tableVehicles = [...v];
        });
        break;
      default:
        this.vehicleService.getVehicles().subscribe(vehicles => {
          this.tableVehicles = [...vehicles];
        });
    }
  }

  onClickModel() {

  }

  onClickContactName() {

  }

  private changeSort(sort: Sort) {
    return (sort + 1) % this.sortLength;
  }
}
