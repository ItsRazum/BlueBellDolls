import { PhotosType } from '../enums/enums.ts';

declare global {
    export interface PagedResult<T> {
        items: T[];
        pageNumber: number;
        pageSize: number;
        totalItems: number;
        totalPages: number;
    }

    export interface PhotoDto {
        id: number;
        url: string;
        type: PhotosType;
        isMain: boolean;
    }
}

