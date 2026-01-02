import { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Component } from '@angular/core';
import { CategoriesService, CategoryDto } from 'src/app/proxy/categories';

@Component({
  selector: 'app-list-categories',
  imports: [],
  templateUrl: './list-categories.html',
  styleUrl: './list-categories.scss'
})
export class ListCategories {
  categories: CategoryDto[] = [];
  input: PagedAndSortedResultRequestDto = { maxResultCount: 10, skipCount: 0 };

  constructor(private categoriesService: CategoriesService) {
  }
  ngOnInit(): void {
    this.categoriesService.getList(this.input).subscribe(result => this.categories = result.items);
  }

}
