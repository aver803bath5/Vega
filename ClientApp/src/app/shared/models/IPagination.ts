export interface IPagination {
    hasPrevious: boolean;
    totalCount: number;
    pageSize: number;
    currentPage: number;
    totalPage: number;
    hasNext: boolean;
}
