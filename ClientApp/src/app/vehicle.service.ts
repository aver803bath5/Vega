import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IMake } from "./models/IMake";
import { IKeyValuePair } from "./models/IKeyValuePair";
import { ISaveVehicle } from "./models/ISaveVehicle";
import { IVehicle } from "./models/IVehicle";

@Injectable({
  providedIn: 'root',
})
export class VehicleService {

  constructor(private http: HttpClient) { }

  getMakes() {
    return this.http.get<Array<IMake>>('/api/makes');
  }

  getFeatures() {
    return this.http.get<Array<IKeyValuePair>>('/api/features');
  }

  getVehicle(id: Number) {
    return this.http.get<IVehicle>(`/api/vehicles/${id}`);
  }

  create(saveVehicle: ISaveVehicle) {
    return this.http.post('/api/vehicles', saveVehicle);
  }

  update(saveVehicle: ISaveVehicle) {
    return this.http.put(`/api/vehicles/${saveVehicle.id}`, saveVehicle);
  }
}
