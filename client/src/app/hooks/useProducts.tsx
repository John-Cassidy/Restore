import {
  fetchFiltersAsync,
  fetchProductsAsync,
  productsSelectors,
} from '../../features/catalog/catalogSlice';
import { useAppDispatch, useAppSelector } from '../store/configureStore';

import { useEffect } from 'react';

export const useProducts = () => {
  const products = useAppSelector(productsSelectors.selectAll);
  const {
    productsLoaded,
    filtersLoaded,
    brands,
    types,
    productParams,
    metaData,
  } = useAppSelector((state) => state.catalog);
  const dispatch = useAppDispatch();

  useEffect(() => {
    if (!productsLoaded) dispatch(fetchProductsAsync());
  }, [productsLoaded, dispatch]);

  useEffect(() => {
    if (!filtersLoaded) dispatch(fetchFiltersAsync());
  }, [filtersLoaded, dispatch]);

  return {
    products,
    filtersLoaded,
    brands,
    types,
    productParams,
    metaData,
    dispatch,
  };
};
