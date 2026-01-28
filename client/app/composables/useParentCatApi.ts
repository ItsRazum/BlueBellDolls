export const useParentCatApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase;
  const route = useRoute();
  const nuxtApp = useNuxtApp();

  const getById = (
    idSource: MaybeRefOrGetter<number | undefined>,
    initialDataSource: MaybeRefOrGetter<ParentCatDetailDto | null | undefined> = undefined,
  ) => {
    const key = computed(() => {
      const id = toValue(idSource);
      return id ? `parentcat-${id}` : null;
    });

    return useAsyncData(
      () => key.value,
      async () => {
        // 1. Пропсы
        const initialData = toValue(initialDataSource);
        if (initialData) return initialData;

        // 2. Сеть
        const id = toValue(idSource);
        if (id) {
          return await $fetch<ParentCatDetailDto>(`/api/parentcats/${id}`, {
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

  const getByPage = async (isMale: boolean) => {
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

    const key = `parentcats-page-${page()}-isMale-${isMale}`;

    return useAsyncData(
      key,
      async () => {
        return await $fetch<PagedResult<ParentCatListDto>>(`/api/parentcats`, {
          baseURL: apiBase,
          query: {
            page: page(),
            isMale: isMale,
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
