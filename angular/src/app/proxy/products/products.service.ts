import type { CreateUpdateProductDto, ProductDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
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
      url: '/api/app/products',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/products/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'GET',
      url: `/api/app/products/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductDto>>({
      method: 'GET',
      url: '/api/app/products',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: CreateUpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/products/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}