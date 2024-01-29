import {
  AppBar,
  Badge,
  Box,
  IconButton,
  List,
  ListItem,
  Switch,
  Toolbar,
  Typography,
} from '@mui/material';
import { Link, NavLink } from 'react-router-dom';

import { ShoppingCart } from '@mui/icons-material';
import { SignedInMenu } from './SignedInMenu';
import { useAppSelector } from '../store/configureStore';

const midLinks = [
  { title: 'catalog', path: '/catalog' },
  { title: 'about', path: '/about' },
  { title: 'contact', path: '/contact' },
];

const rightLinks = [
  { title: 'login', path: '/login' },
  { title: 'register', path: '/register' },
];

const navLinkStyles = {
  color: 'inherit',
  textDecoration: 'none',
  typography: 'h6',
  '&:hover': {
    color: 'grey.500',
  },
  '&.active': {
    color: 'text.secondary',
  },
};

interface IProps {
  darkMode: boolean;
  handleThemeChange: () => void;
}

export const Header = ({ handleThemeChange, darkMode }: IProps) => {
  const { user } = useAppSelector((state) => state.account);
  const { basket } = useAppSelector((state) => state.basket);
  // reduce array of items to a single number
  const itemCount = basket?.items.reduce((acc, item) => acc + item.quantity, 0);

  return (
    <AppBar position='static' sx={{ mb: 4 }}>
      <Toolbar
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <Box display='flex' alignItems='center'>
          <Typography
            variant='h6'
            component={NavLink}
            to='/'
            sx={navLinkStyles}
          >
            RE-STORE
          </Typography>
          <Switch checked={darkMode} onChange={handleThemeChange} />
        </Box>

        <Box>
          <List sx={{ display: 'flex' }}>
            {midLinks.map(({ title, path }) => (
              <ListItem
                component={NavLink}
                to={path}
                key={path}
                sx={navLinkStyles}
              >
                {title.toUpperCase()}
              </ListItem>
            ))}
          </List>
        </Box>

        <Box display='flex' alignItems='center'>
          <IconButton
            component={Link}
            to='/basket'
            size='large'
            edge='start'
            color='inherit'
            sx={{ mr: 2 }}
          >
            <Badge badgeContent={itemCount} color='secondary'>
              <ShoppingCart />
            </Badge>
          </IconButton>
          {user ? (
            <SignedInMenu />
          ) : (
            <List sx={{ display: 'flex' }}>
              {rightLinks.map(({ title, path }) => (
                <ListItem
                  component={NavLink}
                  to={path}
                  key={path}
                  sx={navLinkStyles}
                >
                  {title.toUpperCase()}
                </ListItem>
              ))}
            </List>
          )}
        </Box>
      </Toolbar>
    </AppBar>
  );
};
