export interface IModelStateErrors {
  IsValid: boolean;
  Errors: IError[];
  RuleSetsExecuted?: string[];
}

export interface IError {
  PropertyName: string;
  ErrorMessage: string;
  AttemptedValue?: unknown;
  CustomState?: unknown;
  Severity: Severity;
  ErrorCode?: string;
  FormattedMessagePlaceholderValues?: Record<string, unknown>;
}

export enum Severity {
  Error,
  Warning,
  Info,
}
