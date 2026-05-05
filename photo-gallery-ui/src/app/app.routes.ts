import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth-guard';

import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { AlbumsList } from './features/albums/albums-list/albums-list';
import { MyAlbums } from './features/albums/my-albums/my-albums';
import { AlbumDetails } from './features/albums/album-details/album-details';

export const routes: Routes = [
  {
    path: '',
    component: AlbumsList
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'register',
    component: Register
  },
  {
    path: 'my-albums',
    component: MyAlbums,
    canActivate: [authGuard]
  },
  {
    path: 'albums/:id',
    component: AlbumDetails
  }
];
