import {
  Alert,
  AlertTitle,
  List,
  ListItem,
  ListItemText,
  Paper,
} from '@mui/material';
import { IError, IModelStateErrors } from '../../app/models/modelStateErrors';
import { Link, useNavigate } from 'react-router-dom';

import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import { LoadingButton } from '@mui/lab';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { agent } from '../../app/api/agent';
import { useAppDispatch } from '../../app/store/configureStore';
import { useForm } from 'react-hook-form';
import { useState } from 'react';

export const Register = () => {
  const [validationErrors, setValidationErrors] = useState<IError[]>([]);
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const {
    register,
    handleSubmit,
    formState: { isSubmitting, errors, isValid },
  } = useForm({
    mode: 'onTouched',
  });

  // const handleApiErrors = (errors: any) => {
  //   console.log(errors);
  //   if (errors) {
  //     errors.foreach((error: string) => {
  //       if (error.includes('password')) {
  //         console.log('password', { message: error });
  //       } else if (error.includes('email')) {
  //         console.log('email', { message: error });
  //       } else if (error.includes('username')) {
  //         console.log('username', { message: error });
  //       }
  //     });
  //     setValidationErrors(errors);
  //   }
  // };

  return (
    <Container
      component={Paper}
      maxWidth='sm'
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        p: 4,
      }}
    >
      <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
        <LockOutlinedIcon />
      </Avatar>
      <Typography component='h1' variant='h5'>
        Register
      </Typography>
      <Box
        component='form'
        onSubmit={handleSubmit((data) =>
          agent.Account.register(data).catch((error) => {
            if (error.data.detail) {
              const modelStateErrors: IModelStateErrors = JSON.parse(
                error.data.detail
              );
              console.log(modelStateErrors);
              setValidationErrors(modelStateErrors.Errors);
            }
          })
        )}
        noValidate
        sx={{ mt: 1 }}
      >
        <TextField
          margin='normal'
          fullWidth
          label='Username'
          autoComplete='username'
          autoFocus
          {...register('username', { required: 'Username is required' })}
          error={!!errors.username}
          helperText={errors.username?.message as string}
        />
        <TextField
          margin='normal'
          fullWidth
          label='Email'
          type='email'
          autoComplete='email'
          {...register('email', { required: 'Email is required' })}
          error={!!errors.email}
          helperText={errors.email?.message as string}
        />
        <TextField
          margin='normal'
          required
          fullWidth
          label='Password'
          type='password'
          {...register('password', { required: 'Password is required' })}
          error={!!errors.email}
          helperText={errors.email?.message as string}
        />
        {validationErrors.length > 0 && (
          <Alert severity='error'>
            <AlertTitle>Validation Errors</AlertTitle>
            <List>
              {validationErrors.map((error: any) => (
                <ListItem key={error.PropertyName}>
                  <ListItemText>{error.ErrorMessage}</ListItemText>
                </ListItem>
              ))}
            </List>
          </Alert>
        )}
        <LoadingButton
          loading={isSubmitting}
          disabled={!isValid || isSubmitting}
          type='submit'
          fullWidth
          variant='contained'
          sx={{ mt: 3, mb: 2 }}
        >
          Register
        </LoadingButton>
        <Grid container>
          <Grid item>
            <Link to='/login'>{'Already have an account? Sign In'}</Link>
          </Grid>
        </Grid>
      </Box>
    </Container>
  );
};
