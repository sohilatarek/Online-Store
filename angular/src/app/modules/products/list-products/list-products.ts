import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { PermissionService, PagedResultDto, RestService, Rest } from '@abp/ng.core';
import { ProductsService, ProductDto, GetProductsInput } from 'src/app/proxy/products';
import { CategoriesService, CategoryDto } from 'src/app/proxy/categories';
import { ConfirmationService } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-list-products',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './list-products.html',
  styleUrl: './list-products.scss'
})
export class ListProducts implements OnInit {
  private productsService = inject(ProductsService);
  private categoriesService = inject(CategoriesService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  private permissionService = inject(PermissionService);
  private confirmationService = inject(ConfirmationService);
  private restService = inject(RestService);

  products: ProductDto[] = [];
  categories: CategoryDto[] = [];
  totalCount = 0;
  loading = false;
  searchForm: FormGroup;

  // Permissions
  canCreate = false;
  canEdit = false;
  canDelete = false;
  canPublish = false;
  canManageStock = false;

  // Pagination
  page = 1;
  pageSize = 20;
  maxResultCount = 20;
  skipCount = 0;

  // Sorting
  sorting = 'creationTime desc';

  constructor() {
    this.buildForm();
  }

  ngOnInit(): void {
    this.checkPermissions();
    this.loadCategories();
    this.loadProducts();
  }

  checkPermissions(): void {
    this.canCreate = this.permissionService.getGrantedPolicy('OnlineStore.Products.Create');
    this.canEdit = this.permissionService.getGrantedPolicy('OnlineStore.Products.Edit');
    this.canDelete = this.permissionService.getGrantedPolicy('OnlineStore.Products.Delete');
    this.canPublish = this.permissionService.getGrantedPolicy('OnlineStore.Products.Publish');
    this.canManageStock = this.permissionService.getGrantedPolicy('OnlineStore.Products.ManageStock');
  }

  buildForm(): void {
    this.searchForm = this.formBuilder.group({
      searchTerm: new FormControl(''),
      categoryId: new FormControl(null),
      isActive: new FormControl(null),
      isPublished: new FormControl(null),
      minPrice: new FormControl(null),
      maxPrice: new FormControl(null),
      isLowStock: new FormControl(null),
      isOutOfStock: new FormControl(null),
    });
  }

  loadCategories(): void {
    this.categoriesService.getList({ maxResultCount: 1000, skipCount: 0 }).subscribe(res => {
      this.categories = res.items;
    });
  }

  loadProducts(): void {
    this.loading = true;

    // Get form values
    const formValue = this.searchForm.value;
    console.log('Form value before processing:', formValue);
    console.log('Form searchTerm:', formValue.searchTerm);

    const input: GetProductsInput = {
      skipCount: this.skipCount,
      maxResultCount: this.maxResultCount,
      sorting: this.sorting,
      ...formValue
    };

    const searchTermValue = formValue.searchTerm;
    console.log('Raw searchTerm from form:', searchTermValue, 'type:', typeof searchTermValue);

    if (searchTermValue != null && searchTermValue !== undefined && searchTermValue !== '') {
      if (typeof searchTermValue === 'string') {
        const trimmed = searchTermValue.trim();
        if (trimmed !== '') {
          input.searchTerm = trimmed;
          console.log('Keeping searchTerm:', trimmed);
        } else {
          delete input.searchTerm;
          console.log('Deleted searchTerm - empty after trim');
        }
      } else {
        delete input.searchTerm;
        console.log('Deleted searchTerm - not a string');
      }
    } else {
      delete input.searchTerm;
      console.log('Deleted searchTerm - null/undefined/empty');
    }

    Object.keys(input).forEach(key => {
      if (key !== 'searchTerm' && (input[key] === null || input[key] === undefined || input[key] === '')) {
        delete input[key];
      }
    });

    console.log('Sending search request with input:', input);
    console.log('SearchTerm value:', input.searchTerm);
    console.log('Has searchTerm?', 'searchTerm' in input);

    const params: any = {
      sorting: input.sorting,
      skipCount: input.skipCount,
      maxResultCount: input.maxResultCount
    };

    if (input.categoryId != null) params.categoryId = input.categoryId;
    if (input.isActive != null) params.isActive = input.isActive;
    if (input.isPublished != null) params.isPublished = input.isPublished;
    if (input.searchTerm != null && input.searchTerm !== '' && input.searchTerm !== undefined) {
      params.searchTerm = input.searchTerm;
      console.log('✅ Adding searchTerm to params:', input.searchTerm);
    } else {
      console.log('❌ searchTerm NOT added - value:', input.searchTerm);
    }
    if (input.minPrice != null) params.minPrice = input.minPrice;
    if (input.maxPrice != null) params.maxPrice = input.maxPrice;
    if (input.isLowStock != null) params.isLowStock = input.isLowStock;
    if (input.lowStockThreshold != null) params.lowStockThreshold = input.lowStockThreshold;
    if (input.isOutOfStock != null) params.isOutOfStock = input.isOutOfStock;

    console.log('Final params being sent:', params);
    console.log('Params keys:', Object.keys(params));

    // Call RestService directly to ensure params are sent correctly
    this.restService.request<any, PagedResultDto<ProductDto>>({
      method: 'GET',
      url: '/api/app/cached-product',
      params: params,
    }, { apiName: 'Default' })
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (result: PagedResultDto<ProductDto>) => {
          this.products = result.items;
          this.totalCount = result.totalCount;
        },
        error: (error) => {
          console.error('Error loading products:', error);
        }
      });
  }

  search(): void {
    this.page = 1;
    this.skipCount = 0;

    console.log('Search form value:', this.searchForm.value);
    this.loadProducts();
  }

  resetFilters(): void {
    this.searchForm.reset();
    this.page = 1;
    this.skipCount = 0;
    this.loadProducts();
  }

  onPageChange(page: number): void {
    this.page = page;
    this.skipCount = (page - 1) * this.maxResultCount;
    this.loadProducts();
  }

  onSort(sortField: string): void {
    if (this.sorting === `${sortField} asc`) {
      this.sorting = `${sortField} desc`;
    } else {
      this.sorting = `${sortField} asc`;
    }
    this.loadProducts();
  }

  getSortIcon(field: string): string {
    if (this.sorting.startsWith(field)) {
      return this.sorting.endsWith('asc') ? '↑' : '↓';
    }
    return '';
  }

  addProduct(): void {
    this.router.navigateByUrl('/products/add');
  }

  editProduct(id: number): void {
    this.router.navigateByUrl(`/products/edit/${id}`);
  }

  deleteProduct(product: ProductDto): void {
    this.confirmationService.warn(
      `Are you sure you want to delete "${product.nameEn || product.nameAr}"?`,
      'Delete Product',
      { messageLocalizationParams: [] }
    ).subscribe((status) => {
      if (status === 'confirm') {
        this.productsService.delete(product.id).subscribe({
          next: () => {
            this.loadProducts();
          },
          error: (error) => {
            console.error('Error deleting product:', error);
          }
        });
      }
    });
  }

  publishProduct(product: ProductDto): void {
    if (product.isPublished) {
      this.productsService.unpublish(product.id).subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (error) => {
          console.error('Error unpublishing product:', error);
        }
      });
    } else {
      this.productsService.publish(product.id).subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (error) => {
          console.error('Error publishing product:', error);
        }
      });
    }
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

  formatPrice(price: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  }

  getStockStatusClass(product: ProductDto): string {
    if (product.isOutOfStock) return 'stock-danger';
    if (product.isLowStock) return 'stock-warning';
    return 'stock-success';
  }

  getStockStatusText(product: ProductDto): string {
    if (product.isOutOfStock) return 'Out of Stock';
    if (product.isLowStock) return 'Low Stock';
    return 'In Stock';
  }

  getDisplayRange(): string {
    const start = this.skipCount + 1;
    const end = Math.min(this.skipCount + this.maxResultCount, this.totalCount);
    return `${start} to ${end}`;
  }

  manageStock(product: ProductDto): void {
    const newStock = prompt(`Update stock for "${product.nameEn || product.nameAr}"\nCurrent stock: ${product.stockQuantity}\nEnter new stock quantity:`, product.stockQuantity.toString());

    if (newStock !== null) {
      const stockValue = parseInt(newStock, 10);
      if (!isNaN(stockValue) && stockValue >= 0) {
        this.productsService.updateStock(product.id, { stockQuantity: stockValue }).subscribe({
          next: () => {
            this.loadProducts();
          },
          error: (error) => {
            console.error('Error updating stock:', error);
            alert('Failed to update stock. Please try again.');
          }
        });
      } else {
        alert('Please enter a valid number (0 or greater)');
      }
    }
  }
}
