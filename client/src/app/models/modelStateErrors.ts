export interface IModelStateErrors {
  IsValid: boolean;
  Errors: IError[];
  RuleSetsExecuted?: string[];
}

export interface IError {
  PropertyName: string;
  ErrorMessage: string;
  AttemptedValue?: any;
  CustomState?: any;
  Severity: Severity;
  ErrorCode?: string;
  FormattedMessagePlaceholderValues?: Record<string, unknown>;
}

export enum Severity {
  Error,
  Warning,
  Info,
}
