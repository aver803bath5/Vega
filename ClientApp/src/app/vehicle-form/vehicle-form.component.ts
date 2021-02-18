import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from "@angular/forms";

import { VehicleFormService } from "../vehicle-form.service";
import { IMake } from "../models/IMake";
import { IModel } from "../models/IModel";
import { IFeature } from "../models/IFeature";
import { ISaveVehicle } from "../models/ISaveVehicle";

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit {
  form = new FormBuilder().group({
    id: 0,
    makeId: [0, Validators.min(1)],
    modelId: [0, Validators.min(1)],
    features: new FormArray([]),
    isRegistered: false,
    contact: new FormBuilder().group({
      name: ["", [Validators.maxLength(255), Validators.required]],
      phone: ["", [Validators.maxLength(255), Validators.required]],
      email: ["", [Validators.email, Validators.maxLength(255)]],
    })
  });
  makes = new Array<IMake>();
  models = new Array<IModel>();
  features = new Array<IFeature>();

  get featuresFormArray() {
    return this.form.controls.features as FormArray;
  }

  constructor(
    private vehicleFormService: VehicleFormService
  ) {
  }

  ngOnInit() {
    this.vehicleFormService.getMakes().subscribe(makes => {
      this.makes = makes;
    });

    this.vehicleFormService.getFeatures().subscribe(features => {
      this.features = features;
      features.forEach(() => this.featuresFormArray.push(new FormControl(false)));
    });
  }

  onMakeSelect() {
    const makeId = this.form.controls.makeId.value;
    this.form.patchValue({
      modelId: 0
    });

    this.models = this.makes.find(m => m.id == makeId).models;
  }

  onSubmit() {
    const selectedFeatureIds = this.form.value.features
      .map((checked: boolean, i) => checked ? this.features[i].id : -1)
      .filter((v: number) => v !== -1);
    const saveVehicle: ISaveVehicle = { ...this.form.value, features: selectedFeatureIds };
    this.vehicleFormService.createVehicle(saveVehicle).subscribe(x => {
      console.log(x);
    });
  }
}
