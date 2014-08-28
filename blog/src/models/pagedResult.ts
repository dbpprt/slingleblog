
module Application {

    export interface IPagedResult<T> {
        pageSize: number;
        page: number;

        hasNextPage: boolean;
        hasPreviousPage: boolean;

        items : T[];
    }
}
