import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { VehicleService } from "../vehicle.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-vehicle-view-photos-tab-content',
  template: `
    <ng-container>
      <div class="input-group mb-3">
        <div class="input-group-prepend">
          <span class="input-group-text" id="inputGroupFileAddon01">Upload</span>
        </div>
        <div class="custom-file">
          <input type="file" class="custom-file-input" id="vehicleFile" aria-describedby="vehiclePhotoFileUploadInput"
                 (change)="onFileUpload($event)">
          <label class="custom-file-label" for="vehicleFile">Choose file</label>
        </div>
      </div>
      <div class="row row-cols-1 row-cols-md-3">
        <div *ngFor="let p of photos" class="col mb-4">
          <div class="card">
            <img class="img-thumbnail" [src]="p.requestPath" alt="">
          </div>
        </div>
      </div>
    </ng-container>
  `
})
export class VehicleViewPhotosTabContentComponent implements OnInit {
  vehicleId = 0;
  photos: IPhoto[] = [];

  constructor(
    private toastr: ToastrService,
    private vehicleService: VehicleService,
    private route: ActivatedRoute
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
      // Reset file input value
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
