export interface ProductParams extends PaginationParams {
  orderBy?: string;
  searchTerm?: string;
  types?: string;
  brands?: string;
}

export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
}
