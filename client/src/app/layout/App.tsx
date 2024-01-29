import 'react-toastify/dist/ReactToastify.css';

import {
  Container,
  CssBaseline,
  ThemeProvider,
  createTheme,
} from '@mui/material';
import { useCallback, useEffect, useState } from 'react';

import { Header } from './Header';
import { LoadingComponent } from './LoadingComponent';
import { Outlet } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { fecthBasketAsync } from '../../features/basket/basketSlice';
import { fetchCurrentUser } from '../../features/account/accountSlice';
import { useAppDispatch } from '../store/configureStore';

export const App = () => {
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
  const theme = createTheme({
    palette: {
      mode: palleteType,
      background: {
        default: palleteType === 'light' ? '#eaeaea' : '#121212',
      },
    },
  });

  const handleThemeChange = () => {
    setDarkMode(!darkMode);
  };

  if (loading)
    return <LoadingComponent message='Loading basket...'></LoadingComponent>;

  return (
    <div>
      <ThemeProvider theme={theme}>
        <ToastContainer
          position='bottom-right'
          hideProgressBar
          theme='colored'
        />
        <CssBaseline />
        <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
        <Container>
          <Outlet />
        </Container>
      </ThemeProvider>
    </div>
  );
};
