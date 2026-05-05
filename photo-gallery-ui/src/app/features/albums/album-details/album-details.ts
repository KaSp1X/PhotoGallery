import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { Pagination } from '../../../shared/components/pagination/pagination';
import { Images } from '../../../core/services/images';
import { Auth } from '../../../core/services/auth';
import { Image } from '../../../shared/models/image.model';

@Component({
  selector: 'app-album-details',
  standalone: true,
  imports: [CommonModule, Pagination],
  templateUrl: './album-details.html',
  styleUrl: './album-details.scss',
})
export class AlbumDetails implements OnInit {
  images: Image[] = [];
  albumId = '';
  currentPage = 1;
  totalPages = 1;
  isLoading = false;
  isAuthenticated = false;
  selectedImage: string | null = null;
  selectedFile: File | null = null;

  constructor(private readonly route: ActivatedRoute, private readonly imagesService: Images, private readonly authService: Auth, private readonly changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.albumId = this.route.snapshot.paramMap.get('id')!;
    this.isAuthenticated = this.authService.isAuthenticated();
    this.loadImages();
  }

  loadImages(page = 1): void {
    this.isLoading = true;
    this.imagesService.getAlbumImages(this.albumId, page).subscribe(result => {
      this.images = result.items;
      this.currentPage = result.page;
      this.totalPages = result.totalPages;
      this.isLoading = false;
      this.changeDetector.markForCheck();
    });
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  uploadImage(): void {
    if (!this.selectedFile) return;

    this.imagesService.uploadImage(this.albumId, this.selectedFile).subscribe(() => {
      this.selectedFile = null;
      this.loadImages();
    });
  }

  deleteImage(id: string): void {
    const confirmed = confirm('Delete image?');

    if (!confirmed) return;

    this.imagesService.deleteImage(id).subscribe(() => { this.loadImages(this.currentPage); });
  }

  likeImage(id: string): void {
    this.imagesService.likeImage(id).subscribe(() => { this.loadImages(this.currentPage); });
  }

  dislikeImage(id: string): void {
    this.imagesService.dislikeImage(id).subscribe(() => { this.loadImages(this.currentPage); });
  }

  openImage(imagePath: string): void {
    this.selectedImage = imagePath;
  }

  closeModal(): void {
    this.selectedImage = null;
  }
}
