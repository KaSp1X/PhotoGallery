import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Albums } from '../../../core/services/albums';
import { Album } from '../../../shared/models/album.model';
import { Pagination } from '../../../shared/components/pagination/pagination';

@Component({
  selector: 'app-my-albums',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, Pagination],
  templateUrl: './my-albums.html',
  styleUrl: './my-albums.scss',
})
export class MyAlbums implements OnInit {
  private readonly fb: FormBuilder = inject(FormBuilder);

  albums: Album[] = [];
  currentPage = 1;
  totalPages = 1;
  isLoading = false;
  form = this.fb.group({
    title: ['', Validators.required]
  });

  constructor(private readonly albumsService: Albums, private readonly changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadAlbums();
  }

  loadAlbums(page = 1): void {
    this.isLoading = true;
    this.albumsService.getMyAlbums(page).subscribe(result => {
      this.albums = result.items;
      this.currentPage = result.page;
      this.totalPages = result.totalPages;
      this.isLoading = false;
      this.changeDetector.markForCheck();
    });
  }

  createAlbum(): void {
    if (this.form.invalid)
      return;

    const title = this.form.value.title!;

    this.albumsService.createAlbum(title).subscribe(() => {
      this.form.reset();
      this.loadAlbums();
    });
  }

  deleteAlbum(id: string): void {
    const confirmed = confirm('Delete this album?');

    if (!confirmed)
      return;

    this.albumsService.deleteAlbum(id).subscribe(() => {
      this.loadAlbums(this.currentPage);
    });
  }
}
