import { Button, Fade, Menu, MenuItem } from '@mui/material';
import { useAppDispatch, useAppSelector } from '../store/configureStore';

import React from 'react';
import { signOut } from '../../features/account/accountSlice';

export const SignedInMenu = () => {
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.account);

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <>
      <Button color='inherit' sx={{ typography: 'h6' }} onClick={handleClick}>
        {user?.email}
      </Button>
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        TransitionComponent={Fade}
      >
        <MenuItem onClick={handleClose}>Profile</MenuItem>
        <MenuItem onClick={handleClose}>My Orders</MenuItem>
        <MenuItem onClick={() => dispatch(signOut())}>Logout</MenuItem>
      </Menu>
    </>
  );
};
