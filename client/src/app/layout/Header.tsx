import { AppBar, Toolbar, Typography } from '@mui/material';

import React from 'react';

export const Header = () => {
  return (
    <AppBar position='static'>
      <Toolbar>
        <Typography variant='h6' component='div' sx={{ flexGrow: 1 }}>
          RE-STORE
        </Typography>
      </Toolbar>
    </AppBar>
  );
};
