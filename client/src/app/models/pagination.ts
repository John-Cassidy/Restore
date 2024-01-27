export interface IMetaData {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
}

export class PaginatedResponse<T extends object> {
  data: T[];
  metaData: IMetaData;

  constructor(data: T[], metaData: IMetaData) {
    this.data = data;
    this.metaData = metaData;
  }
}
