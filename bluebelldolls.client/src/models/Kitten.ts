import type { ICat }  from '@/interfaces/ICat.ts';
import type { KittenStatus } from '@/enums/KittenStatus.ts';
import type { KittenClass } from '@/enums/KittenClass.ts';

export type Kitten = ICat & {
  Class: KittenClass;
  Status: KittenStatus;
  LitterId: number;
}
