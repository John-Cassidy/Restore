import { TextField, debounce } from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';

import React from 'react';
import { setProductParams } from './catalogSlice';

export const ProductSearch = () => {
  const { productParams } = useAppSelector((state) => state.catalog);
  const dispatch = useAppDispatch();
  const [searchTerm, setSearchTerm] = React.useState(productParams.searchTerm);

  // create material ui debounce hook
  const debouncedSearch = debounce((event: any) => {
    dispatch(
      setProductParams({ ...productParams, searchTerm: event.target.value })
    );
  }, 1000);

  return (
    <TextField
      label='Search products'
      variant='outlined'
      fullWidth
      value={searchTerm}
      onChange={(event) => {
        setSearchTerm(event.target.value);
        debouncedSearch(event);
      }}
    />
  );
};
