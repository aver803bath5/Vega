import { IKeyValuePair } from "./IKeyValuePair";

export interface IMake {
    id: Number,
    name: String,
    models: Array<IKeyValuePair>,
}
