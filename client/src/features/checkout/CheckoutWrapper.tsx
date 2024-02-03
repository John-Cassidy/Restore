import { CheckoutPage } from './CheckoutPage';
import { Elements } from '@stripe/react-stripe-js';
import { loadStripe } from '@stripe/stripe-js';

const stripePromise = loadStripe(
  'pk_test_51OfiQ1BurVzaOgd8q783rhJSLbnA9wXJ9njvxFgW26zkkO31Cal6BC9IiX92e1j2iSvgJBs9Y4fzGzmFPo8qZQ9W00LkGQFB36'
);

export const CheckoutWrapper = () => {
  return (
    <Elements stripe={stripePromise}>
      <CheckoutPage />
    </Elements>
  );
};
