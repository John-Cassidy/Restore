import { Box, Pagination, Typography } from '@mui/material';

import { IMetaData } from '../models/pagination';
import { useState } from 'react';

interface IProps {
  metaData: IMetaData;
  onPageChange: (page: number) => void;
}

export const AppPagination = ({ metaData, onPageChange }: IProps) => {
  const { pageSize, currentPage, totalCount, totalPages } = metaData;
  const [pageNumber, setPageNumber] = useState(currentPage);

  const handlePageChange = (page: number) => {
    setPageNumber(page);
    onPageChange(page);
  };

  return (
    <Box
      display='flex'
      justifyContent='space-between'
      alignItems='center'
      sx={{ marginBottom: 3 }}
    >
      <Typography>
        Displaying {(currentPage - 1) * pageSize - 1}-
        {currentPage * pageSize > totalCount!
          ? totalCount
          : currentPage * pageSize}{' '}
        of {totalCount} results
      </Typography>
      <Pagination
        color='secondary'
        size='large'
        count={totalPages}
        page={pageNumber}
        onChange={(_, page) => handlePageChange(page)}
      />
    </Box>
  );
};
