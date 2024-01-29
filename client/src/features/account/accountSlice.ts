import { createAsyncThunk, createSlice, isAnyOf } from '@reduxjs/toolkit';

import { FieldValues } from 'react-hook-form';
import { IUser } from '../../app/models/user';
import { agent } from '../../app/api/agent';

interface AccountState {
  user: IUser | null;
}

const initialState: AccountState = {
  user: null,
};

export const signInUser = createAsyncThunk<IUser, FieldValues>(
  'account/signInUser',
  async (data, thunkAPI) => {
    try {
      const user = await agent.Account.login(data);
      localStorage.setItem('user', JSON.stringify(user));
      return user;
    } catch (error: any) {
      return thunkAPI.rejectWithValue(error.data);
    }
  }
);

export const fetchCurrentUser = createAsyncThunk<IUser>(
  'account/fetchCurrentUser',
  async (_, thunkAPI) => {
    try {
      const user = await agent.Account.current();
      localStorage.setItem('user', JSON.stringify(user));
      return user;
    } catch (error: any) {
      return thunkAPI.rejectWithValue(error.data);
    }
  }
);

export const accountSlice = createSlice({
  name: 'account',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder.addMatcher(
      isAnyOf(signInUser.fulfilled, fetchCurrentUser.fulfilled),
      (state, action) => {
        state.user = action.payload;
      }
    );
    builder.addMatcher(
      isAnyOf(signInUser.rejected, fetchCurrentUser.rejected),
      (state, action) => {
        console.log(action.payload);
      }
    );
  },
});
