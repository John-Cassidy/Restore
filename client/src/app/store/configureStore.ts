import { counterReducer } from '../../features/contact/counterReducer';
import { createStore } from 'redux';

export const configureStore = () => {
  return createStore(counterReducer);
};
