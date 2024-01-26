import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
} from '@reduxjs/toolkit';

import { IPaginatedResponse } from '../../app/models/pagination';
import { IProduct } from '../../app/models/product';
import { RootState } from '@reduxjs/toolkit/query';
import { agent } from '../../app/api/agent';

const productsAdapter = createEntityAdapter<IProduct>();

export interface CatalogState {
  productsLoaded: boolean;
  status: string;
}

export const fetchProductsAsync = createAsyncThunk<
  IPaginatedResponse<IProduct>
>('catalog/fetchProductsAsync', async () => {
  try {
    return await agent.Catalog.list();
  } catch (error) {
    console.log(error);
  }
});

export const fetchProductAsync = createAsyncThunk<IProduct, number>(
  'catalog/fetchProductAsync',
  async (productId) => {
    try {
      const product = await agent.Catalog.details(productId);
      return product;
    } catch (error) {
      console.log(error);
    }
  }
);

export const catalogSlice = createSlice({
  name: 'catalog',
  initialState: productsAdapter.getInitialState({
    productsLoaded: false,
    status: 'idle',
  }),
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchProductsAsync.pending, (state) => {
        state.status = 'pendingFetchProducts';
      })
      .addCase(fetchProductsAsync.rejected, (state) => {
        state.status = 'idle';
      })
      .addCase(fetchProductsAsync.fulfilled, (state, action) => {
        productsAdapter.setAll(state, action.payload.data);
        state.status = 'idle';
        state.productsLoaded = true;
      })
      .addCase(fetchProductAsync.pending, (state) => {
        state.status = 'pendingFetchProduct';
      })
      .addCase(fetchProductAsync.rejected, (state) => {
        state.status = 'idle';
      })
      .addCase(fetchProductAsync.fulfilled, (state, action) => {
        productsAdapter.upsertOne(state, action.payload);
        state.status = 'idle';
      });
  },
});

export const productsSelectors = productsAdapter.getSelectors(
  (state: RootState) => state.catalog
);
