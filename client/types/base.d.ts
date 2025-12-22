import type { PhotoDto } from "./api";
import type {KittenClass, KittenStatus} from "../enums/enums";

declare global {
    export abstract class Cat {
        id: number;
        name: string;
        birthDay: string;
        description: string;
        isMale: boolean;
        color: CatColorDto;
    }

    export abstract class Kitten extends Cat {
        class: KittenClass;
        status: KittenStatus;
        litterId: number;
        litterLetter: string;
    }

    export interface CatColorDto {
        id: number;
        identifier: string;
        description: string;
        photos: PhotoDto[];
    }
}