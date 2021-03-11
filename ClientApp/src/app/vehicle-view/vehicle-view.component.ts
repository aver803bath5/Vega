import { Component, OnInit } from '@angular/core';
import { VehicleService } from "../vehicle.service";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";

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
  photos: IPhoto[] = [];
  vehicleId = 0;

  constructor(
    private vehicleService: VehicleService,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
  }

  ngOnInit() {
    this.vehicleId = +this.route.snapshot.paramMap.get('id');
    this.setTableData();
    this.getPhotos();
  }

  onFileUpload(event) {
    const files: FileList = event.target.files;
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      if (files[i])
        formData.append('files', files[i]);
    }
    this.vehicleService.uploadPhotos(this.vehicleId, formData).subscribe(x => {
      this.toastr.success('Photo has been uploaded', 'Success');
      this.getPhotos();
    });
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

  private getPhotos() {
    this.vehicleService.getPhotos(this.vehicleId).subscribe(photos => this.photos = [...photos]);
  }
}
