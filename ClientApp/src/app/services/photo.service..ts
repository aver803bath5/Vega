import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
  constructor(
    private http: HttpClient
  ) {
  }

  getPhotos(vehicleId: Number) {
    return this.http.get<Array<IPhoto>>(`/api/vehicles/${ vehicleId }/photos`);
  }

  uploadPhotos(vehicleId, photos) {
    return this.http.post(`/api/vehicles/${ vehicleId }/photos`, photos);
  }
}
