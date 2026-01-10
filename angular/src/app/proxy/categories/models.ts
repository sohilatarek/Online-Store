import type { EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CategoryDto extends FullAuditedEntityDto<number> {
  nameAr?: string;
  nameEn?: string;
  descriptionAr?: string;
  descriptionEn?: string;
  isActive: boolean;
  displayOrder: number;
  productCount: number;
}

export interface CreateUpdateCategoryDto extends EntityDto<number> {
  nameAr?: string;
  nameEn?: string;
  descriptionAr?: string;
  descriptionEn?: string;
  isActive: boolean;
  displayOrder: number;
}

export interface GetCategoriesInput extends PagedAndSortedResultRequestDto {
  isActive?: boolean;
  searchTerm?: string;
}
