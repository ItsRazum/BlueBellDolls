import type { PhotoDto } from "./api";
import {KittenClass, KittenStatus} from "../enums/enums";

declare global {
    export interface KittenDetailDto extends Cat {
        class: KittenClass;
        status: KittenStatus;
        photos: PhotoDto[];
        litterId: number;
        litterLetter: string;
    }

    export interface KittenListDto extends Cat {
        class: KittenClass;
        status: KittenStatus;
        mainPhotoUrl: string;
        litterId: number;
        litterLetter: string;
    }
}
