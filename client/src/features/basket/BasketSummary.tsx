import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
} from '@mui/material';

import { currencyFormat } from '../../app/util/util';
import { useAppSelector } from '../../app/store/configureStore';

export const BasketSummary = () => {
  const { basket } = useAppSelector((state) => state.basket);
  // reduce basket items array into single value and get subtotal
  const subtotal =
    basket?.items.reduce((a, b) => a + b.price * b.quantity, 0) ?? 0;

  const deliveryFee = subtotal == 0 ? 0 : subtotal > 10000 ? 0 : 500;

  return (
    <>
      <TableContainer component={Paper} variant={'outlined'}>
        <Table>
          <TableBody>
            <TableRow>
              <TableCell colSpan={2}>Subtotal</TableCell>
              <TableCell align='right'>{currencyFormat(subtotal)}</TableCell>
            </TableRow>
            <TableRow>
              <TableCell colSpan={2}>Delivery fee*</TableCell>
              <TableCell align='right'>{currencyFormat(deliveryFee)}</TableCell>
            </TableRow>
            <TableRow>
              <TableCell colSpan={2}>Total</TableCell>
              <TableCell align='right'>
                {currencyFormat(subtotal + deliveryFee)}
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell>
                {deliveryFee > 0 && (
                  <TableRow>
                    <TableCell colSpan={3}>
                      <span style={{ fontStyle: 'italic' }}>
                        *Orders over $100 qualify for free delivery
                      </span>
                    </TableCell>
                  </TableRow>
                )}
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
};
