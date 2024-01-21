import { useState } from 'react';

function generateProducts() {
  const products = [];
  for (let i = 1; i <= 3; i++) {
    const product = {
      name: `product${i}`,
      price: i * 100.0,
    };
    products.push(product);
  }
  return products;
}

export const App = () => {
  const [products, setProducts] = useState(generateProducts());

  const addProducts = () => {
    setProducts((prevState) => [
      ...prevState,
      {
        name: `product${prevState.length + 1}`,
        price: prevState.length * 100 + 100.0,
      },
    ]);
  };

  return (
    <div>
      <h1>Re-Store</h1>
      <button onClick={addProducts}>Add products</button>
      <ul>
        {products.map((product) => (
          <li key={product.name}>
            {product.name} - {product.price}
          </li>
        ))}
      </ul>
    </div>
  );
};
