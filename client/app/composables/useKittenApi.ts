export const useKittenApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase;
  const route = useRoute();
  const nuxtApp = useNuxtApp();

  const getById = (
    idSource: MaybeRefOrGetter<number | undefined>,
    initialDataSource: MaybeRefOrGetter<KittenDetailDto | null | undefined> = undefined,
  ) => {
    const key = computed(() => {
      const id = toValue(idSource);
      return id ? `kitten-${id}` : null;
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
          return await $fetch<KittenDetailDto>(`/api/kittens/${id}`, {
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

  const getAvailableKittens = async () => {
    return await useFetch<KittenListDto[]>(`/api/kittens`, {
      baseURL: apiBase,
      lazy: true,
      key: `kittens`,
    });
  };

  const getByPage = async () => {
    const page = () => route.query.page;
    if (!page()) {
      await navigateTo(
        { path: route.path, query: { ...route.query, page: "1" } },
        { replace: true },
      );
    }
    return await useFetch<PagedResult<KittenListDto>>(`/api/kittens`, {
      baseURL: apiBase,
      key: `kittens-page-${page()}`,
      lazy: true,
      query: { page: page() },
    });
  };

  const bookKitten = async (customer: CreateBookingRequestDto) => {
    return await $fetch(`/api/bookingrequests/`, {
      baseURL: apiBase,
      method: "POST",
      body: customer,
    });
  };

  return {
    getById,
    getAvailableKittens,
    getByPage,
    bookKitten,
  };
};
