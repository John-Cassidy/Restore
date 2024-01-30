import { IBasket, IBasketItem } from '../../app/models/basket';
import { createAsyncThunk, createSlice, isAnyOf } from '@reduxjs/toolkit';

import { agent } from '../../app/api/agent';
import { getCookie } from '../../app/util/util';

// create initial state
interface BasketState {
  basket: IBasket | null;
  status: string;
}

const initialState: BasketState = {
  basket: null,
  status: 'idle',
};

export const fecthBasketAsync = createAsyncThunk<IBasket>(
  'basket/fetchBasketAsync',
  async (_, thunkAPI) => {
    try {
      return await agent.Basket.get();
    } catch (error: any) {
      return thunkAPI.rejectWithValue({ error: error.data });
    }
  },
  {
    condition: () => {
      if (!getCookie('buyerId')) return false;
    },
  }
);

export const addBasketItemAsync = createAsyncThunk<
  IBasket,
  { productId: number; quantity?: number }
>(
  'basket/addBasketItemAsync',
  async ({ productId, quantity = 1 }, thunkAPI) => {
    try {
      return await agent.Basket.addItem(productId, quantity);
    } catch (error: any) {
      return thunkAPI.rejectWithValue({ error: error.data });
    }
  }
);

export const removeBasketItemAsync = createAsyncThunk<
  void,
  { productId: number; quantity: number; name?: string }
>('basket/removeBasketItemAsync', async ({ productId, quantity }, thunkAPI) => {
  try {
    await agent.Basket.removeItem(productId, quantity);
  } catch (error: any) {
    return thunkAPI.rejectWithValue({ error: error.data });
  }
});

export const basketSlice = createSlice({
  name: 'basket',
  initialState,
  reducers: {
    setBasket: (state, action) => {
      state.basket = action.payload;
    },
    clearBasket: (state) => {
      state.basket = null;
    },
  },
  extraReducers: (builder) => {
    builder.addCase(addBasketItemAsync.pending, (state, action) => {
      state.status = 'pendingAddItem' + action.meta.arg.productId;
    });
    builder
      .addCase(removeBasketItemAsync.pending, (state, action) => {
        state.status =
          'pendingRemoveItem' +
          action.meta.arg.productId +
          action.meta.arg.name;
      })
      .addCase(removeBasketItemAsync.rejected, (state, action) => {
        console.log(action.payload);
        state.status = 'idle';
      })
      .addCase(removeBasketItemAsync.fulfilled, (state, action) => {
        const { productId, quantity } = action.meta.arg;
        const itemIndex = state.basket?.items.findIndex(
          (x: IBasketItem) => x.productId === productId
        );
        if (itemIndex === -1 || itemIndex === undefined) return;
        state.basket!.items[itemIndex!].quantity -= quantity;
        if (state.basket!.items[itemIndex!].quantity <= 0)
          state.basket!.items.splice(itemIndex!, 1);
        state.status = 'idle';
      });
    builder.addMatcher(
      isAnyOf(addBasketItemAsync.rejected, fecthBasketAsync.rejected),
      (state, action) => {
        console.log(action.payload);
        state.status = 'idle';
      }
    );
    builder.addMatcher(
      isAnyOf(addBasketItemAsync.fulfilled, fecthBasketAsync.fulfilled),
      (state, action) => {
        state.status = 'idle';
        state.basket = action.payload;
      }
    );
  },
});

export const { setBasket, clearBasket } = basketSlice.actions;
