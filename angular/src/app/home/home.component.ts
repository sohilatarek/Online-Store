import { Component, inject } from '@angular/core';
import { AuthService, LocalizationPipe } from '@abp/ng.core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [LocalizationPipe, RouterModule, CommonModule]
})
export class HomeComponent {
  private authService = inject(AuthService);

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated
  }

  login() {
    this.authService.navigateToLogin();
  }
}
