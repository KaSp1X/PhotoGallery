import { TestBed } from '@angular/core/testing';

import { Albums } from './albums';

describe('Albums', () => {
  let service: Albums;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Albums);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
