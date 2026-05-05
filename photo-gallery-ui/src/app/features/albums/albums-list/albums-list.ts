import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { Auth } from '../../../core/services/auth';
import { Albums } from '../../../core/services/albums';
import { Album } from '../../../shared/models/album.model';
import { PagedResult } from '../../../shared/models/paged-result.model';
import { Pagination } from '../../../shared/components/pagination/pagination';

@Component({
  selector: 'app-albums-list',
  standalone: true,
  imports: [CommonModule, RouterModule, Pagination],
  templateUrl: './albums-list.html',
  styleUrl: './albums-list.scss',
})
export class AlbumsList implements OnInit {
  albums: Album[] = [];
  currentPage = 1;
  totalPages = 1;
  isLoading = false;
  isAdmin = false;

  constructor(private readonly albumsService: Albums, private readonly authService: Auth, private readonly changeDetector: ChangeDetectorRef) {
    this.isAdmin = this.authService.isAdmin();
  }

  ngOnInit(): void {
    this.loadAlbums();
  }

  loadAlbums(page = 1): void {
    this.isLoading = true;
    this.albumsService.getAlbums(page).subscribe({
      next: (result: PagedResult<Album>) => {
        this.albums = result.items;
        this.currentPage = result.page;
        this.totalPages = result.totalPages;
        this.isLoading = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  deleteAlbum(id: string): void {
    const confirmed = confirm('Delete this album?');

    if (!confirmed) return;

    this.albumsService.deleteAlbum(id).subscribe(() => {
      this.loadAlbums(this.currentPage);
    });
  }
}
