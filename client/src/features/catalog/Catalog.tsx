import { useEffect, useState } from 'react';

import { IPaginatedResponse } from '../../app/models/pagination';
import { IProduct } from '../../app/models/product';
import { ProductList } from './ProductList';

export const Catalog = () => {
  const [products, setProducts] = useState<IProduct[]>([]);

  useEffect(() => {
    fetch('http://localhost:5000/products?PageNumber=1&PageSize=30')
      .then((res: Response) => res.json())
      .then((data: IPaginatedResponse<IProduct>) => setProducts(data.data));
  }, []);

  return (
    <>
      <ProductList products={products} />
    </>
  );
};
