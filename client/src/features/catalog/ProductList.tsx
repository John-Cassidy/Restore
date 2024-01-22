import { IProduct } from '../../app/models/product';
import { List } from '@mui/material';
import { ProductCard } from './ProductCard';

interface IProps {
  products: IProduct[];
}

export const ProductList = ({ products }: IProps) => {
  return (
    <List>
      {products.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </List>
  );
};
