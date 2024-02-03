import { forwardRef, useImperativeHandle, useRef } from 'react';

import { InputBaseComponentProps } from '@mui/material';

interface IProps extends InputBaseComponentProps {}

export const StripeInput = forwardRef<unknown, IProps>(
  ({ component: Component, ...props }: IProps, ref: React.Ref<unknown>) => {
    const elementRef = useRef<any>();

    useImperativeHandle(ref, () => ({
      focus: () => elementRef.current.focus,
    }));

    return (
      <Component
        onReady={(element: any) => (elementRef.current = element)}
        {...props}
      />
    );
  }
);
