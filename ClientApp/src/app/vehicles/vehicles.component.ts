import { Component, OnInit } from '@angular/core';

import { faSort } from "@fortawesome/free-solid-svg-icons";

import { VehicleService } from "../vehicle.service";
import { IVehicle } from "../models/IVehicle";
import { IMake } from "../models/IMake";

enum Order {
  Non,
  Descending,
  Ascending
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
  makeOrder = Order.Non;
  modelOrder = Order.Non;
  contactNameOrder = Order.Non;
  faSort = faSort;
  readonly sortLength = Object.keys(Order).filter(x => isNaN(Number(x))).length;

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
    this.makeOrder = this.changeOrder(this.makeOrder);
    this.sortVehicles("model.make.name", this.makeOrder);
  }

  onClickModel() {
    this.modelOrder = this.changeOrder(this.modelOrder);
    this.sortVehicles("model.name", this.modelOrder);
  }

  onClickContactName() {
    this.contactNameOrder = this.changeOrder(this.contactNameOrder);
    this.sortVehicles("contact.name", this.contactNameOrder);
  }

  private sortVehicles(orderBy = "", order = Order.Non) {
    switch (order) {
      case Order.Descending:
        this.vehicleService.getVehicles(`orderBy=${ orderBy } desc`).subscribe(v => {
          this.tableVehicles = [...v];
        });
        break;
      case Order.Ascending:
        this.vehicleService.getVehicles(`orderBy=${ orderBy }`).subscribe(v => {
          this.tableVehicles = [...v];
        });
        break;
      default:
        this.vehicleService.getVehicles().subscribe(vehicles => {
          this.tableVehicles = [...vehicles];
        });
    }
  }

  private changeOrder(sort: Order) {
    return (sort + 1) % this.sortLength;
  }
}
