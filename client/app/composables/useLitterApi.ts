export const useLitterApi = () => {
    const config = useRuntimeConfig();
    const apiBase = config.public.apiBase;
    const route = useRoute();

    const getById = async (id: number) => {
        return await useFetch<KittenDetailDto>(`/api/litters/${id}`, {
            baseURL: apiBase,
            key: `litter-${id}`
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

        return await useFetch<PagedResult<KittenListDto>>(`/api/litters`, {
            baseURL: apiBase,
            key: `litters-page-${page()}`,
            lazy: true,
            query: {
                page: page()
            }
        });
    }

    return {
        getById,
        getByPage,
    }
}