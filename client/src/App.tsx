import { useEffect, useState } from 'react';

export const App = () => {
  const [products, setProducts] = useState({
    metaData: {
      pageNumber: 1,
      pageSize: 10,
      totalPages: 1,
      totalCount: 0,
    },
    data: [] as any[],
  });

  useEffect(() => {
    fetch('http://localhost:5000/products?PageNumber=1&PageSize=10')
      .then((res) => res.json())
      .then((data) => setProducts(data));
  }, []);

  // const addProducts = () => {
  //   setProducts((prevState) => [
  //     ...prevState,
  //     {
  //       name: `product${prevState.length + 1}`,
  //       price: prevState.length * 100 + 100.0,
  //     },
  //   ]);
  // };

  return (
    <div>
      <h1>Re-Store</h1>
      {/* <button onClick={addProducts}>Add products</button> */}
      <ul>
        {products.data.map((product) => (
          <li key={product.name}>
            {product.name} - {product.price}
          </li>
        ))}
      </ul>
    </div>
  );
};
