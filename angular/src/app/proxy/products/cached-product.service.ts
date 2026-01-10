import type { AdjustStockDto, BulkUpdateStockDto, CheckStockInput, CreateUpdateProductDto, ProductDto, StockCheckResultDto, UpdateStockDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CachedProductService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  adjustStock = (id: number, input: AdjustStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: `/api/app/cached-product/${id}/adjust-stock`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  bulkUpdateStock = (input: BulkUpdateStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/cached-product/bulk-update-stock',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  checkStock = (input: CheckStockInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, StockCheckResultDto>({
      method: 'POST',
      url: '/api/app/cached-product/check-stock',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: '/api/app/cached-product',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/cached-product/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'GET',
      url: `/api/app/cached-product/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByCategory = (categoryId: number, onlyPublished?: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: `/api/app/cached-product/by-category/${categoryId}`,
      params: { onlyPublished },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductDto>>({
      method: 'GET',
      url: '/api/app/cached-product',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLowStock = (threshold?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/cached-product/low-stock',
      params: { threshold },
    },
    { apiName: this.apiName,...config });
  

  getOutOfStock = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/cached-product/out-of-stock',
    },
    { apiName: this.apiName,...config });
  

  getPublishedProducts = (categoryId?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/cached-product/published-products',
      params: { categoryId },
    },
    { apiName: this.apiName,...config });
  

  publish = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: `/api/app/cached-product/${id}/publish`,
    },
    { apiName: this.apiName,...config });
  

  unpublish = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: `/api/app/cached-product/${id}/unpublish`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: CreateUpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/cached-product/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateStock = (id: number, input: UpdateStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/cached-product/${id}/stock`,
      body: input,
    },
    { apiName: this.apiName,...config });
}