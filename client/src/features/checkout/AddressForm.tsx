import { Grid, Typography } from '@mui/material';

import { AppCheckBox } from '../../app/components/AppCheckBox';
import { AppTextInput } from '../../app/components/AppTextInput';
import { useFormContext } from 'react-hook-form';

export const AddressForm = () => {
  const { control, watch } = useFormContext();
  const isDirty = watch([
    'fullName',
    'address1',
    'address2',
    'city',
    'state',
    'zip',
    'country',
  ]);

  return (
    <>
      <Typography variant='h6' gutterBottom>
        Shipping address
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name='fullName' label='Full name' />
        </Grid>
        <Grid item xs={12}>
          <AppTextInput control={control} name='address1' label='Address 1' />
        </Grid>
        <Grid item xs={12}>
          <AppTextInput control={control} name='address2' label='Address 2' />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name='city' label='City' />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name='state' label='State' />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name='zip' label='Zipcode' />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name='country' label='Country' />
        </Grid>
        <Grid item xs={12}>
          <AppCheckBox
            disabled={!isDirty}
            name='saveAddress'
            label='Save this as default address'
            control={control}
          />
        </Grid>
      </Grid>
    </>
  );
};
