import { IProduct } from '../../app/models/product';
import React from 'react';

interface IProps {
  products: IProduct[];
  addProduct: () => void;
}

export const Catalog = ({ products, addProduct }: IProps) => {
  return (
    <>
      <button onClick={addProduct}>Add products</button>
      <ul>
        {products.map((product) => (
          <li key={product.id}>
            {product.name} - {product.price}
          </li>
        ))}
      </ul>
    </>
  );
};
