import {
  Container,
  CssBaseline,
  ThemeProvider,
  createTheme,
} from '@mui/material';

import { Catalog } from '../../features/catalog/Catalog';
import { Header } from './Header';
import { useState } from 'react';

export const App = () => {
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

  return (
    <div>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
        <Container>
          <Catalog />
        </Container>
      </ThemeProvider>
    </div>
  );
};
