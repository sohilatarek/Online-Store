import type {
  CreateUpdateProductDto,
  ProductDto,
  GetProductsInput,
  UpdateStockDto,
  AdjustStockDto,
  BulkUpdateStockDto,
  CheckStockInput,
  StockCheckResultDto
} from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductsService {
  private restService = inject(RestService);
  apiName = 'Default';


  create = (input: CreateUpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: '/api/app/cached-product',
      body: input,
    },
      { apiName: this.apiName, ...config });


  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/cached-product/${id}`,
    },
      { apiName: this.apiName, ...config });


  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'GET',
      url: `/api/app/cached-product/${id}`,
    },
      { apiName: this.apiName, ...config });


  getList = (input: GetProductsInput, config?: Partial<Rest.Config>) => {

    const params: any = {
      sorting: input.sorting,
      skipCount: input.skipCount,
      maxResultCount: input.maxResultCount
    };


    if (input.categoryId != null) params.categoryId = input.categoryId;
    if (input.isActive != null) params.isActive = input.isActive;
    if (input.isPublished != null) params.isPublished = input.isPublished;


    if (input.searchTerm != null && input.searchTerm !== undefined && input.searchTerm !== '') {
      params.searchTerm = input.searchTerm;
      console.log('Adding searchTerm to params:', input.searchTerm);
    } else {
      console.log('searchTerm not included - value:', input.searchTerm, 'type:', typeof input.searchTerm);
    }

    if (input.minPrice != null) params.minPrice = input.minPrice;
    if (input.maxPrice != null) params.maxPrice = input.maxPrice;
    if (input.isLowStock != null) params.isLowStock = input.isLowStock;
    if (input.lowStockThreshold != null) params.lowStockThreshold = input.lowStockThreshold;
    if (input.isOutOfStock != null) params.isOutOfStock = input.isOutOfStock;

    console.log('Final params object:', params);

    return this.restService.request<any, PagedResultDto<ProductDto>>({
      method: 'GET',
      url: '/api/app/cached-product',
      params: params,
    },
      { apiName: this.apiName, ...config });
  }


  update = (id: number, input: CreateUpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/cached-product/${id}`,
      body: input,
    },
      { apiName: this.apiName, ...config });

  getByCategory = (categoryId: number, onlyPublished: boolean = false, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: `/api/app/cached-product/by-category/${categoryId}`,
      params: { onlyPublished },
    },
      { apiName: this.apiName, ...config });

  getPublished = (categoryId?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/cached-product/published-products',
      params: categoryId ? { categoryId } : {},
    },
      { apiName: this.apiName, ...config });

  publish = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: `/api/app/cached-product/${id}/publish`,
    },
      { apiName: this.apiName, ...config });

  unpublish = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: `/api/app/cached-product/${id}/unpublish`,
    },
      { apiName: this.apiName, ...config });

  updateStock = (id: number, input: UpdateStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/cached-product/${id}/stock`,
      body: input,
    },
      { apiName: this.apiName, ...config });

  adjustStock = (id: number, input: AdjustStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: `/api/app/cached-product/${id}/adjust-stock`,
      body: input,
    },
      { apiName: this.apiName, ...config });

  bulkUpdateStock = (input: BulkUpdateStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/cached-product/bulk-update-stock',
      body: input,
    },
      { apiName: this.apiName, ...config });

  checkStock = (input: CheckStockInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, StockCheckResultDto>({
      method: 'POST',
      url: '/api/app/cached-product/check-stock',
      body: input,
    },
      { apiName: this.apiName, ...config });

  getLowStock = (threshold?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/cached-product/low-stock',
      params: threshold ? { threshold } : {},
    },
      { apiName: this.apiName, ...config });

  getOutOfStock = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/cached-product/out-of-stock',
    },
      { apiName: this.apiName, ...config });
}
