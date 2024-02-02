import { Add, Delete, Remove } from '@mui/icons-material';
import {
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material';
import { addBasketItemAsync, removeBasketItemAsync } from './basketSlice';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';

import { IBasketItem } from '../../app/models/basket';
import { LoadingButton } from '@mui/lab';
import { currencyFormat } from '../../app/util/util';

interface IProps {
  items: IBasketItem[];
  isBasket?: boolean;
}

export const BasketTable = ({ items, isBasket = true }: IProps) => {
  const { status } = useAppSelector((state) => state.basket);
  const dispatch = useAppDispatch();
  return (
    <TableContainer component={Paper}>
      <Table sx={{ minWidth: 650 }} aria-label='simple table'>
        <TableHead>
          <TableRow>
            <TableCell>Product</TableCell>
            <TableCell align='center'>Price</TableCell>
            <TableCell align='center'>Quantity</TableCell>
            <TableCell align='right'>Subtotal</TableCell>
            {isBasket && <TableCell align='right'></TableCell>}
          </TableRow>
        </TableHead>
        <TableBody>
          {items.map((item) => (
            <TableRow
              key={item.productId}
              sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
            >
              <TableCell component='th' scope='row'>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  <img
                    src={item.pictureUrl}
                    alt={item.name}
                    style={{ width: 50, marginRight: 20 }}
                  />
                  <span>{item.name}</span>
                </Box>
              </TableCell>
              <TableCell align='center'>{currencyFormat(item.price)}</TableCell>
              <TableCell align='center'>
                {isBasket && (
                  <LoadingButton
                    loading={status.endsWith(
                      'pendingRemoveItem' + item.productId + 'rem'
                    )}
                    onClick={() =>
                      dispatch(
                        removeBasketItemAsync({
                          productId: item.productId,
                          quantity: 1,
                          name: 'rem',
                        })
                      )
                    }
                    color='error'
                  >
                    <Remove />
                  </LoadingButton>
                )}
                {item.quantity}
                {isBasket && (
                  <LoadingButton
                    loading={status.endsWith('pendingAddItem' + item.productId)}
                    onClick={() =>
                      dispatch(
                        addBasketItemAsync({ productId: item.productId })
                      )
                    }
                    color='secondary'
                  >
                    <Add />
                  </LoadingButton>
                )}
              </TableCell>
              <TableCell align='right'>
                {((item.price / 100) * item.quantity).toFixed(2)}
              </TableCell>
              {isBasket && (
                <TableCell align='right'>
                  <LoadingButton
                    loading={status.endsWith(
                      'pendingRemoveItem' + item.productId + 'del'
                    )}
                    onClick={() =>
                      dispatch(
                        removeBasketItemAsync({
                          productId: item.productId,
                          quantity: item.quantity,
                          name: 'del',
                        })
                      )
                    }
                    color='error'
                  >
                    <Delete />
                  </LoadingButton>
                </TableCell>
              )}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};
