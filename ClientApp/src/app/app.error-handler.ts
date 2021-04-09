import { ErrorHandler, Injectable, Injector, isDevMode } from "@angular/core";
import { ToastrService } from "ngx-toastr";

@Injectable()
export class AppErrorHandler implements ErrorHandler {
  private toastr: ToastrService
  constructor(
    injector: Injector
  ) {
    setTimeout(() => this.toastr = injector.get(ToastrService))
  }

  handleError(error: any): void {
    if (!isDevMode())
      this.toastr.error("An unexpected happened", "Error");
    console.error(error);
  }
}
