import {BookingDto} from "~~/types/api";

export const useKittenApi = () => {
    const config = useRuntimeConfig();
    const apiBase = config.public.apiBase;

    const getById = async (id: number) => {
        return await $fetch(`${apiBase}/kittens/${id}`, {
            baseURL: apiBase,
        });
    }

    const bookKitten = async (id: number, customer: BookingDto) => {
        return await $fetch(`${apiBase}/booking/${id}`, {})
    }
}