import { useEffect, useState } from 'react';

import { IPaginatedResponse } from '../../app/models/pagination';
import { IProduct } from '../../app/models/product';
import { LoadingComponent } from '../../app/layout/LoadingComponent';
import { ProductList } from './ProductList';
import { agent } from '../../app/api/agent';

export const Catalog = () => {
  const [products, setProducts] = useState<IProduct[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    agent.Catalog.list()
      .then((response: IPaginatedResponse<IProduct>) => {
        setProducts(response.data);
      })
      .catch((error) => console.log(error))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingComponent message='Loading products...' />;

  return (
    <>
      <ProductList products={products} />
    </>
  );
};
