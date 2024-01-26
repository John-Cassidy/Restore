import { createSlice } from '@reduxjs/toolkit';

// create initial state
export interface CounterState {
  data: number;
  title: string;
}

const initialState: CounterState = {
  data: 42,
  title: 'Redux with redux toolkit example',
};

// create slice
const counterSlice = createSlice({
  name: 'counter',
  initialState,
  reducers: {
    increment(state, action) {
      state.data += action.payload;
    },
    decrement(state, action) {
      state.data -= action.payload;
    },
  },
});

// export actions
export const { increment, decrement } = counterSlice.actions;
