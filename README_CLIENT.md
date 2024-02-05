# Developer Notes

## Create React App using Vite

[Documentation](https://vitejs.dev/guide/)

```powershell

npm create vite@latest

√ Project name: ... client
√ Select a framework: » React
√ Select a variant: » TypeScript + SWC

Done. Now run:

  cd client
  npm install
  npm run start
```

## Packages

- Material UI: v4 - import {Container} from "@mui/material"
- Axios
- Redux
- Forms (React-hook-form)
- React-Router - version????

### Material UI

[Documentation](https://mui.com/material-ui/)

```powershell
npm install @mui/material @emotion/react @emotion/styled

npm install @fontsource/roboto

npm install @mui/icons-material, @mui/lab

```

### React Router

[Documentation React Router v6+](https://reactrouter.com/en/main/start/overview)

```powershell
npm i react-router-dom
```

### Axios

HTTP Client

```powershell
npm install axios
```

### react-toastify

[NPM](https://www.npmjs.com/package/react-toastify)
[Playground](https://fkhadra.github.io/react-toastify/introduction/)
[Using React-Toastify to style your toast messages](https://blog.logrocket.com/using-react-toastify-style-toast-messages/)

```powershell
npm i react-toastify
```

## Translate C# Class to Json to TypeScript

[Json To C#](https://json2csharp.com/)
[C# To Json](https://csharp2json.azurewebsites.net/)
[Json To Typescript](https://json2ts.vercel.app/)

## Linting

```powershell
npm run lint
```

## Shopping Cart

## Redux

This section implements state management with the following:

- [Redux](https://redux.js.org/)
- React-Redux
- [Redux Toolkit](https://redux-toolkit.js.org/)
- Redux Dev tools

```powershell

npm i redux react-redux
npm i @reduxjs/toolkit

npm

```

## Identity

- Setting up ASP.NET Identity
- Using EF with Identity
- JWT Tokens
- Using Forms in React
- Validating form inputs
- App initialization
- Transfering anonymous basket to logged in user

## React Hook Form

[Documentation](https://react-hook-form.com/)

[useForm API](https://react-hook-form.com/docs/useform)

```powershell
npm i react-hook-form
```

[Schema-based form validation with Yup](https://github.com/jquense/yup)

```powershell
npm i @hookform/resolvers yup
```

## Checkout

Create a Material UI checkout page using the following [Material UI Template](https://github.com/mui/material-ui/tree/v5.15.7/docs/data/material/getting-started/templates/checkout)

View the [Demo](https://mui.com/material-ui/getting-started/templates/checkout/)

## Payments

### Stripe Setup on client with React

[Stripe React Documenation](https://stripe.com/docs/stripe-js/react)

React Stripe.js is a thin wrapper around Stripe Elements. It allows you to add Elements to any React app.

```powershell
npm i --save @stripe/react-stripe-js @stripe/stripe-js
```

Setup [Elements Provider](https://stripe.com/docs/stripe-js/react#elements-provider) and wrap CheckoutPage with CheckoutWrapper

Create [Element Component](https://stripe.com/docs/stripe-js/react#element-components) StripeInput and use it in PaymentForm for credit card input fields

Copilot provided implementation that is slightly different from project implementation.

```tsx
import { InputBaseComponentProps } from '@mui/material';
import React, { forwardRef, useImperativeHandle, useRef } from 'react';

interface Props extends InputBaseComponentProps {}

export const StripeInput = forwardRef<unknown, Props>((props, ref) => {
  const { component: Component, ...otherProps } = props;
  const elementRef = useRef<any>();

  useImperativeHandle(ref, () => ({
    focus: () => elementRef.current.focus,
  }));

  return (
    <Component
      onReady={(element: any) => (elementRef.current = element)}
      {...otherProps}
    />
  );
});
```

## Publishing

In this section:

- Create a Production BUild of the React App
- Host the React app on the API (Kestrel) Server
- Switch Database server to PostGreSQL
- \*Setup and configure Heroku (no longer free to use)

  - Publish to alternative cloud provider

### Homepage

[Add Slider to Homepage - React Slick](https://react-slick.neostack.com/)

```powershell
npm i react-slick @types/react-slick slick-carousel
```
