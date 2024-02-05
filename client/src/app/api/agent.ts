import axios, { AxiosError, AxiosResponse } from 'axios';

import { router } from '../router/Routes';
import { store } from '../store/configureStore';
import { toast } from 'react-toastify';

const sleep = () => new Promise((resolve) => setTimeout(resolve, 100));

axios.defaults.baseURL = import.meta.env.VITE_API_URL;
axios.defaults.withCredentials = true;

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.request.use((config) => {
  const token = store.getState().account.user?.token;
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

axios.interceptors.response.use(
  async (response) => {
    if (import.meta.env.DEV) await sleep();
    const pagination = response.headers['pagination'];
    if (pagination) {
      response.data = { ...response.data, metaData: JSON.parse(pagination) };
      // response.data = new PaginatedResponse(
      //   response.data,
      //   JSON.parse(pagination)
      // );
      return response;
    }

    return response;
  },
  (error: AxiosError) => {
    const { data, status } = error.response as AxiosResponse;
    switch (status) {
      case 400:
        // // option 1
        // if (data.detail) {
        //   const modelStateErrors: Record<string, string[]> = JSON.parse(
        //     data.detail
        //   );
        //   throw modelStateErrors;
        // }
        // // option 2
        // if (data.detail) {
        //   const modelStateErrors: string[] = [];
        //   for (const key in data.detail) {
        //     if (data.detail[key]) {
        //       modelStateErrors.push(data.detail[key]);
        //     }
        //   }
        //   throw modelStateErrors.flat();
        // }
        toast.error(data.title);
        break;
      case 401:
        toast.error(data.title);
        break;
      case 404:
        toast.error(data.title);
        break;
      case 500:
        router.navigate('/server-error', { state: { error: data } });
        break;
      default:
        break;
    }

    return Promise.reject(error.response as AxiosResponse);
  }
);

const requests = {
  get: (url: string, params?: URLSearchParams) =>
    axios.get(url, { params }).then(responseBody),
  post: (url: string, body: object) => axios.post(url, body).then(responseBody),
  put: (url: string, body: object) => axios.put(url, body).then(responseBody),
  delete: (url: string) => axios.delete(url).then(responseBody),
};

const Account = {
  login: (values: any) => requests.post('account/login', values),
  register: (values: any) => requests.post('account/register', values),
  current: () => requests.get('account/current'),
  fetchAddress: () => requests.get('account/address'),
};

const Basket = {
  get: () => requests.get('basket'),
  addItem: (productId: number, quantity = 1) =>
    requests.post(`basket?productId=${productId}&quantity=${quantity}`, {}),
  removeItem: (productId: number, quantity = 1) =>
    requests.delete(`basket?productId=${productId}&quantity=${quantity}`),
};

const Catalog = {
  list: (params: URLSearchParams) => requests.get('products', params),
  details: (id: number) => requests.get(`products/${id}`),
  filters: () => requests.get('products/filters'),
};

const Orders = {
  list: () => requests.get('orders'),
  fetch: (id: number) => requests.get(`orders/${id}`),
  create: (values: any) => requests.post('orders', values),
};

const Payments = {
  createPaymentIntent: () => requests.post('payments', {}),
};

const TestErrors = {
  get400HttpError: () => requests.get('throwBadHttpRequest'),
  get400Error: () => requests.get('throwBadRequest'),
  get401Error: () => requests.get('throwUnauthorized'),
  get404Error: () => requests.get('throwNotFound'),
  get500Error: () => requests.get('throwException'),
  getValidationError: () => requests.get('validation-error'),
  getProductNotFound: () => requests.get('products/0'),
};

export const agent = {
  Account,
  Basket,
  Catalog,
  Orders,
  Payments,
  TestErrors,
};
