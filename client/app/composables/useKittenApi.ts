export const useKittenApi = () => {
    const config = useRuntimeConfig();
    const apiBase = config.public.apiBase;
    const route = useRoute();
    const getById = async (id: number) => {
        return await useFetch<KittenDetailDto>(`/api/kittens/${id}`, {
            baseURL: apiBase,
            lazy: true,
            key: `kitten-${id}`
        });
    }

    const getAvailableKittens = async () => {
        return await useFetch<KittenListDto[]>(`/api/kittens`,
            {
                baseURL: apiBase,
                lazy: true,
                key: `kittens`
            });
    }

    const getByPage = async () => {
        const page = () => route.query.page;
        if (!page()) {
            await navigateTo({
                path: route.path,
                query: {
                    ...route.query,
                    page: '1'
                }
            }, { replace: true });
        }

        return await useFetch<PagedResult<KittenListDto>>(`/api/kittens`, {
            baseURL: apiBase,
            key: `kittens-page-${page()}`,
            lazy: true,
            query: {
                page: page(),
            }
        });
    }

    const bookKitten = async (customer: BookingDto) => {
        return await $fetch(`/api/bookingrequests/${customer.kittenId}`, {
            baseURL: apiBase,
            method: 'POST',
            lazy: true,
            body: customer,
        });
    }

    return {
        getById,
        getAvailableKittens,
        getByPage,
        bookKitten,
    }
}