import axios, { AxiosError, AxiosResponse } from 'axios';

import { toast } from 'react-toastify';

axios.defaults.baseURL = 'http://localhost:5000/api/';

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.response.use(
  (response) => {
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
        toast.error(data.title);
        break;
      default:
        break;
    }

    return Promise.reject(error.response as AxiosResponse);
  }
);

const requests = {
  get: (url: string) => axios.get(url).then(responseBody),
  post: (url: string, body: object) => axios.post(url, body).then(responseBody),
  put: (url: string, body: object) => axios.put(url, body).then(responseBody),
  del: (url: string) => axios.delete(url).then(responseBody),
};

const Catalog = {
  list: () => requests.get('products?PageNumber=1&PageSize=30'),
  details: (id: number) => requests.get(`products/${id}`),
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
  Catalog,
  TestErrors,
};