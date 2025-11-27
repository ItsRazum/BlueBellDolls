import type { PhotoDto } from "./api";
import type { KittenListDto } from "./Kitten";

declare global {
    export interface LitterDetailDto {
        id: number;
        letter: string;
        birthDay: string;
        description: string;
        photos: PhotoDto[];
        motherCatId: number;
        motherCat: ParentCatListDto;
        fatherCatId: number;
        fatherCat: ParentCatListDto;
        kittens: KittenListDto[];
    }
}