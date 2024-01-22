import { Button } from '@mui/material';
import { IProduct } from '../../app/models/product';
import { ProductList } from './ProductList';

interface IProps {
  products: IProduct[];
  addProduct: () => void;
}

export const Catalog = ({ products, addProduct }: IProps) => {
  return (
    <>
      <Button variant='contained' onClick={addProduct}>
        Add products
      </Button>
      <ProductList products={products} />
    </>
  );
};
