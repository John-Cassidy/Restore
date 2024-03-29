import { Grid, Paper } from '@mui/material';
import { setPageNumber, setProductParams } from './catalogSlice';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';

import { AppPagination } from '../../app/components/AppPagination';
import { CheckboxButtons } from '../../app/components/CheckboxButtons';
import { LoadingComponent } from '../../app/layout/LoadingComponent';
import { ProductList } from './ProductList';
import { ProductSearch } from './ProductSearch';
import { RadioButtonGroup } from '../../app/components/RadioButtonGroup';
import { useProducts } from '../../app/hooks/useProducts';

const sortOptions = [
  { value: 'name', label: 'Alphabetical' },
  { value: 'priceDesc', label: 'Price - High to Low' },
  { value: 'priceAsc', label: 'Price - Low to High' },
];

export const Catalog = () => {
  const { products, filtersLoaded, brands, types, metaData } = useProducts();

  const { productParams } = useAppSelector((state) => state.catalog);
  const dispatch = useAppDispatch();

  if (!filtersLoaded) return <LoadingComponent message='Loading products...' />;

  return (
    <Grid container columnSpacing={4}>
      <Grid item xs={3}>
        <Paper sx={{ mb: 2 }}>
          <ProductSearch />
        </Paper>
        <Paper sx={{ mb: 2, p: 2 }}>
          <RadioButtonGroup
            options={sortOptions}
            selectedValue={productParams.orderBy}
            onChange={(e) =>
              dispatch(setProductParams({ orderBy: e.target.value }))
            }
          />
        </Paper>
        <Paper sx={{ mb: 2 }}>
          <CheckboxButtons
            items={brands}
            checked={productParams.brands}
            onChange={(items: string[]) =>
              dispatch(setProductParams({ brands: items }))
            }
          />
        </Paper>
        <Paper sx={{ mb: 2 }}>
          <CheckboxButtons
            items={types}
            checked={productParams.types}
            onChange={(items: string[]) =>
              dispatch(setProductParams({ types: items }))
            }
          />
        </Paper>
      </Grid>
      <Grid item xs={9}>
        <ProductList products={products} />
      </Grid>
      <Grid item xs={3}></Grid>
      <Grid item xs={9} sx={{ mb: 2 }}>
        {metaData && (
          <AppPagination
            metaData={metaData}
            onPageChange={(page: number) => dispatch(setPageNumber(page))}
          />
        )}
      </Grid>
    </Grid>
  );
};
