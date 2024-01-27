import { Checkbox, FormControlLabel, FormGroup } from '@mui/material';

import React from 'react';

interface IProps {
  items: string[];
  checked?: string[];
  onChange: (event: string[]) => void;
}

export const CheckboxButtons = ({ items, checked, onChange }: IProps) => {
  const [checkedItems, setCheckedItems] = React.useState(checked || []);

  const handleChecked = (item: string) => {
    const currentIndex = checkedItems.indexOf(item);
    const newChecked = [...checkedItems];

    if (currentIndex === -1) {
      newChecked.push(item);
    } else {
      newChecked.splice(currentIndex, 1);
    }

    setCheckedItems(newChecked);
    onChange(newChecked);
  };

  return (
    <FormGroup>
      {items.map((item) => {
        return (
          <FormControlLabel
            key={item}
            control={
              <Checkbox
                checked={checkedItems.indexOf(item) !== -1}
                onClick={() => handleChecked(item)}
              />
            }
            label={item}
          />
        );
      })}
    </FormGroup>
  );
};
