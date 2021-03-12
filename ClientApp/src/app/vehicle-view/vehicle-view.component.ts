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
    this.getPhotos();
  }

  onFileUpload(event) {
    const files: FileList = event.target.files;
    if (files.length === 0) return;

    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      if (files[i])
        formData.append('files', files[i]);
    }
    this.vehicleService.uploadPhotos(this.vehicleId, formData).subscribe(() => {
      this.toastr.success('Photo has been uploaded', 'Success');
      // reset file input value
      event.target.value = '';
      this.getPhotos();
    }, error => {
      if (error.status == 400)
        error.error.files.forEach(errorMessage => this.toastr.error(errorMessage));
    });
  }


  private getPhotos() {
    this.vehicleService.getPhotos(this.vehicleId).subscribe(photos => this.photos = [...photos]);
  }
}
