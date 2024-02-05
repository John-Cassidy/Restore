import { UseControllerProps, useController } from 'react-hook-form';

import { TextField } from '@mui/material';

interface IProps extends UseControllerProps {
  label: string;
}

export const AppTextInput = (props: IProps) => {
  const { fieldState, field } = useController({ ...props, defaultValue: '' });

  return (
    <TextField
      {...props}
      {...field}
      fullWidth
      variant='outlined'
      error={!!fieldState.error}
      helperText={fieldState.error?.message}
    />
  );
};
