import { Grid } from '@mui/material';
import { IProduct } from '../../app/models/product';
import { ProductCard } from './ProductCard';
import { ProductCardSkeleton } from './ProductCardSkeleton';
import { useAppSelector } from '../../app/store/configureStore';

interface IProps {
  products: IProduct[];
}

export const ProductList = ({ products }: IProps) => {
  const { productsLoaded } = useAppSelector((state) => state.catalog);

  return (
    <Grid container spacing={4}>
      {products.map((product) => (
        <Grid item xs={4} key={product.id}>
          {!productsLoaded ? (
            <ProductCardSkeleton />
          ) : (
            <ProductCard product={product} />
          )}
        </Grid>
      ))}
    </Grid>
  );
};
