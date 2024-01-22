export interface IMetaData {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
}

export interface IPaginatedResponse<T extends object> {
  data: T[];
  metaData: IMetaData;
}
