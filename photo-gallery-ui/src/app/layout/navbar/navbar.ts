import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

import { Auth } from '../../core/services/auth';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  isAuthenticated = false;

  constructor(public authService: Auth, private router: Router) {
    this.authService.isAuthenticated$.subscribe(value => { this.isAuthenticated = value; });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
