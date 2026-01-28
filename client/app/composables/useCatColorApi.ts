export const useCatColorApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase;
  const route = useRoute();
  const nuxtApp = useNuxtApp();

  const getById = (
    idSource: MaybeRefOrGetter<number | undefined>,
    initialDataSource: MaybeRefOrGetter<CatColorDetailDto | null | undefined> = undefined,
  ) => {
    const key = computed(() => {
      const id = toValue(idSource);
      return id ? `catcolor-${id}` : null;
    });

    return useAsyncData(
      () => key.value,
      async () => {
        const initialData = toValue(initialDataSource);
        if (initialData) {
          return initialData;
        }

        const id = toValue(idSource);
        if (id) {
          return await $fetch<CatColorDetailDto>(`/api/catcolors/${id}`, {
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

  const getByPage = async () => {
    const page = () => route.query.page || "1";

    if (!route.query.page) {
      await navigateTo(
        {
          path: route.path,
          query: { ...route.query, page: "1" },
        },
        { replace: true },
      );
    }

    const key = `catcolors-page-${page()}`;

    return useAsyncData(
      key,
      async () => {
        return await $fetch<PagedResult<CatColorListDto>>(`/api/catcolors`, {
          baseURL: apiBase,
          query: {
            page: page(),
          },
        });
      },
      {
        watch: [() => route.query.page],

        getCachedData: (key) => nuxtApp.payload.data[key] || nuxtApp.static.data[key],
      },
    );
  };

  return {
    getById,
    getByPage,
  };
};
