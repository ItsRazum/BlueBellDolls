import type { PhotoDto } from "./api";

declare global {
    export interface KittenDetailDto extends Kitten {
        photos: PhotoDto[];
    }

    export interface KittenListDto extends Kitten {
        mainPhotoUrl: string;
    }
}
