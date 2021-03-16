import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IMake } from "./shared/models/IMake";
import { IKeyValuePair } from "./shared/models/IKeyValuePair";
import { ISaveVehicle } from "./shared/models/ISaveVehicle";
import { IVehicle } from "./shared/models/IVehicle";

@Injectable({
  providedIn: 'root'
})
export class VehicleService {

  constructor(private http: HttpClient) {
  }

  getMakes() {
    return this.http.get<Array<IMake>>('/api/makes');
  }

  getFeatures() {
    return this.http.get<Array<IKeyValuePair>>('/api/features');
  }

  getVehicle(id: Number) {
    return this.http.get<IVehicle>(`/api/vehicles/${ id }`);
  }

  getVehicles(queryParameters) {
    return this.http.get(`/api/vehicles`, {
      observe: 'response',
      params: queryParameters
    });
  }

  create(saveVehicle: ISaveVehicle) {
    return this.http.post<IVehicle>('/api/vehicles', saveVehicle);
  }

  update(saveVehicle: ISaveVehicle) {
    return this.http.put<IVehicle>(`/api/vehicles/${ saveVehicle.id }`, saveVehicle);
  }

  delete(id: Number) {
    return this.http.delete(`/api/vehicles/${ id }`);
  }
}
