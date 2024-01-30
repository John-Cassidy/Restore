import { IError, IModelStateErrors } from '../../app/models/modelStateErrors';
import { Link, useNavigate } from 'react-router-dom';

import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import { LoadingButton } from '@mui/lab';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { Paper } from '@mui/material';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { agent } from '../../app/api/agent';
import { toast } from 'react-toastify';
import { useForm } from 'react-hook-form';

export const Register = () => {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    setError,
    formState: { isSubmitting, errors, isValid },
  } = useForm({
    mode: 'onTouched',
  });

  const handleApiErrors = (errors: any) => {
    console.log(errors);

    if (errors?.data?.detail) {
      const modelStateErrors: IModelStateErrors = JSON.parse(
        errors.data.detail
      );
      if (modelStateErrors?.Errors) {
        modelStateErrors.Errors.forEach((error: IError) => {
          if (error.ErrorMessage.toLocaleLowerCase().includes('password')) {
            setError('password', { message: error.ErrorMessage });
          } else if (error.ErrorMessage.toLocaleLowerCase().includes('email')) {
            setError('email', { message: error.ErrorMessage });
          } else if (
            error.ErrorMessage.toLocaleLowerCase().includes('username')
          ) {
            setError('username', { message: error.ErrorMessage });
          }
        });
      }
    }
  };

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
          agent.Account.register(data)
            .then(() => {
              toast.success('Registration successful - you can now login');
              navigate('/login');
            })
            .catch((error) => handleApiErrors(error))
        )}
        noValidate
        sx={{ mt: 1 }}
      >
        <TextField
          margin='normal'
          required
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
          {...register('email', {
            required: 'Email is required',
            pattern: {
              value: /^\w+[\w-.]*@\w+((-\w+)|(\w*))\.[a-z]{2,3}$/,
              message: 'Not a valid email address',
            },
          })}
          error={!!errors.email}
          helperText={errors.email?.message as string}
        />
        <TextField
          margin='normal'
          required
          fullWidth
          label='Password'
          type='password'
          {...register('password', {
            required: 'password is required',
            pattern: {
              value:
                /(?=^.{6,10}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$/,
              message: 'Password does not meet complexity requirements',
            },
          })}
          error={!!errors.password}
          helperText={errors.password?.message as string}
        />
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
