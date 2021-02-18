import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IMake } from "./models/IMake";
import { IKeyValuePair } from "./models/IKeyValuePair";
import { ISaveVehicle } from "./models/ISaveVehicle";

@Injectable({
  providedIn: 'root',
})
export class VehicleFormService {

  constructor(private http: HttpClient) { }

  getMakes() {
    return this.http.get<Array<IMake>>('/api/makes');
  }

  getFeatures() {
    return this.http.get<Array<IKeyValuePair>>('/api/features');
  }

  createVehicle(saveVehicle: ISaveVehicle) {
    return this.http.post('/api/vehicles', saveVehicle);
  }
}
