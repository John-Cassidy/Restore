import { useEffect, useState } from 'react';

import { CheckoutPage } from './CheckoutPage';
import { Elements } from '@stripe/react-stripe-js';
import { LoadingComponent } from '../../app/layout/LoadingComponent';
import { agent } from '../../app/api/agent';
import { loadStripe } from '@stripe/stripe-js';
import { setBasket } from '../basket/basketSlice';
import { useAppDispatch } from '../../app/store/configureStore';

const stripePromise = loadStripe(
  'pk_test_51OfiQ1BurVzaOgd8q783rhJSLbnA9wXJ9njvxFgW26zkkO31Cal6BC9IiX92e1j2iSvgJBs9Y4fzGzmFPo8qZQ9W00LkGQFB36'
);

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
