import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Image } from '../../shared/models/image.model';
import { PagedResult } from '../../shared/models/paged-result.model';

@Injectable({
  providedIn: 'root',
})
export class Images {
  constructor(private readonly http: HttpClient) { }

  getAlbumImages(albumId: string, page: number): Observable<PagedResult<Image>> {
    return this.http.get<PagedResult<Image>>(`${environment.apiUrl}/images/album/${albumId}?page=${page}`);
  }

  uploadImage(albumId: string, file: File): Observable<any> {
    const formData = new FormData();

    formData.append('albumId', albumId);
    formData.append('file', file);

    return this.http.post(`${environment.apiUrl}/images/upload`, formData);
  }

  deleteImage(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/images/${id}`);
  }

  likeImage(id: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/images/${id}/like`, {});
  }

  dislikeImage(id: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/images/${id}/dislike`, {});
  }
}
