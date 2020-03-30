import { TopNavComponent } from './layout/top-nav/top-nav.component';
import { SideNavComponent } from './layout/side-nav/side-nav.component';
import { RegisterComponent } from './modules/account/register/register.component';
import { LoginComponent } from './modules/account/login/login.component';
import { HomeComponent } from './modules/home/home.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialForReactivities } from './angular-material.module';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    SideNavComponent,
    TopNavComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AngularMaterialForReactivities
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
