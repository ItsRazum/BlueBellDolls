export const useFeedbackRequestApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase;

  const sendRequest = async (customer: CreateFeedbackRequestDto) => {
    return await $fetch(`/api/feedbackrequests/`, {
      baseURL: apiBase,
      method: "POST",
      body: customer,
    });
  };

  return {
    sendRequest,
  };
};
