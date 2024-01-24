import { useEffect, useState } from 'react';

import { IPaginatedResponse } from '../../app/models/pagination';
import { IProduct } from '../../app/models/product';
import { ProductList } from './ProductList';
import { agent } from '../../app/api/agent';

export const Catalog = () => {
  const [products, setProducts] = useState<IProduct[]>([]);

  useEffect(() => {
    agent.Catalog.list().then((response: IPaginatedResponse<IProduct>) => {
      setProducts(response.data);
    });
  }, []);

  return (
    <>
      <ProductList products={products} />
    </>
  );
};
