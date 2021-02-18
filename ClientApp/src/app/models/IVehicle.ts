import { IKeyValuePair } from "./IKeyValuePair";
import { IContact } from "./IContact";

export interface IVehicle {
  id: Number;
  make: IKeyValuePair;
  model: IKeyValuePair;
  features: Array<IKeyValuePair>;
  contact: IContact;
  isRegistered: Boolean;
}
