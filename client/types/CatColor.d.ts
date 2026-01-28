declare global {
  export interface CatColorMinimalDto {
    id: number;
    identifier: string;
  }

  export interface CatColorListDto {
    id: number;
    identifier: string;
    description: string;
    mainPhotoUrl: string;
  }

  export interface CatColorDetailDto {
    id: number;
    identifier: string;
    description: string;
    photos: PhotoDto[];
  }
}
