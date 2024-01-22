import { Container, CssBaseline } from '@mui/material';
import { useEffect, useState } from 'react';

import { Catalog } from '../../features/catalog/Catalog';
import { Header } from './Header';
import { IPaginatedResponse } from '../models/pagination';
import { IProduct } from '../models/product';

export const App = () => {
  const [products, setProducts] = useState<IProduct[]>([]);

  useEffect(() => {
    fetch('http://localhost:5000/products?PageNumber=1&PageSize=30')
      .then((res: Response) => res.json())
      .then((data: IPaginatedResponse<IProduct>) => setProducts(data.data));
  }, []);

  const addProduct = () => {
    setProducts((prevState) => [
      ...prevState,
      {
        id: prevState.length + 101,
        name: `product${prevState.length + 1}`,
        price: prevState.length * 100 + 100.0,
        brand: 'brand',
        description: 'description',
        pictureUrl: 'http://picsum.photos/200',
      },
    ]);
  };

  return (
    <div>
      <CssBaseline />
      <Header />
      <Container>
        <Catalog products={products} addProduct={addProduct} />
      </Container>
    </div>
  );
};
