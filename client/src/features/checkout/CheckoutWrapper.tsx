import { useEffect, useState } from 'react';

import { CheckoutPage } from './CheckoutPage';
import { Elements } from '@stripe/react-stripe-js';
import { LoadingComponent } from '../../app/layout/LoadingComponent';
import { agent } from '../../app/api/agent';
import { loadStripe } from '@stripe/stripe-js';
import { setBasket } from '../basket/basketSlice';
import { useAppDispatch } from '../../app/store/configureStore';

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY);

export const CheckoutWrapper = () => {
  const dispatch = useAppDispatch();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    agent.Payments.createPaymentIntent()
      .then((response) => {
        dispatch(setBasket(response));
      })
      .catch((error) => console.log(error))
      .finally(() => setLoading(false));
  }, [dispatch]);

  if (loading) return <LoadingComponent message='Loading checkout' />;

  return (
    <Elements stripe={stripePromise}>
      <CheckoutPage />
    </Elements>
  );
};
