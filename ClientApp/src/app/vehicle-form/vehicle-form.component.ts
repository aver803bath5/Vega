import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

import * as _ from "underscore";
import { forkJoin, Observable } from "rxjs";

import { VehicleFormService } from "../vehicle-form.service";
import { IMake } from "../models/IMake";
import { IKeyValuePair } from "../models/IKeyValuePair";
import { ISaveVehicle } from "../models/ISaveVehicle";
import { IVehicle } from "../models/IVehicle";

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
  models = new Array<IKeyValuePair>();
  features = new Array<IKeyValuePair>();

  get featuresFormArray() {
    return this.form.controls.features as FormArray;
  }

  constructor(
    private vehicleFormService: VehicleFormService,
    private route: ActivatedRoute,
    private router: Router
  ) {
  }

  ngOnInit() {
    const vehicleId = +this.route.snapshot.paramMap.get('id');
    const sources: Array<Observable<any>> = [
      this.vehicleFormService.getFeatures(),
      this.vehicleFormService.getMakes()
    ];
    if (vehicleId)
      sources.push(this.vehicleFormService.getVehicle(vehicleId));

    forkJoin(sources).subscribe((data: [Array<IKeyValuePair>, Array<IMake>, IVehicle]) => {
      this.features = [...data[0]];
      this.makes = [...data[1]];

      if (vehicleId) {
        this.generateFeaturesFormArray(data[2].features);
        this.setVehicle(data[2]);
      }
      this.populateModels();
    }, error => {
      if (error.status === 404)
        this.router.navigate(['/']);
    });
  }

  private setVehicle(v: IVehicle) {
    this.form.patchValue({
      id: v.id,
      makeId: v.make.id,
      modelId: v.model.id,
      isRegistered: v.isRegistered,
      contact: v.contact
    });
  }

  private generateFeaturesFormArray(vehicleFeatures: Array<IKeyValuePair>) {
    const vehicleFeatureIds = _.pluck(vehicleFeatures, 'id');
    this.features.forEach(f => {
      this.featuresFormArray.push(new FormControl(vehicleFeatureIds.includes(f.id)));
    });
  }

  onMakeSelect() {
    this.form.patchValue({
      modelId: 0
    });
    this.populateModels();
  }

  private populateModels() {
    const selectedMake = this.form.value.makeId;
    console.log(selectedMake);
    this.models = selectedMake ? this.makes.find(m => m.id == selectedMake).models : [];
  }

  onSubmit() {
    const selectedFeatureIds = this.form.value.features
      .map((checked: boolean, i) => checked ? this.features[i].id : -1)
      .filter((v: number) => v !== -1);
    const saveVehicle: ISaveVehicle = { ...this.form.value, features: selectedFeatureIds };

    if (this.form.controls.id.value) {
      this.vehicleFormService.update(saveVehicle).subscribe(x => {
        console.log(x);
      });
    } else {
      this.vehicleFormService.create(saveVehicle).subscribe(x => {
        console.log(x);
      });
    }
  }
}
