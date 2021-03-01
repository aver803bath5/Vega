import { Component, EventEmitter, Input, Output } from "@angular/core";
import { IPagination } from "../models/IPagination";

@Component({
  selector: 'app-pagination',
  template: `
    <nav aria-label="Page navigation" *ngIf="pagination !== null && pagination.totalCount > pagination.pageSize">
      <ul class="pagination justify-content-center">
        <li class="page-item" [class.disabled]="!pagination.hasPrevious">
          <a class="page-link" [routerLink]="['/vehicles']" [queryParams]="{pageNumber: (pagination.currentPage - 1)}">
            &laquo;
          </a>
        </li>
        <li *ngFor="let n of pagination?.totalPage | range:1" class="page-item" [class.active]="pagination.currentPage == n">
          <a class="page-link" [routerLink]="['/vehicles']" [queryParams]="{pageNumber: n}">
            {{n}}
          </a>
        </li>
        <li class="page-item" [class.disabled]="!pagination.hasNext">
          <a class="page-link" [routerLink]="['/vehicles']" [queryParams]="{pageNumber: (pagination.currentPage + 1)}">
            &raquo;
          </a>
        </li>
      </ul>
    </nav>
  `
})
export class PaginationComponent {
  @Input('pagination') pagination: IPagination
  @Output('page-changed') pageChanged = new EventEmitter();
}
