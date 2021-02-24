import { Component, OnInit } from '@angular/core';

import { faSortDown, faSortUp } from "@fortawesome/free-solid-svg-icons";

import { VehicleService } from "../vehicle.service";
import { IVehicle } from "../shared/models/IVehicle";
import { IMake } from "../shared/models/IMake";
import { forkJoin, Observable } from "rxjs";
import { ActivatedRoute, Router } from "@angular/router";
import { IPagination } from "./IPagination";

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
  selectedMake = 0;
  makeOrder = Order.Non;
  modelOrder = Order.Non;
  contactNameOrder = Order.Non;
  faSort = null;
  pagination: IPagination = null;
  queryParameters = {
    pageNumber: 1,
    makeId: 0,
    orderBy: "",
  }

  get sortLength() {
    return Object.keys(Order).filter(x => isNaN(Number(x))).length;
  }

  constructor(
    private vehicleService: VehicleService,
    private route: ActivatedRoute,
    private  router: Router
  ) {
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.queryParameters.pageNumber = +params.pageNumber;
      this.queryVehicles();
    });
  }


  onMakeChange() {
    this.queryParameters.pageNumber = 1;
    this.queryParameters.makeId = this.selectedMake;
    this.router.navigate(['/vehicles'], {queryParams: this.queryParameters});
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
    this.makeOrder === Order.Non && this.resetOrders();
    this.makeOrder = this.changeOrder(this.makeOrder);
    this.sortVehicles("model.make.name", this.makeOrder);
  }

  onClickModel() {
    this.modelOrder === Order.Non && this.resetOrders();
    this.modelOrder = this.changeOrder(this.modelOrder);
    this.sortVehicles("model.name", this.modelOrder);
  }

  onClickContactName() {
    this.contactNameOrder === Order.Non && this.resetOrders();
    this.contactNameOrder = this.changeOrder(this.contactNameOrder);
    this.sortVehicles("contact.name", this.contactNameOrder);
  }

  setSortIcon(order = Order.Non) {
    switch (order) {
      case Order.Descending:
        return faSortDown;
      case Order.Ascending:
        return faSortUp;
      default:
        return null;
    }
  }

  private queryVehicles() {
    const source: Array<Observable<any>> = [this.vehicleService.getVehicles(this.queryParameters), this.vehicleService.getMakes()];
    forkJoin(source).subscribe(data => {
      this.setTableData(data[0]);
      this.makes = [...data[1]];
    });
  }

  private resetOrders() {
    this.makeOrder = Order.Non;
    this.modelOrder = Order.Non;
    this.contactNameOrder = Order.Non;
  }

  private sortVehicles(orderBy = "", order = Order.Non) {
    switch (order) {
      case Order.Descending:
        this.queryParameters.orderBy = `${ orderBy } desc`
        this.vehicleService.getVehicles(this.queryParameters).subscribe(res => {
          this.tableVehicles = [...res.body as Array<IVehicle>];
        });
        break;
      case Order.Ascending:
        this.queryParameters.orderBy = `${ orderBy }`
        this.vehicleService.getVehicles(this.queryParameters).subscribe(res => {
          this.tableVehicles = [...res.body as Array<IVehicle>];
        });
        break;
      default:
        this.queryParameters.orderBy = "";
        this.vehicleService.getVehicles(this.queryParameters).subscribe(res => {
          this.tableVehicles = [...res.body as Array<IVehicle>];
        });
    }
  }

  private changeOrder(sort: Order) {
    return (sort + 1) % this.sortLength;
  }

  private setTableData(res) {
    this.tableVehicles = [...res.body as Array<IVehicle>];
    this.pagination = JSON.parse(res.headers.get('x-pagination'));
  }
}
