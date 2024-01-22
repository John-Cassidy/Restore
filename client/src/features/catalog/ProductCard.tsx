import { Avatar, ListItem, ListItemAvatar, ListItemText } from '@mui/material';

import { IProduct } from '../../app/models/product';
import React from 'react';

interface IProps {
  product: IProduct;
}

export const ProductCard = ({ product }: IProps) => {
  return (
    <ListItem key={product.id}>
      <ListItemAvatar>
        <Avatar src={product.pictureUrl} />
      </ListItemAvatar>
      <ListItemText>
        {product.name} - {product.price}
      </ListItemText>
    </ListItem>
  );
};
