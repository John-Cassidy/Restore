import 'react-toastify/dist/ReactToastify.css';

import createTheme from '@mui/material/styles/createTheme';
import { Outlet, useLocation } from 'react-router-dom';
import { useCallback, useEffect, useMemo, useState } from 'react';

import { Header } from './Header';
import { HomePage } from '../../features/home/HomePage';
import { LoadingComponent } from './LoadingComponent';
import { ToastContainer } from 'react-toastify';
import { fecthBasketAsync } from '../../features/basket/basketSlice';
import { fetchCurrentUser } from '../../features/account/accountSlice';
import { useAppDispatch } from '../store/configureStore';
import { Container, CssBaseline } from '@mui/material';
import { ThemeProvider } from '@mui/material/styles';

export const App = () => {
  const location = useLocation();
  const dispatch = useAppDispatch();
  const [loading, setLoading] = useState(true);

  const initApp = useCallback(async () => {
    try {
      await dispatch(fetchCurrentUser());
      await dispatch(fecthBasketAsync());
    } catch (error) {
      console.log(error);
    }
  }, [dispatch]);

  useEffect(() => {
    initApp().then(() => setLoading(false));
  }, [initApp]);

  const [darkMode, setDarkMode] = useState(true);
  const palleteType = darkMode ? 'dark' : 'light';
  const theme = useMemo(() => createTheme({
    palette: {
      mode: palleteType,
      background: {
        default: palleteType === 'light' ? '#eaeaea' : '#121212',
      },
    },
  }), [palleteType]);

  const handleThemeChange = () => {
    setDarkMode(!darkMode);
  };

  return (    
      <ThemeProvider theme={theme}>
        <ToastContainer
          position='bottom-right'
          hideProgressBar
          theme='colored'
        />
        <CssBaseline />
        <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
        {loading ? (
          <LoadingComponent message='Initialising app...'></LoadingComponent>
        ) : location.pathname === '/' ? (
          <HomePage />
        ) : (
          <Container sx={{ mt: 4 }}>
            <Outlet />
          </Container>
        )}
      </ThemeProvider>
  );
};
