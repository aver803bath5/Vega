import { ErrorHandler, Inject, isDevMode, NgZone } from "@angular/core";
import { ToastrService } from "ngx-toastr";

export class AppErrorHandler implements ErrorHandler {
  constructor(
    private ngZone: NgZone,
    @Inject(ToastrService) private toastr: ToastrService
  ) {
  }

  handleError(error: any): void {
    this.ngZone.run(() => {
      if (!isDevMode())
        this.toastr.error("An unexpected happened", "Error");
      console.error(error);
    });
  }
}
