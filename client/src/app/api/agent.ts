import axios, { AxiosError, AxiosResponse } from 'axios';

axios.defaults.baseURL = 'http://localhost:5000/api/';

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.response.use(
  (response) => {
    return response;
  },
  (error: AxiosError) => {
    console.log('caught by interceptors');
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
  getProductNotFound: () => requests.get('products/999'),
  //   getValidationError: () => requests.post('products', {}),
};

export const agent = {
  Catalog,
  TestErrors,
};
