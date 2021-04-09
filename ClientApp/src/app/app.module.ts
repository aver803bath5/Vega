import { BrowserModule } from '@angular/platform-browser';
import { ErrorHandler, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { ToastrModule } from "ngx-toastr";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";

import { environment as env } from "../environments/environment";

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { VehicleFormComponent } from './vehicle-form/vehicle-form.component';
import { AppErrorHandler } from "./app.error-handler";
import { VehicleListComponent } from "./vehicle-list/vehicle-list.component";
import { PaginationComponent } from "./shared/components/pagination.component";
import { VehicleViewComponent } from "./vehicle-view/vehicle-view.component";
import { RangePipe } from './pipes/range.pipe';
import { TabsComponent } from "./shared/components/tabs.component";
import { TabComponent } from "./shared/components/tab.component";
import { CardTabsComponent } from "./shared/components/card-tabs.component";
import { VehicleViewBasicsTabContentComponent } from "./vehicle-view/vehicle-view-basics-tab-content.component";
import { VehicleViewPhotosTabContentComponent } from "./vehicle-view/vehicle-view-photos-tab-content.component";
import { AuthButtonComponent } from "./shared/components/auth-button.component";
import { AuthGuard, AuthHttpInterceptor, AuthModule, HttpMethod } from "@auth0/auth0-angular";
import { AdminComponent } from "./admin/admin.component";
import { AdminAuthGuardService } from "./services/admin-auth-guard.service";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    VehicleFormComponent,
    VehicleListComponent,
    PaginationComponent,
    VehicleViewComponent,
    TabsComponent,
    TabComponent,
    CardTabsComponent,
    VehicleViewBasicsTabContentComponent,
    VehicleViewPhotosTabContentComponent,
    AuthButtonComponent,
    RangePipe,
    AdminComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    HttpClientModule,
    FontAwesomeModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'vehicles', pathMatch: 'full' },
      { path: 'vehicles/new', component: VehicleFormComponent, canActivate: [AuthGuard] },
      { path: 'vehicles/edit/:id', component: VehicleFormComponent, canActivate: [AuthGuard] },
      { path: 'vehicles/:id', component: VehicleViewComponent },
      { path: 'vehicles', component: VehicleListComponent },
      {
        path: 'admin',
        component: AdminComponent,
        canActivate: [AdminAuthGuardService]
      }
    ], { relativeLinkResolution: 'legacy' }),
    ReactiveFormsModule,
    FormsModule,
    AuthModule.forRoot({
      ...env.auth,
      httpInterceptor: {
        allowedList: [
          // /api/vehicles: POST request method to create vehicles needs tob authorized.
          {
            uri: '/api/vehicles',
            httpMethod: 'POST',
          },
          // /api/vehicles/*: POST, PUT, DELETE request methods need to be authorized.
          // including the request to upload and delete photos
          {
            uri: '/api/vehicles/*',
            httpMethod: HttpMethod.Put
          },
          {
            uri: '/api/vehicles/*',
            httpMethod: HttpMethod.Delete
          },
          {
            uri: '/api/vehicles/*',
            httpMethod: HttpMethod.Post
          },
        ],
      }
    })
  ],
  providers: [
    {
      provide: ErrorHandler,
      useClass: AppErrorHandler
    },
    { provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
