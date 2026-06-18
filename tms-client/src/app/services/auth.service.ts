import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, map } from 'rxjs';
import { Router } from '@angular/router';
import { ApiResponse, CurrentUser, LoginRequest, LoginResponse, RoleName, UpdateUserRequest, UserResponse } from '../models';

const API_BASE = 'http://localhost:5080/api';
const STORAGE_KEY = 'tms_current_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject = new BehaviorSubject<CurrentUser | null>(this.loadFromStorage());
  readonly currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  private loadFromStorage(): CurrentUser | null {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      if (stored) {
        const user: CurrentUser = JSON.parse(stored);
        if (new Date(user.tokenExpiry) > new Date()) {
          return user;
        }
        localStorage.removeItem(STORAGE_KEY);
      }
    } catch {
      localStorage.removeItem(STORAGE_KEY);
    }
    return null;
  }

  get currentUser(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  get isLoggedIn(): boolean {
    const user = this.currentUser;
    return !!user && new Date(user.tokenExpiry) > new Date();
  }

  get token(): string | null {
    return this.currentUser?.token ?? null;
  }

  get userRole(): RoleName | null {
    return this.currentUser?.roleName ?? null;
  }

  hasRole(roles: RoleName[]): boolean {
    const role = this.userRole;
    return !!role && roles.includes(role);
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<ApiResponse<LoginResponse>>(`${API_BASE}/auth/login`, credentials).pipe(
      map((res) => res.data),
      tap((data) => {
        const user: CurrentUser = {
          userId: data.userId,
          fullName: data.fullName,
          email: data.email,
          roleName: data.roleName,
          token: data.token,
          tokenExpiry: data.tokenExpiry,
        };
        localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
        this.currentUserSubject.next(user);
      })
    );
  }

  getProfile(): Observable<UserResponse> {
    return this.http.get<ApiResponse<UserResponse>>(`${API_BASE}/auth/profile`).pipe(
      map((res) => res.data)
    );
  }

  updateProfile(payload: UpdateUserRequest): Observable<UserResponse> {
    return this.http.put<ApiResponse<UserResponse>>(`${API_BASE}/auth/profile`, payload).pipe(
      map((res) => res.data),
      tap((profile) => this.updateStoredProfile(profile))
    );
  }

  private updateStoredProfile(profile: UserResponse): void {
    const currentUser = this.currentUser;
    if (!currentUser) return;

    const updatedUser: CurrentUser = {
      ...currentUser,
      fullName: `${profile.firstName} ${profile.lastName}`.trim(),
      email: profile.email,
      roleName: profile.roleName as RoleName,
    };

    localStorage.setItem(STORAGE_KEY, JSON.stringify(updatedUser));
    this.currentUserSubject.next(updatedUser);
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }
}
