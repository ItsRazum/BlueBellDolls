import type { PhotoDto } from "./api";

declare global {
  export interface ParentCatDetailDto extends Cat {
    photos: PhotoDto[];
  }

  export interface ParentCatListDto extends Cat {
    mainPhotoUrl: string;
  }

  export interface ParentCatMinimalDto {
    id: number;
    name: string;
  }
}
