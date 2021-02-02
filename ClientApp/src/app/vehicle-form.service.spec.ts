import { TestBed } from '@angular/core/testing';

import { VehicleFormService } from './vehicle-form.service';

describe('VehicleFormService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: VehicleFormService = TestBed.get(VehicleFormService);
    expect(service).toBeTruthy();
  });
});
