import {
  Avatar,
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
  Typography,
} from '@mui/material';

import { IProduct } from '../../app/models/product';
import { Link } from 'react-router-dom';
import { LoadingButton } from '@mui/lab';
import { agent } from '../../app/api/agent';
import { useState } from 'react';

interface IProps {
  product: IProduct;
}

export const ProductCard = ({ product }: IProps) => {
  const [loading, setLoading] = useState(false);

  const handleAddItem = (productId: number) => {
    setLoading(true);
    try {
      agent.Basket.addItem(productId)
        .then(() => console.log('Item added to basket'))
        .catch((error) => console.log(error))
        .finally(() => setLoading(false));
    } catch (error) {
      console.log(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card>
      <CardHeader
        avatar={
          <Avatar sx={{ bgcolor: 'secondary.main' }}>
            {product.name.charAt(0).toUpperCase()}
          </Avatar>
        }
        title={product.name}
        titleTypographyProps={{
          sx: { fontWeight: 'bold', color: 'primary.main' },
        }}
      />
      <CardMedia
        sx={{
          height: 140,
          backgroundSize: 'contain',
          bgcolor: 'primary.light',
        }}
        image={product.pictureUrl}
        title={product.name}
      />
      <CardContent>
        <Typography gutterBottom color='secondary' variant='h5'>
          ${(product.price / 100).toFixed(2)}
        </Typography>
        <Typography variant='body2' color='text.secondary'>
          {product.brand} / {product.type}
        </Typography>
      </CardContent>
      <CardActions>
        <LoadingButton
          loading={loading}
          onClick={() => handleAddItem(product.id)}
          size='small'
        >
          ADD TO CART
        </LoadingButton>
        <Button component={Link} to={`/catalog/${product.id}`} size='small'>
          VIEW
        </Button>
      </CardActions>
    </Card>
  );
};
