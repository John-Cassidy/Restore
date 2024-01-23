import {
  Alert,
  AlertTitle,
  Button,
  ButtonGroup,
  Container,
  List,
  ListItem,
  ListItemText,
  Typography,
} from '@mui/material';
import React, { useState } from 'react';

import { agent } from '../../app/api/agent';

export const AboutPage = () => {
  const [validationErrors, setValidationErrors] = useState<string[]>([]);

  function getValidationError() {
    agent.TestErrors.getValidationError()
      .then(() => console.log('should not see this!'))
      .catch((error) => {
        console.log(error);
        const errorMessages = error.data.title
          .split(',')
          .map((err: string) => err.trim());
        setValidationErrors(errorMessages);
      });
  }

  return (
    <Container>
      <Typography gutterBottom variant={'h2'}>
        Errors for testing purposes
      </Typography>
      <ButtonGroup fullWidth>
        <Button
          onClick={() =>
            agent.TestErrors.get400HttpError().catch((error) =>
              console.log(error)
            )
          }
          variant={'contained'}
        >
          Test 400 Http error
        </Button>
        <Button
          onClick={() =>
            agent.TestErrors.get400Error().catch((error) => console.log(error))
          }
          variant={'contained'}
        >
          Test 400 error
        </Button>
        <Button
          onClick={() =>
            agent.TestErrors.get401Error().catch((error) => console.log(error))
          }
          variant={'contained'}
        >
          Test 401 error
        </Button>
        <Button
          onClick={() =>
            agent.TestErrors.get404Error().catch((error) => console.log(error))
          }
          variant={'contained'}
        >
          Test 404 error
        </Button>
        <Button
          onClick={() =>
            agent.TestErrors.get500Error().catch((error) => console.log(error))
          }
          variant={'contained'}
        >
          Test 500 error
        </Button>
        <Button onClick={getValidationError} variant={'contained'}>
          Test 400 validation error
        </Button>
        <Button
          onClick={() =>
            agent.TestErrors.getProductNotFound().catch((error) =>
              console.log(error)
            )
          }
          variant={'contained'}
        >
          Test Product Not Found error
        </Button>
      </ButtonGroup>
      {validationErrors.length > 0 && (
        <Alert severity='error'>
          <AlertTitle>Validation Errors</AlertTitle>
          <List>
            {validationErrors.map((error) => (
              <ListItem key={error}>
                <ListItemText>{error}</ListItemText>
              </ListItem>
            ))}
          </List>
        </Alert>
      )}
    </Container>
  );
};
