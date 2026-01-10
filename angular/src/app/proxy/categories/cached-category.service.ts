import type { CategoryDto, CreateUpdateCategoryDto, GetCategoriesInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CachedCategoryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  activate = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'POST',
      url: `/api/app/cached-category/${id}/activate`,
    },
    { apiName: this.apiName,...config });
  

  canDelete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: `/api/app/cached-category/${id}/can-delete`,
    },
    { apiName: this.apiName,...config });
  

  changeDisplayOrder = (id: number, newOrder: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'POST',
      url: `/api/app/cached-category/${id}/change-display-order`,
      params: { newOrder },
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'POST',
      url: '/api/app/cached-category',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deactivate = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'POST',
      url: `/api/app/cached-category/${id}/deactivate`,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/cached-category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'GET',
      url: `/api/app/cached-category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getActiveList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto[]>({
      method: 'GET',
      url: '/api/app/cached-category/active-list',
    },
    { apiName: this.apiName,...config });
  

  getFilteredList = (input: GetCategoriesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CategoryDto>>({
      method: 'GET',
      url: '/api/app/cached-category/filtered-list',
      params: { isActive: input.isActive, searchTerm: input.searchTerm, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CategoryDto>>({
      method: 'GET',
      url: '/api/app/cached-category',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: CreateUpdateCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'PUT',
      url: `/api/app/cached-category/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}