import { useEffect, useState } from 'react';

import { IPaginatedResponse } from '../models/pagination';
import { IProduct } from '../models/product';

export const App = () => {
  const [products, setProducts] = useState<IProduct[]>([]);

  useEffect(() => {
    fetch('http://localhost:5000/products?PageNumber=1&PageSize=10')
      .then((res: Response) => res.json())
      .then((data: IPaginatedResponse<IProduct>) => setProducts(data.data));
  }, []);

  const addProducts = () => {
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
      <h1>Re-Store</h1>
      <button onClick={addProducts}>Add products</button>
      <ul>
        {products.map((product) => (
          <li key={product.id}>
            {product.name} - {product.price}
          </li>
        ))}
      </ul>
    </div>
  );
};
