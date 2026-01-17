export const useCatColorApi = () => {
    const config = useRuntimeConfig();
    const apiBase = config.public.apiBase;
    const route = useRoute();

    const getById = async (id: number) => {
        return await useFetch<KittenDetailDto>(`/api/catcolors/${id}`, {
            baseURL: apiBase,
            key: `catcolor-${id}`
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

        return await useFetch<PagedResult<KittenListDto>>(`/api/catcolors`, {
            baseURL: apiBase,
            key: `catcolors-page-${page()}`,
            query: {
                page: page(),
            }
        });
    }

    return {
        getById,
        getByPage,
    }
}