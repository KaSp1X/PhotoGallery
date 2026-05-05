import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { environment } from '../../../environments/environment';
import { AuthResponse } from '../../shared/models/auth-response.model';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private readonly tokenKey = 'token';
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());

  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private readonly http: HttpClient) {
    this.isAuthenticatedSubject.next(this.hasToken());
  }

  login(data: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, data);
  }

  register(data: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, data);
  }

  saveToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
    this.isAuthenticatedSubject.next(true);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.isAuthenticatedSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  getUserRoles(): string[] {
    const token = this.getToken();

    if (!token) return [];

    const decoded: any = jwtDecode(token);
    const roles = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (!roles) return [];

    return Array.isArray(roles) ? roles : [roles];
  }

  isAdmin(): boolean {
    return this.getUserRoles().includes('Admin');
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}
