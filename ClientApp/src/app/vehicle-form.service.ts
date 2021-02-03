import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import {IMake} from "../core/models/IMake";
import {IFeature} from "../core/models/IFeature";

@Injectable({
  providedIn: 'root',
})
export class VehicleFormService {

  constructor(private http: HttpClient) { }

  getMakes() {
    return this.http.get<Array<IMake>>('/api/makes');
  }

  getFeatures() {
    return this.http.get<Array<IFeature>>('/api/features');
  }
}
