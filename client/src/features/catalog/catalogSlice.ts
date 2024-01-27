import { IMetaData, PaginatedResponse } from '../../app/models/pagination';
import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
} from '@reduxjs/toolkit';

import { IProduct } from '../../app/models/product';
import { IProductParams } from '../../app/models/productParams';
import { RootState } from '@reduxjs/toolkit/query';
import { agent } from '../../app/api/agent';

const productsAdapter = createEntityAdapter<IProduct>();

export interface CatalogState {
  productsLoaded: boolean;
  filtersLoaded: boolean;
  status: string;
  brands: string[];
  types: string[];
  productParams: IProductParams;
  metaData: IMetaData | null;
}

const getAxiosParams = (params: IProductParams) => {
  const axiosParams = new URLSearchParams();
  axiosParams.append('pageNumber', params.pageNumber.toString());
  axiosParams.append('pageSize', params.pageSize.toString());
  axiosParams.append('orderBy', params.orderBy);

  params.searchTerm && axiosParams.append('searchTerm', params.searchTerm);

  params.brands &&
    params.brands.length > 0 &&
    axiosParams.append('brands', params.brands.join(','));

  params.types &&
    params.types.length > 0 &&
    axiosParams.append('types', params.types.join(','));

  return axiosParams;
};

export const fetchProductsAsync = createAsyncThunk<
  IProduct[],
  void,
  { state: RootState }
>('catalog/fetchProductsAsync', async (_, thunkAPI) => {
  const params = getAxiosParams(thunkAPI.getState().catalog.productParams);
  try {
    const response = await agent.Catalog.list(params);
    thunkAPI.dispatch(setMetaData(response.metaData));
    return response.data;
  } catch (error: any) {
    return thunkAPI.rejectWithValue({ error: error.data });
  }
});

export const fetchProductAsync = createAsyncThunk<IProduct, number>(
  'catalog/fetchProductAsync',
  async (productId, thunkAPI) => {
    try {
      const product = await agent.Catalog.details(productId);
      return product;
    } catch (error: any) {
      return thunkAPI.rejectWithValue({ error: error.data });
    }
  }
);

export const fetchFiltersAsync = createAsyncThunk(
  'catalog/fetchFiltersAsync',
  async (_, thunkAPI) => {
    try {
      const filters = await agent.Catalog.filters();
      return filters;
    } catch (error: any) {
      return thunkAPI.rejectWithValue({ error: error.data });
    }
  }
);

const initParams = (): IProductParams => {
  return {
    pageNumber: 1,
    pageSize: 6,
    orderBy: 'name',
    brands: [],
    types: [],
  };
};

export const catalogSlice = createSlice({
  name: 'catalog',
  initialState: productsAdapter.getInitialState({
    productsLoaded: false,
    filtersLoaded: false,
    status: 'idle',
    brands: [],
    types: [],
    productParams: initParams(),
    metaData: null,
  }),
  reducers: {
    setProductParams: (state, action) => {
      state.productsLoaded = false;
      state.productParams = {
        ...state.productParams,
        ...action.payload,
        pageNumber: 1,
      };
    },
    resetProductParams: (state) => {
      state.productParams = initParams();
    },
    setPageNumber: (state, action) => {
      state.productsLoaded = false;
      state.productParams.pageNumber = action.payload;
    },
    setPageSize: (state, action) => {
      state.productParams.pageSize = action.payload;
    },
    setOrderBy: (state, action) => {
      state.productParams.orderBy = action.payload;
    },
    setBrands: (state, action) => {
      state.productParams.brands = action.payload;
    },
    setTypes: (state, action) => {
      state.productParams.types = action.payload;
    },
    setMetaData: (state, action) => {
      state.metaData = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchProductsAsync.pending, (state) => {
        state.status = 'pendingFetchProducts';
      })
      .addCase(fetchProductsAsync.rejected, (state, action) => {
        console.log(action?.payload);
        state.status = 'idle';
      })
      .addCase(fetchProductsAsync.fulfilled, (state, action) => {
        productsAdapter.setAll(state, action.payload);
        state.status = 'idle';
        state.productsLoaded = true;
      })
      .addCase(fetchProductAsync.pending, (state) => {
        state.status = 'pendingFetchProduct';
      })
      .addCase(fetchProductAsync.rejected, (state, action) => {
        console.log(action?.payload);
        state.status = 'idle';
      })
      .addCase(fetchProductAsync.fulfilled, (state, action) => {
        productsAdapter.upsertOne(state, action.payload);
        state.status = 'idle';
      })
      .addCase(fetchFiltersAsync.pending, (state) => {
        state.status = 'pendingFetchFilters';
      })
      .addCase(fetchFiltersAsync.rejected, (state, action) => {
        console.log(action?.payload);
        state.status = 'idle';
      })
      .addCase(fetchFiltersAsync.fulfilled, (state, action) => {
        state.brands = action.payload.brands;
        state.types = action.payload.types;
        state.filtersLoaded = true;
        state.status = 'idle';
      });
  },
});

export const {
  setPageNumber,
  setProductParams,
  resetProductParams,
  setMetaData,
} = catalogSlice.actions;

export const productsSelectors = productsAdapter.getSelectors(
  (state: RootState) => state.catalog
);
