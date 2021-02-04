import {Component, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormControl, ValidatorFn, Validators} from "@angular/forms";

import {VehicleFormService} from "../vehicle-form.service";
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
    contactName: ["", [Validators.maxLength(255), Validators.required]],
    contactPhone: ["", [Validators.maxLength(255), Validators.required]],
    contactEmail: ["", [Validators.email, Validators.maxLength(255), Validators.required]],
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
    const makeId = this.form.controls.make.value;
    this.form.patchValue({
      model: 0
    });

    this.models = this.makes.find(m => m.id == makeId).models;
  }

  onSubmit() {
    const selectedFeatureIds = this.form.value.features
      .map((checked, i) => checked ? this.features[i].id : checked)
      .filter(v => v !== false);


    console.log(this.form.value);
  }
}
