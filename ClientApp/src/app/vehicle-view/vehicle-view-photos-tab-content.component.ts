import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { PhotoService } from "../services/photo.service.";
import { HttpEventType, HttpResponse } from "@angular/common/http";
import { finalize } from "rxjs/operators";

@Component({
  selector: 'app-vehicle-view-photos-tab-content',
  template: `
    <ng-container>
      <div class="input-group">
        <div class="input-group-prepend">
          <span class="input-group-text" id="inputGroupFileAddon01">Upload</span>
        </div>
        <div class="custom-file">
          <input type="file" class="custom-file-input" id="vehicleFile" aria-describedby="vehiclePhotoFileUploadInput"
                 (change)="onFileUpload($event)">
          <label class="custom-file-label" for="vehicleFile">Choose file</label>
        </div>
      </div>
      <div class="progress" *ngIf="uploadProgress.percentage >= 0">
        <div class="progress-bar" role="progressbar" [style.width]="uploadProgress.percentage + '%'"
             [attr.aria-valuenow]="uploadProgress.percentage" aria-valuemin="0" aria-valuemax="100">
          {{uploadProgress.percentage}}%
        </div>
      </div>
      <div class="row row-cols-1 row-cols-md-3">
        <div *ngFor="let p of photos" class="col mb-4">
          <div class="card">
            <img class="img-thumbnail" src="/uploads/VehiclePhotos/{{vehicleId}}/{{p.fileName}}" alt="">
          </div>
        </div>
      </div>
    </ng-container>
  `
})
export class VehicleViewPhotosTabContentComponent implements OnInit {
  vehicleId = 0;
  photos: IPhoto[] = [];
  uploadProgress = {
    percentage: -1
  };

  constructor(
    private toastr: ToastrService,
    private photoService: PhotoService,
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

    this.photoService.uploadPhotos(this.vehicleId, formData)
      .pipe(finalize(() => {
        // Reset upload progress
        this.uploadProgress.percentage = -1;
        // Reset file input value
        event.target.value = '';
      }))
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          this.uploadProgress.percentage = Math.round(event.loaded / event.total * 100);
        } else if (event instanceof HttpResponse) {
          this.toastr.success('Photo has been uploaded', 'Success');
          this.photos.push(...event.body as IPhoto[]);
        }
      }, error => {
        if (error.status == 400)
          error.error.files.forEach(errorMessage => this.toastr.error(errorMessage));
      });
  }

  private getPhotos() {
    this.photoService.getPhotos(this.vehicleId).subscribe(photos => this.photos = [...photos]);
  }
}
