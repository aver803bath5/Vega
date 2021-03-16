import { Component, OnInit } from '@angular/core';

import { faSortDown, faSortUp } from "@fortawesome/free-solid-svg-icons";

import { VehicleService } from "../services/vehicle.service";
import { IVehicle } from "../shared/models/IVehicle";
import { IMake } from "../shared/models/IMake";
import { ActivatedRoute, Router } from "@angular/router";
import { IPagination } from "../shared/models/IPagination";

interface IQueryParameters {
  pageNumber?: number;
  makeId?: number;
  orderBy?: string;
}

interface IColumn {
  title: string;
  key?: string;
  isSortable?: boolean;
}

@Component({
  selector: 'app-vehicles',
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.css']
})
export class VehicleListComponent implements OnInit {
  // vehicles is for client-side filtering
  // vehicles: Array<IVehicle> = [];
  tableVehicles: Array<IVehicle> = [];
  makes: Array<IMake> = [];
  selectedMake = 0;
  faSort = null;
  pagination: IPagination = null;
  queryParameters: IQueryParameters = {
    pageNumber: 1,
    makeId: 0,
    orderBy: "",
  }
  columns: IColumn[] = [
    { title: 'Id' },
    { title: 'Make', key: 'make', isSortable: true },
    { title: 'Model', key: 'model', isSortable: true },
    { title: 'Contact Name', key: 'contactName', isSortable: true },
    { title: '' },
  ]

  constructor(
    private vehicleService: VehicleService,
    private route: ActivatedRoute,
    private  router: Router
  ) {
  }

  ngOnInit() {
    this.vehicleService.getMakes().subscribe(makes => this.makes = makes);
    this.route.queryParams.subscribe(params => {
      if (params.pageNumber)
        this.queryParameters.pageNumber = params.pageNumber;
      this.populateVehicles();
    });
  }


  onMakeChange() {
    this.queryParameters = {
      pageNumber: 1,
      makeId: this.selectedMake,
      orderBy: ''
    };
    this.router.navigate(['/vehicles'], { queryParams: this.queryParameters }).then();
  }

  // Client-Side filter
  // onMakeChange() {
  //   if (this.selectMake === 0) {
  //     this.filteredVehicles = [...this.vehicles];
  //   } else {
  //     this.filteredVehicles = _.filter(this.vehicles, v => v.make.id === this.selectMake);
  //   }
  // }

  setSortIcon() {
    if (this.queryParameters.orderBy.includes('desc'))
      return faSortDown;
    else
      return faSortUp;
  }

  private populateVehicles() {
    this.vehicleService.getVehicles(this.queryParameters).subscribe(res => {
      this.tableVehicles = [...res.body as Array<IVehicle>];
      this.pagination = JSON.parse(res.headers.get('x-pagination'));
    });
  }

  sortVehicles(orderBy) {
    if (this.queryParameters.orderBy === orderBy)
      this.queryParameters.orderBy = `${ orderBy } desc`;
    else
      this.queryParameters.orderBy = orderBy;

    this.populateVehicles();
  }
}
