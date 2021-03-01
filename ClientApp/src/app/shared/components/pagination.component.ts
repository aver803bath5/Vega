import { Component, Input, OnChanges} from "@angular/core";

@Component({
  selector: 'app-pagination',
  template: `
    <nav aria-label="Page navigation" *ngIf="pageSize < totalItems">
      <ul class="pagination justify-content-center">
        <li class="page-item" [class.disabled]="currentPage === 1">
          <a class="page-link" [routerLink]="['/vehicles']" [queryParams]="{pageNumber: currentPage - 1}">
            &laquo;
          </a>
        </li>
        <li *ngFor="let n of totalPage | range:1" class="page-item" [class.active]="currentPage == n">
          <a class="page-link" [routerLink]="['/vehicles']" [queryParams]="{pageNumber: n}">
            {{n}}
          </a>
        </li>
        <li class="page-item" [class.disabled]="currentPage === totalPage">
          <a class="page-link" [routerLink]="['/vehicles']" [queryParams]="{pageNumber: currentPage + 1}">
            &raquo;
          </a>
        </li>
      </ul>
    </nav>
  `
})
export class PaginationComponent implements OnChanges{
  @Input('total-items') totalItems:number = 0;
  @Input('pageSize') pageSize:number = 10;
  @Input('current-page') currentPage = 1;
  totalPage = 0;

  ngOnChanges(): void {
    this.totalPage = Math.ceil(this.totalItems / this.pageSize);
  }
}
