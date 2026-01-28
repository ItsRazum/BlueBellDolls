export const useLitterApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase;
  const route = useRoute();
  const nuxtApp = useNuxtApp();

  const getById = (
    idSource: MaybeRefOrGetter<number | undefined>,
    initialDataSource: MaybeRefOrGetter<LitterDetailDto | null | undefined> = undefined,
  ) => {
    const key = computed(() => {
      const id = toValue(idSource);
      return id ? `litter-${id}` : null;
    });

    return useAsyncData(
      () => key.value,
      async () => {
        const initialData = toValue(initialDataSource);
        if (initialData) return initialData;

        const id = toValue(idSource);
        if (id) {
          return await $fetch<LitterDetailDto>(`/api/litters/${id}`, {
            baseURL: apiBase,
          });
        }
        return null;
      },
      {
        watch: [() => toValue(idSource)],
        getCachedData: (key) => {
          if (toValue(initialDataSource)) return;
          return nuxtApp.payload.data[key] || nuxtApp.static.data[key];
        },
      },
    );
  };

  const getByPage = () => {
    const page = () => route.query.page || "1";

    const key = `litters-page-${page()}`;

    return useAsyncData(
      key,
      async () => {
        return await $fetch<PagedResult<LitterListDto>>(`/api/litters`, {
          baseURL: apiBase,
          query: {
            page: page(),
          },
        });
      },
      {
        watch: [() => route.query.page],
        lazy: true,
        getCachedData: (key) => nuxtApp.payload.data[key] || nuxtApp.static.data[key],
      },
    );
  };

  return {
    getById,
    getByPage,
  };
};
