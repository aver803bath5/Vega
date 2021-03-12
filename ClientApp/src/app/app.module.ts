import { BrowserModule } from '@angular/platform-browser';
import { ErrorHandler, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { ToastrModule } from "ngx-toastr";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
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

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    VehicleFormComponent,
    VehicleListComponent,
    PaginationComponent,
    VehicleViewComponent,
    TabsComponent,
    TabComponent,
    CardTabsComponent,
    VehicleViewBasicsTabContentComponent,
    RangePipe,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    HttpClientModule,
    FontAwesomeModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'vehicles', pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'vehicles/new', component: VehicleFormComponent },
      { path: 'vehicles/edit/:id', component: VehicleFormComponent },
      { path: 'vehicles/:id', component: VehicleViewComponent },
      { path: 'vehicles', component: VehicleListComponent }
    ]),
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    {
      provide: ErrorHandler,
      useClass: AppErrorHandler
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
