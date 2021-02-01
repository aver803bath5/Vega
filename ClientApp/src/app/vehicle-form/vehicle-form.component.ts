import { Component, OnInit } from '@angular/core';
import { FormBuilder } from "@angular/forms";

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit {
  vehicleForm = new FormBuilder().group({
    id: 0,
    make: 0,
    models: new Array<Number>(),
    isRegistered: false,
    contactName: "",
    contactForm: "",
    contactEmail: "",
  });

  constructor() { }

  ngOnInit() {
  }
}
