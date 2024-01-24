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
import { IError, IModelStateErrors } from '../../app/models/modelStateErrors';

import { agent } from '../../app/api/agent';
import { useState } from 'react';

export const AboutPage = () => {
  const [validationErrors, setValidationErrors] = useState<IError[]>([]);

  const getProductNotFound = () => {
    agent.TestErrors.getProductNotFound()
      .then(() => console.log('should not see this!'))
      .catch((error) => {
        console.log(error);
        if (error.data.detail) {
          const modelStateErrors: IModelStateErrors = JSON.parse(
            error.data.detail
          );
          console.log(modelStateErrors);
          // const errorDetail: Record<string, string[]> = JSON.parse(
          //   error.data.detail
          // );
          setValidationErrors(modelStateErrors.Errors);
        }
      });
  };

  function getValidationError() {
    agent.TestErrors.getValidationError()
      .then(() => console.log('should not see this!'))
      .catch((error) => {
        console.log(error);
        if (error.data.detail) {
          const modelStateErrors: IModelStateErrors = JSON.parse(
            error.data.detail
          );
          console.log(modelStateErrors);
          // const errorDetail: Record<string, string[]> = JSON.parse(
          //   error.data.detail
          // );
          setValidationErrors(modelStateErrors.Errors);
        }
      });
    // .catch((error) => {
    //   console.log(error);
    //   if (error.data.detail) {
    //     const errorMessages: string[] = [];
    //     for (const key in error.data.detail) {
    //       if (error.data.detail[key]) {
    //         errorMessages.push(error.data.detail[key]);
    //       }
    //     }
    //     setValidationErrors(errorMessages);
    //   }

    //   // const errorMessages = error.data.detail
    //   //   .split(',')
    //   //   .map((err: string) => err.trim());
    //   // setValidationErrors(errorMessages);
    // });
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
        <Button onClick={getProductNotFound} variant={'contained'}>
          Test Product Not Found error
        </Button>
      </ButtonGroup>

      {validationErrors.length > 0 && (
        <Alert severity='error'>
          <AlertTitle>Validation Errors</AlertTitle>
          <List>
            {validationErrors.map((error) => (
              <ListItem key={error.PropertyName}>
                <ListItemText>{error.ErrorMessage}</ListItemText>
              </ListItem>
            ))}
          </List>
        </Alert>
      )}

      {/* {validationErrors && (
        <Alert severity={'error'}>
          <AlertTitle>Validation errors</AlertTitle>
          <List>
            {Object.entries(validationErrors).map(([key, value]) => (
              <ListItem key={key}>
                <ListItemText primary={key} secondary={value} />
              </ListItem>
            ))}
          </List>
        </Alert>
      )} */}
    </Container>
  );
};
