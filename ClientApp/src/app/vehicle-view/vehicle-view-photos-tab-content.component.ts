import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { PhotoService } from "../services/photo.service.";
import { HttpEventType, HttpResponse } from "@angular/common/http";
import { finalize, takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { IPhoto } from "../shared/models/IPhoto";
import { AuthService } from "@auth0/auth0-angular";

@Component({
  selector: 'app-vehicle-view-photos-tab-content',
  template: `
    <ng-container>
      <!-- Container for file input and cancel upload button to hide the whole upload photos component.-->
      <ng-container *ngIf="auth.isAuthenticated$ | async">
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
        <div class="progress" *ngIf="uploadProgress.percentage > 0">
          <div class="progress-bar" role="progressbar" [style.width]="uploadProgress.percentage + '%'"
               [attr.aria-valuenow]="uploadProgress.percentage" aria-valuemin="0" aria-valuemax="100">
            {{uploadProgress.percentage}}%
          </div>
        </div>
        <button [disabled]="uploadProgress.percentage < 0" class="btn btn-danger" type="button"
                (click)="cancelUpload()">
          Cancel
        </button>
      </ng-container>

      <div class="row row-cols-1 row-cols-md-6">
        <div *ngFor="let p of photos" class="col mb-4">
          <div class="card">
            <img class="img-thumbnail"
                 src="{{generateImageURL(p)}}" alt="">
            <button *ngIf="auth.isAuthenticated$ | async" class="btn btn-danger btn-block" (click)="delete(p.id)"
                    type="button" [disabled]="isLoading">
              <ng-container *ngIf="!isLoading; else loadingText">
                Delete
              </ng-container>
              <ng-template #loadingText>
                Deleting...
              </ng-template>
            </button>
          </div>
        </div>
      </div>
    </ng-container>
  `
})
export class VehicleViewPhotosTabContentComponent implements OnInit {
  vehicleId = 0;
  photos: IPhoto[] = [];
  cancelUpload$ = new Subject()
  uploadProgress = {
    percentage: -1
  };
  isLoading = false

  constructor(
    private toastr: ToastrService,
    private photoService: PhotoService,
    private route: ActivatedRoute,
    public auth: AuthService
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
      .pipe(
        takeUntil(this.cancelUpload$),
        finalize(() => {
          // Reset upload progress
          this.uploadProgress.percentage = -1;
          // Reset file input value
          event.target.value = '';
        })
      )
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

  cancelUpload() {
    this.cancelUpload$.next('cancel');
  }

  delete(photoId) {
    if (confirm('Do you really want to delete this photos?')) {
      this.isLoading = true;
      this.photoService.deletePhoto(this.vehicleId, photoId)
        .pipe(
          finalize(() => this.isLoading = false)
        )
        .subscribe(() => {
          this.toastr.success('Photo has been deleted.', 'Success');
          // Remove the removed photo from the photos array.
          const removedPhotoIndex = this.photos.findIndex(p => p.id == photoId);
          this.photos.splice(removedPhotoIndex, 1);
        });
    }
  }

  generateImageURL(photo: IPhoto) {
    // Because thumbnail feature is added after regular image upload feature, some photos would not have thumbnail.
    // Return original image filepath if there is no thumbnail filepath in the photo object.
    const imageFilename = photo.thumbnail !== '' ? photo.thumbnail : photo.fileName;
    return `/uploads/VehiclePhotos/${this.vehicleId}/${imageFilename}`;
  }
}
