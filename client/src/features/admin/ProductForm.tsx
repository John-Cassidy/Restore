import { Box, Button, Grid, Paper, Typography } from '@mui/material';
import { FieldValues, useForm } from 'react-hook-form';

import { AppDropzone } from '../../app/components/AppDropzone';
import { AppSelectList } from '../../app/components/AppSelectList';
import { AppTextInput } from '../../app/components/AppTextInput';
import { IProduct } from '../../app/models/product';
import { useEffect } from 'react';
import { useProducts } from '../../app/hooks/useProducts';
import { validationSchema } from './productValidation';
import { yupResolver } from '@hookform/resolvers/yup';

interface IProps {
  product?: IProduct;
  cancelEdit: () => void;
}

export const ProductForm = ({ product, cancelEdit }: IProps) => {
  const {
    control,
    reset,
    handleSubmit,
    watch,
    formState: { isDirty, isSubmitting },
  } = useForm({
    mode: 'all',
    resolver: yupResolver<any>(validationSchema),
  });
  const { brands, types } = useProducts();
  const watchFile = watch('file', null);

  useEffect(() => {
    if (product) {
      reset(product);
    }
  }, [product, reset]);

  const handleSubmitData = (data: FieldValues) => {
    console.log(data);
  };

  return (
    <Box component={Paper} sx={{ p: 4 }}>
      <Typography variant='h4' gutterBottom sx={{ mb: 4 }}>
        Product Details
      </Typography>
      <form onSubmit={handleSubmit(handleSubmitData)}>
        <Grid container spacing={3}>
          <Grid item xs={12} sm={12}>
            <AppTextInput control={control} name='name' label='Product name' />
          </Grid>
          <Grid item xs={12} sm={6}>
            <AppSelectList
              items={brands}
              control={control}
              name='brand'
              label='Brand'
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <AppSelectList
              items={types}
              control={control}
              name='type'
              label='Type'
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <AppTextInput
              type='number'
              control={control}
              name='price'
              label='Price'
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <AppTextInput
              type='number'
              control={control}
              name='quantityInStock'
              label='Quantity in Stock'
            />
          </Grid>
          <Grid item xs={12}>
            <AppTextInput
              multiline={true}
              rows={4}
              control={control}
              name='description'
              label='Description'
            />
          </Grid>
          <Grid item xs={12}>
            <Box
              display='flex'
              justifyContent='space-between'
              alignItems='center'
            >
              <AppDropzone control={control} name='file' />
              {watchFile ? (
                <img
                  src={watchFile.preview}
                  alt='preview'
                  style={{ maxHeight: 200 }}
                />
              ) : (
                <img
                  src={product?.pictureUrl}
                  alt={product?.name}
                  style={{ maxHeight: 200 }}
                />
              )}
            </Box>
          </Grid>
        </Grid>
        <Box display='flex' justifyContent='space-between' sx={{ mt: 3 }}>
          <Button onClick={cancelEdit} variant='contained' color='inherit'>
            Cancel
          </Button>
          <Button type='submit' variant='contained' color='success'>
            Submit
          </Button>
        </Box>
      </form>
    </Box>
  );
};
