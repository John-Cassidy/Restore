import { createAsyncThunk, createSlice, isAnyOf } from '@reduxjs/toolkit';

import { FieldValues } from 'react-hook-form';
import { IUser } from '../../app/models/user';
import { agent } from '../../app/api/agent';
import { router } from '../../app/router/Routes';
import { toast } from 'react-toastify';

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
    thunkAPI.dispatch(setUser(JSON.parse(localStorage.getItem('user')!)));
    try {
      const user = await agent.Account.current();
      localStorage.setItem('user', JSON.stringify(user));
      return user;
    } catch (error: any) {
      return thunkAPI.rejectWithValue(error.data);
    }
  },
  {
    condition: () => {
      if (!localStorage.getItem('user')) return false;
    },
  }
);

export const accountSlice = createSlice({
  name: 'account',
  initialState,
  reducers: {
    signOut: (state) => {
      localStorage.removeItem('user');
      state.user = null;
      router.navigate('/');
    },
    setUser: (state, action) => {
      state.user = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder.addCase(fetchCurrentUser.rejected, (state, action) => {
      console.log(action.payload);
      localStorage.removeItem('user');
      state.user = null;
      toast.error('Session expired - please log in again');
      router.navigate('/');
    });
    builder.addMatcher(
      isAnyOf(signInUser.fulfilled, fetchCurrentUser.fulfilled),
      (state, action) => {
        state.user = action.payload;
      }
    );
    builder.addMatcher(isAnyOf(signInUser.rejected), (state, action) => {
      console.log(action.payload);
    });
  },
});

export const { signOut, setUser } = accountSlice.actions;
