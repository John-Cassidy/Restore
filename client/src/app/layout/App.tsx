import 'react-toastify/dist/ReactToastify.css';

import {
  Container,
  CssBaseline,
  ThemeProvider,
  createTheme,
} from '@mui/material';
import { useEffect, useState } from 'react';

import { Header } from './Header';
import { LoadingComponent } from './LoadingComponent';
import { Outlet } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { agent } from '../api/agent';
import { getCookie } from '../util/util';
import { useStoreContext } from '../context/StoreContext';

export const App = () => {
  const { setBasket } = useStoreContext();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const buyerId = getCookie('buyerId');
    if (buyerId) {
      agent.Basket.get()
        .then((basket) => setBasket(basket))
        .catch((error) => console.log(error))
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, [setBasket]);

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
