import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Album } from '../../shared/models/album.model';
import { PagedResult } from '../../shared/models/paged-result.model';

@Injectable({
  providedIn: 'root',
})
export class Albums {
  constructor(private readonly http: HttpClient) { }

  getAlbums(page: number): Observable<PagedResult<Album>> {
    return this.http.get<PagedResult<Album>>(`${environment.apiUrl}/albums?page=${page}`);
  }

  getMyAlbums(page: number): Observable<PagedResult<Album>> {
    return this.http.get<PagedResult<Album>>(`${environment.apiUrl}/albums/my?page=${page}`);
  }

  createAlbum(title: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/albums`, { title });
  }

  deleteAlbum(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/albums/${id}`);
  }
}
