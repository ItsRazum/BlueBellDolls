export const useParentCatApi = () => {
    const config = useRuntimeConfig();
    const apiBase = config.public.apiBase;
    const route = useRoute();

    const getById = async (id: number) => {
        return await useFetch<KittenDetailDto>(`/api/parentcats/${id}`, {
            baseURL: apiBase,
            key: `parentcat-${id}`
        });
    }

    const getByPage = async () => {
        const page = () => route.query.page;
        const isMale = () =>  route.query.isMale;
        if (!page() || isMale() === undefined) {
            await navigateTo({
                path: route.path,
                query: {
                    ...route.query,
                    page: page() || '1',
                    isMale: isMale() === undefined ? 'true' : isMale(),
                }
            }, { replace: true });
        }
        return await useFetch<PagedResult<KittenListDto>>(`/api/parentcats`, {
            baseURL: apiBase,
            key: `parentcat-page-${page}-${isMale}`,
            query: {
                page: page(),
                isMale: isMale(),
            }
        });
    }

    return {
        getById,
        getByPage,
    }
}