import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { PermissionService, PagedResultDto } from '@abp/ng.core';
import { CategoriesService, CategoryDto, GetCategoriesInput } from 'src/app/proxy/categories';
import { ConfirmationService } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-list-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './list-categories.html',
  styleUrl: './list-categories.scss'
})
export class ListCategories implements OnInit {
  private categoriesService = inject(CategoriesService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  private permissionService = inject(PermissionService);
  private confirmationService = inject(ConfirmationService);

  categories: CategoryDto[] = [];
  totalCount = 0;
  loading = false;
  searchForm: FormGroup;
  
  // Permissions
  canCreate = false;
  canEdit = false;
  canDelete = false;

  // Pagination
  page = 1;
  pageSize = 10;
  maxResultCount = 10;
  skipCount = 0;

  // Sorting
  sorting = 'displayOrder asc';

  constructor() {
    this.buildForm();
  }

  ngOnInit(): void {
    this.checkPermissions();
    this.loadCategories();
  }

  checkPermissions(): void {
    this.canCreate = this.permissionService.getGrantedPolicy('OnlineStore.Categories.Create');
    this.canEdit = this.permissionService.getGrantedPolicy('OnlineStore.Categories.Edit');
    this.canDelete = this.permissionService.getGrantedPolicy('OnlineStore.Categories.Delete');
  }

  buildForm(): void {
    this.searchForm = this.formBuilder.group({
      searchTerm: new FormControl(''),
      isActive: new FormControl(null),
    });
  }

  loadCategories(): void {
    this.loading = true;
    const input: GetCategoriesInput = {
      skipCount: this.skipCount,
      maxResultCount: this.maxResultCount,
      sorting: this.sorting,
      ...this.searchForm.value
    };

    // Remove null/undefined values
    Object.keys(input).forEach(key => {
      if (input[key] === null || input[key] === undefined || input[key] === '') {
        delete input[key];
      }
    });

    this.categoriesService.getFilteredList(input)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (result: PagedResultDto<CategoryDto>) => {
          this.categories = result.items;
          this.totalCount = result.totalCount;
        },
        error: (error) => {
          console.error('Error loading categories:', error);
        }
      });
  }

  search(): void {
    this.page = 1;
    this.skipCount = 0;
    this.loadCategories();
  }

  resetFilters(): void {
    this.searchForm.reset();
    this.page = 1;
    this.skipCount = 0;
    this.loadCategories();
  }

  onPageChange(page: number): void {
    this.page = page;
    this.skipCount = (page - 1) * this.maxResultCount;
    this.loadCategories();
  }

  onSort(sortField: string): void {
    if (this.sorting === `${sortField} asc`) {
      this.sorting = `${sortField} desc`;
    } else {
      this.sorting = `${sortField} asc`;
    }
    this.loadCategories();
  }

  getSortIcon(field: string): string {
    if (this.sorting.startsWith(field)) {
      return this.sorting.endsWith('asc') ? '↑' : '↓';
    }
    return '';
  }

  addCategory(): void {
    this.router.navigateByUrl('/categories/add');
  }

  editCategory(id: number): void {
    this.router.navigateByUrl(`/categories/edit/${id}`);
  }

  deleteCategory(category: CategoryDto): void {
    // Check if category can be deleted
    this.categoriesService.canDelete(category.id).subscribe({
      next: (canDelete) => {
        if (!canDelete) {
          this.confirmationService.warn(
            `Cannot delete category "${category.nameEn || category.nameAr}" because it has associated products.`,
            'Cannot Delete Category',
            { messageLocalizationParams: [] }
          ).subscribe();
          return;
        }

        this.confirmationService.warn(
          `Are you sure you want to delete "${category.nameEn || category.nameAr}"?`,
          'Delete Category',
          { messageLocalizationParams: [] }
        ).subscribe((status) => {
          if (status === 'confirm') {
            this.categoriesService.delete(category.id).subscribe({
              next: () => {
                this.loadCategories();
              },
              error: (error) => {
                console.error('Error deleting category:', error);
              }
            });
          }
        });
      },
      error: (error) => {
        console.error('Error checking if category can be deleted:', error);
      }
    });
  }

  activateCategory(category: CategoryDto): void {
    this.categoriesService.activate(category.id).subscribe({
      next: () => {
        this.loadCategories();
      },
      error: (error) => {
        console.error('Error activating category:', error);
      }
    });
  }

  deactivateCategory(category: CategoryDto): void {
    this.categoriesService.deactivate(category.id).subscribe({
      next: () => {
        this.loadCategories();
      },
      error: (error) => {
        console.error('Error deactivating category:', error);
      }
    });
  }

  getTotalPages(): number {
    return Math.ceil(this.totalCount / this.maxResultCount);
  }

  getPageNumbers(): number[] {
    const totalPages = this.getTotalPages();
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.page - Math.floor(maxPages / 2));
    let endPage = Math.min(totalPages, startPage + maxPages - 1);
    
    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  getDisplayRange(): string {
    const start = this.skipCount + 1;
    const end = Math.min(this.skipCount + this.maxResultCount, this.totalCount);
    return `${start} to ${end}`;
  }
}
