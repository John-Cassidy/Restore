// createBrowserRouter() from reat-router-dom

import { Navigate, createBrowserRouter } from 'react-router-dom';

import { AboutPage } from '../../features/about/AboutPage';
import { App } from '../layout/App';
import { BasketPage } from '../../features/basket/BasketPage';
import { Catalog } from '../../features/catalog/Catalog';
import { CheckoutWrapper } from '../../features/checkout/CheckoutWrapper';
import { ContactPage } from '../../features/contact/ContactPage';
import { Inventory } from '../../features/admin/Inventory';
import { Login } from '../../features/account/Login';
import NotFound from '../errors/NotFound';
import { Orders } from '../../features/orders/Orders';
import { ProductDetails } from '../../features/catalog/ProductDetails';
import { Register } from '../../features/account/Register';
import { RequireAuth } from './RequireAuth';
import ServerError from '../errors/ServerError';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      {
        // authenticated routes
        element: <RequireAuth />,
        children: [
          { path: '/checkout', element: <CheckoutWrapper /> },
          { path: '/orders', element: <Orders /> },
        ],
      },
      { path: '/inventory', element: <Inventory /> },
      {
        path: 'catalog',
        element: <Catalog />,
      },
      {
        path: 'catalog/:id',
        element: <ProductDetails />,
      },
      {
        path: 'about',
        element: <AboutPage />,
      },
      {
        path: 'contact',
        element: <ContactPage />,
      },
      { path: '/server-error', element: <ServerError /> },
      { path: '/not-found', element: <NotFound /> },
      { path: '/basket', element: <BasketPage /> },
      { path: '/login', element: <Login /> },
      { path: '/register', element: <Register /> },
      { path: '*', element: <Navigate replace to='/not-found' /> },
    ],
  },
]);
