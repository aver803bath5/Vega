import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

import * as _ from "underscore";
import { forkJoin, Observable, of, Subject } from "rxjs";
import { ToastrService } from "ngx-toastr";

import { IMake } from "../shared/models/IMake";
import { IKeyValuePair } from "../shared/models/IKeyValuePair";
import { ISaveVehicle } from "../shared/models/ISaveVehicle";
import { IVehicle } from "../shared/models/IVehicle";
import { VehicleService } from "../services/vehicle.service";
import { switchMap, takeUntil } from "rxjs/operators";

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit, OnDestroy {
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
  result$ = new Subject<ISaveVehicle>();
  bye$ = new Subject();

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
    this.generateRequestSubject();
    this.populateForm();
  }

  private generateRequestSubject() {
    this.result$
      .pipe(
        takeUntil(this.bye$),
        // Use switchMap to prevent from sending request with same data repeatedly.
        // Use the vehicle property to decide which state of the form is. If the property is null means this is
        // creating state, otherwise is editing state.
        switchMap(v => this.vehicle == null ?
          this.vehicleService.create(v) : this.vehicleService.update(v)),
      )
      // If the request is response successfully then show a success toast and navigate to the vehicle info page of the
      // vehicle currently created or edited.
      .subscribe((vehicle) => {
        this.toastr.success('Vehicle has been saved', 'Success');
        this.router.navigate(['/vehicles', vehicle.id])
      });
  }

  // Populate the data for the form including the makes data and features data
  // and the vehicle data if there is vehicle id existing.
  private populateForm() {
    const vehicleId = +this.route.snapshot.paramMap.get('id');
    const sources: Array<Observable<any>> = [
      this.vehicleService.getFeatures(),
      this.vehicleService.getMakes()
    ];

    // If there is vehicle id which means this is edit form so it needs to get the vehicle data to fill the data to the
    // form.
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
      // Navigate to the home page if any requests return 404. That's means the API is broke or the given vehicle id
      // is invalid due to can't get the vehicle data based on the vehicle id.
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
    this.result$.next(saveVehicle);
  }

  delete() {
    if (confirm('Are you sure?')) {
      this.vehicleService.delete(this.form.value.id).subscribe(() => {
        this.toastr.success('Vehicle has been deleted', 'Success');
        this.router.navigate(['/']);
      });
    }
  }

  ngOnDestroy(): void {
    this.bye$.next();
  }
}
