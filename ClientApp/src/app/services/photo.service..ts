import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpResponse } from "@angular/common/http";

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
    const req =  new HttpRequest('POST', `/api/vehicles/${ vehicleId }/photos`, photos, {
      // Enable this so that we can track the uploading process.
      reportProgress: true
    });

    return this.http.request(req);
  }
}
