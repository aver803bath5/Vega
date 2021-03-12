import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

import * as _ from "underscore";
import { forkJoin, Observable } from "rxjs";
import { ToastrService } from "ngx-toastr";

import { IMake } from "../shared/models/IMake";
import { IKeyValuePair } from "../shared/models/IKeyValuePair";
import { ISaveVehicle } from "../shared/models/ISaveVehicle";
import { IVehicle } from "../shared/models/IVehicle";
import { VehicleService } from "../vehicle.service";

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
  // Use vehicle property to check if the form is for updating or creating.
  // If vehicle property is null, the form is for creating. Otherwise, is for updating.
  // Also use this property to assign value to formGroup because the return structure object of the getVehicle service
  // is not fit the formGroup object structure.
  vehicle: IVehicle = null;
  makes = new Array<IMake>();
  models = new Array<IKeyValuePair>();
  features = new Array<IKeyValuePair>();

  get featuresFormArray() {
    return this.form.controls.features as FormArray;
  }

  constructor(
    private vehicleService: VehicleService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) {
  }

  ngOnInit() {
    const vehicleId = +this.route.snapshot.paramMap.get('id');
    const sources: Array<Observable<any>> = [
      this.vehicleService.getFeatures(),
      this.vehicleService.getMakes()
    ];
    if (vehicleId)
      sources.push(this.vehicleService.getVehicle(vehicleId));

    forkJoin(sources).subscribe((data: [Array<IKeyValuePair>, Array<IMake>, IVehicle]) => {
      this.features = [...data[0]];
      this.makes = [...data[1]];

      if (vehicleId) {
        this.vehicle = data[2];
        this.setForm();
      }
      this.populateFeatures(data[0]);
      this.populateModels();
    }, error => {
      if (error.status === 404)
        this.router.navigate(['/']);
    });
  }

  private setForm() {
    this.form.patchValue({
      id: this.vehicle.id,
      makeId: this.vehicle.make.id,
      modelId: this.vehicle.model.id,
      isRegistered: this.vehicle.isRegistered,
      contact: this.vehicle.contact
    });
  }

  private populateFeatures(features: IKeyValuePair[]) {
    features.forEach(() => {
      this.featuresFormArray.push(new FormControl(false));
    });

    if (this.vehicle !== null) {
      // set features values of the form
      const featuresValues = _.chain(features)
        .pluck('id')
        .map(x => _.pluck(this.vehicle.features as IKeyValuePair[], 'id').includes(x))
        .value();
      this.form.patchValue({
        features: featuresValues
      });
    }
  }

  onMakeSelect() {
    this.form.patchValue({
      modelId: 0
    });
    this.populateModels();
  }

  private populateModels() {
    const selectedMake = this.form.value.makeId;
    this.models = selectedMake ? this.makes.find(m => m.id == selectedMake).models : [];
  }

  onSubmit() {
    const selectedFeatureIds = this.form.value.features
      .map((checked: boolean, i) => checked ? this.features[i].id : -1)
      .filter((v: number) => v !== -1);
    const saveVehicle: ISaveVehicle = { ...this.form.value, features: selectedFeatureIds };

    if (this.vehicle !== null) {
      this.vehicleService.update(saveVehicle).subscribe(() => {
        this.toastr.success('Vehicle has been updated', 'Success');
        this.router.navigate([`/vehicles/view/${this.vehicle.id}`])
      });
    } else {
      this.vehicleService.create(saveVehicle).subscribe(vehicle => {
        this.toastr.success('Vehicle has been created', 'Success');
        this.router.navigate([`/vehicles/view/${vehicle.id}`])
      });
    }
  }

  delete() {
    if (confirm('Are you sure?')) {
      this.vehicleService.delete(this.form.value.id).subscribe(() => {
        this.toastr.success('Vehicle has been deleted', 'Success');
        this.router.navigate(['/']);
      });
    }
  }
}
