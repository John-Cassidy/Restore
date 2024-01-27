import {
  FormControl,
  FormControlLabel,
  Radio,
  RadioGroup,
} from '@mui/material';

interface IProps {
  options: any[];
  onChange: (event: any) => void;
  selectedValue: string;
}

export const RadioButtonGroup = ({
  options,
  onChange,
  selectedValue,
}: IProps) => {
  return (
    <FormControl component='fieldset'>
      <RadioGroup onChange={onChange} value={selectedValue}>
        {options.map((option) => (
          <FormControlLabel
            key={option.value}
            value={option.value}
            control={<Radio />}
            label={option.label}
          />
        ))}
      </RadioGroup>
    </FormControl>
  );
};
