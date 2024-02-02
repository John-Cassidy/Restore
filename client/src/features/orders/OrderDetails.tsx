import { Box, Button, Grid, Typography } from '@mui/material';

import { BasketSummary } from '../basket/BasketSummary';
import { BasketTable } from '../basket/BasketTable';
import { IBasketItem } from '../../app/models/basket';
import { IOrder } from '../../app/models/order';

interface IProps {
  order: IOrder;
  setSelectedOrder: (id: number) => void;
}

export const OrderDetails = ({ order, setSelectedOrder }: IProps) => {
  const subtotal =
    order.orderItems.reduce((a, b) => a + b.price * b.quantity, 0) ?? 0;
  return (
    <>
      <Box display='flex' justifyContent='space-between'>
        <Typography sx={{ p: 2 }} gutterBottom variant='h4'>
          Order# {order.id} - {order.orderStatus}
        </Typography>
        <Button
          onClick={() => setSelectedOrder(0)}
          sx={{ m: 2 }}
          size='large'
          variant='contained'
        >
          Back to orders
        </Button>
      </Box>
      <BasketTable items={order.orderItems as IBasketItem[]} isBasket={false} />
      <Grid container>
        <Grid item xs={6} />
        <Grid item xs={6}>
          <BasketSummary subtotal={subtotal} />
        </Grid>
      </Grid>
    </>
  );
};
