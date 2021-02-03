import { Component, OnInit } from '@angular/core';
import {FormArray, FormBuilder, FormControl, Validators} from "@angular/forms";

import { VehicleFormService } from "../vehicle-form.service";
import {IMake} from "../../core/models/IMake";
import {IModel} from "../../core/models/IModel";
import {IFeature} from "../../core/models/IFeature";

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit {
  form = new FormBuilder().group({
    id: 0,
    make: [0, Validators.min(1)],
    model: [0, Validators.min(1)],
    features: new FormArray([]),
    isRegistered: false,
    contactName: "",
    contactPhone: "",
    contactEmail: "",
  });
  makes = new Array<IMake>();
  models = new Array<IModel>();
  features = new Array<IFeature>();

  get featuresFormArray() {
    return this.form.controls.features as FormArray;
  }

  constructor(
    private vehicleFormService: VehicleFormService
  ) { }

  ngOnInit() {
    this.vehicleFormService.getMakes().subscribe(result  => {
      this.makes.push(...result);
    });

    this.vehicleFormService.getFeatures().subscribe(result => {
      this.features.push(...result);
      result.forEach(() => this.featuresFormArray.push(new FormControl(false)));
    });
  }

  onMakeSelect(event) {
    const makeId = event.target.value;
    this.form.patchValue({
      model: 0
    });

    for (let i = 0; i < this.makes.length; i++)
    {
      if (this.makes[i].id.toString() === makeId)
      {
        this.models.push(...this.makes[i].models);
        return;
      }
    }
  }

  onSubmit() {
    const selectedFeatureIds = this.form.value.features
      .map((checked, i) => checked ? this.features[i].id : checked)
      .filter(v => v !== false);
    console.log(this.form.value);
  }
}
