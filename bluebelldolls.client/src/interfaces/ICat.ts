export interface ICat {
  id: number;
  name: string;
  birthDate: string;
  isMale: boolean;
  color: string;
  description: string;
  photos: Map<string, string>;
}
