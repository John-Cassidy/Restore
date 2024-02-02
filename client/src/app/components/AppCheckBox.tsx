import { Checkbox, FormControlLabel } from '@mui/material';
import { UseControllerProps, useController } from 'react-hook-form';

interface IProps extends UseControllerProps {
  label: string;
}

export const AppCheckBox = (props: IProps) => {
  const { field } = useController({ ...props, defaultValue: false });
  return (
    <FormControlLabel
      control={<Checkbox {...field} checked={field.value} color='secondary' />}
      label={props.label}
    />
  );
};
