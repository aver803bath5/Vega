import { IContact } from "./IContact";

export interface ISaveVehicle {
  id: Number;
  makeId: Number;
  modelId: Number;
  features: Array<Number>;
  contact: IContact;
}

