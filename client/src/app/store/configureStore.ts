import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

import { basketSlice } from '../../features/basket/basketSlice';
import { configureStore } from '@reduxjs/toolkit';
import { counterSlice } from '../../features/contact/counterSlice';

export const store = configureStore({
  reducer: {
    counter: counterSlice.reducer,
    basket: basketSlice.reducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
